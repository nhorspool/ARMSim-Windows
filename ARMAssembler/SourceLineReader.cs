// File SourceLineReader.cs
//
// Given the location-source line mapping info obtained from
// an Elf/Dwarf2 object file, this class retrieves the
// source line for a given location in an object file.
//
// Note: the location-source mappings are created by gcc for
// files compiled from a high-level language such as C.
// Thus a SourceLineReader instance will normally be used
// for reading a C/C++ source file.
//
// Copyright (c) R. Nigel Horspool,  August 2005 - August 2007


using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;


namespace ArmAssembly 
{


public class FileDirectoryPair : IDisposable {
	public string FileName, dir;
	public StreamReader sr;
	protected int lastLine = 0;
	protected bool triedOpen = false;

	public FileDirectoryPair( string f, string d ) {
		FileName = f; dir = d;
	}

	public override string ToString() {
		return String.Format("{0} (directory {1})",FileName,dir);
	}

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool d)
    {
        if (d) {
            sr.Dispose();
            sr = null;
        }
    }
	// Note: only forwards progress through the file is allowed.
	// Invocation with ln <= the number of the last line read
	// yields a null result.
	public string ReadLine( int ln ) {
		if (sr == null) {
			if (triedOpen) return null;
			OpenFile();
			if (sr == null) return null;
		}
		string lastLineRead = null;
		while(lastLine < ln) {
			lastLineRead = sr.ReadLine();
			if (lastLineRead == null) break;
			lastLine++;
		}
		return lastLineRead;
	}

	public void OpenFile() {
		triedOpen = true;
		try {
			string currdir = Directory.GetCurrentDirectory();
			ensureWindowsPath(ref dir);
			Directory.SetCurrentDirectory(dir);
			sr = new StreamReader(File.OpenRead(this.FileName));
			Directory.SetCurrentDirectory(currdir);
		} catch( IOException ) {
			sr = null;
		}
		if (sr == null) return;
		// initialize the file reading state
		lastLine = 0;
	}

	public void CloseFile() {
		if (sr != null) {
			sr.Close();
			sr = null;
			triedOpen = false;
		}
	}

	// convert a cygwin path into a Windows path, if required
	static protected void ensureWindowsPath(ref string dir) {
		if (!dir.StartsWith("/"))
			return;  // not a cygwin path
		string cygwinBase = null;
		RegistryKey rk = Registry.LocalMachine;
		string subKeyName = "SOFTWARE\\Cygnus Solutions\\Cygwin\\mounts v2\\/";
		using(RegistryKey sk = rk.OpenSubKey(subKeyName)) {
			if (sk != null)
				cygwinBase = sk.GetValue("native").ToString();
		}
		if (cygwinBase == null) return;
		// prefix with cygwin path and convert to Windows path
		dir = cygwinBase + dir.Replace('/', '\\');
	}
}

public class SourceLineReader : IDisposable {
	protected List<FileDirectoryPair> sourceFiles;
	protected byte[] debugLineSect;
	protected string compilationDirectory = ".";
	protected string fileName;
	protected bool unsupportedFileFormat = false;
	int trace = 2;

	// The state of the FSM for decoding address:line number info
	// is captured by the following variables
	int address;
    int column; // useful only for debugging?
	int file;  // an index into the sourceFiles list
	bool is_stmt;

	// records whether the FSM has been initialized
	bool firstTime = true;
	// records whether FSM registers need resetting
	bool reInit = true;

	// some properties of the FSM derived from the prologue
	uint min_insn_length, line_range, opcode_base;
	int line_base, lastIx;
	// byte[] std_opcode_lengths;
	List<string> include_dirs;

	// Our position in the FSM is currIx
	int currIx;

	// properties

    public bool BasicBlock { get; private set; }

    public bool EndSequence { get; private set; }

    public int Line { get; private set; }

	public FileDirectoryPair File { get{ return sourceFiles[file]; } }

	// end of properties

	public SourceLineReader( byte[] debugLineSect, string fileName ) {
		this.debugLineSect  = debugLineSect;
		this.fileName = fileName;
	}

	public void SetCompDirectory( string directory ) {
		this.compilationDirectory = directory;
	}

	public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
	}

    protected virtual void Dispose(bool d)
    {
        if (d)
        {
            foreach (FileDirectoryPair fd in sourceFiles)
            {
                if (fd != null && fd.sr != null)
                    fd.CloseFile();
            }
        }
    }

	// returns a list of strings
	public IList<string> GetSourceLines( int address ) {
		if (unsupportedFileFormat) return null;
		return processLineNumberInfo(address);
	}

	// This method uses the line number information section in an Elf
	// object code file, where the info is in Dwarf2 format.
	// Specs for Dwarf2 taken from www.arm.com/pdfs/TIS-DWARF2.pdf
	// The result is an array of source code lines, representing
	// all lines that follow the result of the previous call up to
	// and including the first line whose address is >= the
	// argument address.
	protected IList<string> processLineNumberInfo( int addressToFind ) {
		if (firstTime) {
			currIx = 0;
			uint sectLength = getInt32(ref currIx); // do not delete
			uint version = getInt16(ref currIx);
			if (version != 2) {
				Debug.WriteLine(String.Format(
					"Object file {0} does not use Dwarf2 format", fileName));
				unsupportedFileFormat = true;
				return null;
			}
			uint prologue_length = getInt32(ref currIx);
			min_insn_length = (uint)debugLineSect[currIx];  currIx += 2;
			line_base = (sbyte)debugLineSect[currIx++];
			line_range = (uint)debugLineSect[currIx++];
			opcode_base = (uint)debugLineSect[currIx++];
			// std_opcode_lengths = new byte[opcode_base];
			// skip over standard_opcode_lengths
			currIx = (int)(14 + opcode_base);
			// read list of include directories
			include_dirs = new List<string>(4);
			include_dirs.Add(compilationDirectory);
			while(debugLineSect[currIx] != 0) {
				string dir = getString(ref currIx);
				include_dirs.Add(dir);
			}
			currIx++;  // skip null byte
			// read list of source files
			sourceFiles = new List<FileDirectoryPair>(4);
			sourceFiles.Add(null);
			while(debugLineSect[currIx] != 0) {
				string fn = getString(ref currIx);
				int dir_ix = (int)getLEB128(ref currIx);
                uint time_last_mod = getLEB128(ref currIx); // do not delete
                uint file_length = getLEB128(ref currIx); // do not delete
				sourceFiles.Add(new FileDirectoryPair(fn,include_dirs[dir_ix]));
			}
			currIx = (int)(10 + prologue_length);
			lastIx = debugLineSect.Length;
			reInit = true;
		}
		if (reInit) {
			// initialize the state machine registers
			address = 0;
			file = 1;	// the main compilation file
			Line = 1;
			column = 0;
			is_stmt = (debugLineSect[11] != 0);
            BasicBlock = false;
            EndSequence = false;
			firstTime = false;
			reInit = false;
		}
		int prevLine = 0;
		int prevFile = 0;
		List<string> result = new List<string>(5);
		// process the state machine operations
		for( ; ; ) {
			if (address > addressToFind) 
				break;
			if (currIx >= lastIx)
				break;
			byte opcode = debugLineSect[currIx++];
			if (opcode < opcode_base) {
				switch((DW_LNS)opcode) {
					case DW_LNS.copy:
                        BasicBlock = false;
						if (address >= addressToFind) goto Done;
						break;
					case DW_LNS.advance_pc:
						address += (int)(getLEB128(ref currIx)*min_insn_length);
						break;
					case DW_LNS.advance_line:
						Line += (int)getLEB128(ref currIx);
						break;
					case DW_LNS.set_file:
						file = (int)getLEB128(ref currIx);
						break;
					case DW_LNS.set_column:
						column = (int)getLEB128(ref currIx);
						break;
					case DW_LNS.negate_stmt:
						is_stmt = !is_stmt;
						break;
					case DW_LNS.set_basic_block:
                        BasicBlock = true;
						break;
					case DW_LNS.const_add_pc:
						address += (int)((255-opcode_base)/line_range);
						break;
					case DW_LNS.fixed_advance_pc:
						address += getInt16(ref currIx);
						break;
					case DW_LNS.extended_op:
						uint len = getLEB128(ref currIx);
						int nextIx = (int)(currIx + len);
						opcode = debugLineSect[currIx++];
						switch((DW_LNE)opcode) {
							case DW_LNE.end_sequence:
                                EndSequence = true;
								if (address >= addressToFind) {
									reInit = true;
									goto Done;
								}
								address = 0;  file = 1;
								Line = 1;
                                //column = 0;
								is_stmt = (debugLineSect[11] != 0);
								BasicBlock = false;
								break;
							case DW_LNE.set_address:
								address = (int)getInt32(ref currIx);
								break;
							case DW_LNE.define_file:
								string fn = getString(ref currIx);
								int dir_ix = (int)getLEB128(ref currIx);
								uint time_last_mod = getLEB128(ref currIx);
								uint file_length = getLEB128(ref currIx);
								sourceFiles.Add(
									new FileDirectoryPair(fn,include_dirs[dir_ix]));
								break;
							default:
								Debug.WriteLine(String.Format(
									"unimplemented extended line number op ({0})",opcode));
								break;
						}
						currIx = nextIx;
						break;
				}
			} else {	// special opcode
				opcode  -= (byte)(opcode_base);
				address += (int)((opcode/line_range)*min_insn_length);
				Line    += (int)(line_base + (opcode%line_range));
                BasicBlock = false;
				if (address >= addressToFind) goto Done;
			}
		}
Done:
		if (prevLine != Line || prevFile != file) {
			if (prevFile != file) {
				prevLine = 0;
				prevFile = file;
			}
			while(prevLine < Line)  {
				string s = sourceFiles[file].ReadLine(++prevLine);
				if (s != null)
					result.Add(s);
			}
		}
		if (trace>0)
			Debug.WriteLine(String.Format(
				" LN: addr=0x{0:X}, line#={1,-2}, flags={2}{3}{4}, file={5}",
                address, Line, is_stmt ? "T" : "F", BasicBlock ? "T" : "F",
                EndSequence ? "T" : "F", sourceFiles[file]));
		return result.Count>0? result : null;
	}

	// get little-endian 32 bit integer from a byte array
	protected uint getInt32( ref int offset ) {
		uint result = 0;
		for ( int i=3;  i>=0;  i-- )
			result = (uint)((result<<8) + (debugLineSect[offset+i]&0xff));
		offset += 4;
		return result;
	}

	// get little-endian 16 bit integer from a byte array
	protected ushort getInt16( ref int offset ) 
	{
		offset += 2;
		return (ushort)((debugLineSect[offset-2] & 0xff)
				+ ((debugLineSect[offset-1]&0xff)<<8));
	}

	// Decode variable length number in 'Little endian base 128' format
	protected uint getLEB128( ref int currIndex ) {
		uint result = 0;
		int shift = 0;
		for( ; ; ) {
			uint b = (uint)debugLineSect[currIndex++];
			result |= (uint)((b & 0x7f) << shift);
			if ((b & 0x80) == 0) break;
			shift += 7;
		}
		return result;
	}

	// Copy a variable-length string and skip over final null byte
	protected string getString( ref int offset ) {
		StringBuilder sb = new StringBuilder();
		for( ; ; ) {
			byte b = debugLineSect[offset++];
			if (b == 0) break;
			sb.Append((char)b);
		}
		return sb.ToString();
	}

}

} // namespace
