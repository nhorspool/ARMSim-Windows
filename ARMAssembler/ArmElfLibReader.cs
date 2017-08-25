// File ArmElfLibReader.cs
//
// Extracts ARM Elf format object files from the archive libraries
// distributed by CodeSourcery.
//
// It requires the gcc format of library as produced by the ar command.
// For symbol resolution, it requires that a symbol table section
// as produced by the ranlib command be present.
//
// Copyright (c) R. Nigel Horspool,  October 2007


using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ArmAssembly {


public class ArmElfLibReader : IDisposable {
	protected FileStream fs;
	protected byte[] extendedFileNames;
	bool trace = true;
	IDictionary<string,uint> ranlibTable;

	// accessor methods

    public string LibraryName { get; private set; }

	// end of accessor methods

	public ArmElfLibReader( string libName ):
		this(libName,false)  { }

	public ArmElfLibReader( string libName, bool trace )  {
        this.LibraryName = libName;
		this.trace = trace;
		openFile();
	}

	protected void openFile() {
		try {
            if (!LibraryName.EndsWith(".a"))
				throw new AsmException(
                    "Library file {0} does not have \".a\" suffix", LibraryName);
            fs = new FileStream(LibraryName, FileMode.Open, FileAccess.Read);
            string result = checkFile();
			if (result != null)
				throw new AsmException(
                    "Library file {0} has invalid format: {1}", LibraryName, result);
		} catch( IOException e ) {
			fs.Close();
			fs = null;
			throw new AsmException(
                "IO error reading file {0}:\n{1}", LibraryName, e);
		}
	}

	public void CloseFile() {
		if (fs == null) return;
		fs.Close();
		fs = null;
	}

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool d)
    {
        if (d) {
            //CloseFile();
            ranlibTable.Clear();
            extendedFileNames = null;
        }
    }

	// Returns a filestream positioned at the start of an object code file
	// which defines the symbol provided as the first argument.
	// The filename argument returns the name of that object code file.
	// If the file cannot be located, the result is null and the filename
	// result is null.
	public FileStream GetLibraryFile( string symbol, out string fileName ) {
		fileName = null;
		uint offset = 0;
        if (ranlibTable == null || !ranlibTable.TryGetValue(symbol, out offset))
			return null;
		try {
			if (fs == null)
                fs = new FileStream(LibraryName, FileMode.Open, FileAccess.Read);
			fs.Seek((long)offset, SeekOrigin.Begin);
			byte[] filePreamble = new byte[60];
			int len = fs.Read(filePreamble, 0, filePreamble.Length);
			if (len != filePreamble.Length) {
				CloseFile();
				throw new AsmException(
                    "Unexpected EOF reading file {0}", LibraryName);
			}
			fileName = getFileName(0, filePreamble);
		} catch( IOException e ) {
			CloseFile();
			throw new AsmException(
                "I/O error reading file {0}:\n{1}", LibraryName, e);
		}
		return fs;
	}

	// Checks file format and extracts the symbol table and extended
	// string sections.
	protected string checkFile() {
		byte[] header = new byte[8];
		int len = fs.Read(header, 0, header.Length);
		if (len < header.Length) return "truncated read of first 8 bytes";
		if (trace)
			Console.WriteLine("Library: {0}, magic number = {1}",
				LibraryName, getString(0, header, 8));
		if (getString(0, header, 8) != "!<arch>\n")
			return "bad magic number";	// bad magic number
		// now search for and read the symbol table section and the extended
		// filename section
		long filePos = 8;
        long fileLength = fs.Length;
        while (filePos < fileLength) {
			if (trace)
				Debug.WriteLine(String.Format("* File offset = {0,6:X6}", filePos));
			byte[] fileHeader = new byte[60];
			len = fs.Read(fileHeader, 0, fileHeader.Length);
            if (len != fileHeader.Length)
            {
                Debug.WriteLine(String.Format(
                    "!! Requested {0}, read {1} bytes", fileHeader.Length, len));
                return "truncated read of file header";
            }
			filePos += len;
			string memberName = getFileName(0, fileHeader);
			int memberLength = getDecNumber(48, fileHeader);
			if (trace)
				Debug.WriteLine(
                    String.Format("* File: \"{0}\", length = {1} (0x{1,6:X6})",
                    memberName, memberLength));
			string fileMagic = getString(58, fileHeader, 2); // do not delete
			if (fileHeader[58] != (byte)0x60 || fileHeader[59] != (byte)0x0A) {
				if (trace)
					Debug.WriteLine(
                        String.Format("* Magic code = {0,1:X1}, {1,1:X1}",
						fileHeader[58], fileHeader[59]));
				return "bad magic code at end of header";
			}
            if (memberName == "/") {
                extendedFileNames = new byte[memberLength];
                if (memberLength != fs.Read(extendedFileNames, 0, memberLength))
                    return "truncated read of extended filenames table";
            }
            else if (String.IsNullOrEmpty(memberName)) {
                byte[] sytab = new byte[memberLength];
                if (memberLength != fs.Read(sytab, 0, memberLength))
					return "truncated read of symbol table";
				convertSymTab(sytab);
				sytab = null;
			}
			if (ranlibTable != null && extendedFileNames != null)
				break;
            if ((memberLength & 1) != 0)
                memberLength++;
            filePos += memberLength;
			fs.Seek(filePos,SeekOrigin.Begin);
		}
        if (ranlibTable == null)
            Debug.WriteLine("No ranlib section found");
		return null;
	}

	// Gets a string from a byte array, given its offset & length.
	static protected string getString( int offset, byte[] section, int len ) {
		StringBuilder sb = new StringBuilder();
		while(len-- > 0) {
			byte b = section[offset++];
			if (b == 0) break;
			sb.Append((char)b);
		}
		return sb.ToString();
	}

	// Gets a string from a byte array, given its offset & length.
	static protected string getNullTerminatedString( ref int offset, byte[] section ) {
		StringBuilder sb = new StringBuilder();
		for( ; ; ) {
			byte b = section[offset++];
			if (b == 0) break;
			sb.Append((char)b);
		}
		return sb.ToString();
	}

	// Gets filename from a byte array, given its offset.
	protected string getFileName( int offset, byte[] section ) {
		StringBuilder sb = new StringBuilder(16);
		int len = 16;
		byte b;
		while(len-- > 0) {
			b = section[offset++];
			if (b == '/') break;
			sb.Append((char)b);
		}
		if (len == 15) {
			b = section[offset];
			if (b >= (byte)'0' && b <= (byte)'9') {
				// it's an extended filename
				if (extendedFileNames == null) return null;
				int pos = getDecNumber(offset, section);
                for (; ; ) {
                    b = extendedFileNames[pos++];
                    if (b == '/') break;
                    sb.Append((char)b);
                }
                return sb.ToString();
			}
			if (b == (byte)'/')
				return "/";
		}
		return sb.ToString();
	}

	// Gets int value expressed in decimal from byte array.
	static protected int getDecNumber( int offset, byte[] section ) {
		// get and check first digit
		int result = section[offset++] - (byte)'0';
		if (result < 0 || result > 9)
			throw new AsmException("Decimal number expected");
		// get and convert trailing digits
		for( ; ; ) {
			int b = section[offset++] - (byte)'0';
			if (b < 0 || b > 9) break;
			result = result*10 + b;
		}
		return result;
	}

	// Creates a dictionary of symbols defined within the archive
	protected void convertSymTab( byte[] sytab ) {
		ranlibTable = new Dictionary<string,uint>();
		uint numSymbols = getInt32(sytab,0);
		int offsetPos = 4;
		int namePos = offsetPos + 4 * (int)numSymbols;
		if (trace)
			Debug.WriteLine(String.Format("* Symbol table size = {0}", numSymbols));
		while(numSymbols-- > 0) {
			uint offset = getInt32(sytab,offsetPos);
			string symbol = getNullTerminatedString(ref namePos, sytab);
			if (trace)
				Debug.WriteLine(String.Format("Symbol table entry: 0x{0,6:X6} / {1}",
					offset, symbol));
			ranlibTable[symbol] = offset;
			offsetPos += 4;
		}
	}

	// get big-endian 32 bit integer from a byte array
	static protected uint getInt32( byte[] arr, int offset ) {
		uint result = 0;
		for(int i=0;  i<=3;  i++ )
			result = (uint)((result<<8) + (arr[offset+i]&0xff));
		return result;
	}


#if STANDALONETEST
	static void Main( string[] args ) {
		bool trace = false;
		ArmElfLibReader t = null;
		foreach( string s in args ) {
			if (s == "/t") {
				trace = true;
				continue;
			}
			try {
				if (t == null) {
					t = new ArmElfLibReader(s, trace);
				} else {
					string name;
					FileStream lib = t.GetLibraryFile(s, out name);
					if (name == null) name = "<<null>>";
					Console.WriteLine("-- Looked up {0}, returned {1}", s, name);
				}
			} catch(Exception e) {
				Console.WriteLine("Caught exception:\n\t{0}", e);
			}
		}
	}
#endif

}

#if STANDALONETEST
    class AsmException : Exception
    {
        public AsmException() { }

        public AsmException(String msg, params Object[] args)
            :
            base("Error during test:\n\t" +
                String.Format(msg, args)) { }
    }
#endif
}