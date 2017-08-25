// File ArmElfReader.cs
//
// Reads a relocatable object file in Elf format.
//
// Documentation for the format of entries in the relocation section
// comes from "ELF for the ARM® Architecture", ARM IHI 0044D,
// 28th October 2009:
//    http://infocenter.arm.com/help/topic/com.arm.doc.ihi0044d/IHI0044D_aaelf.pdf
//
// Also some additional typecodes were obtained from
//    http://www.sco.com/developers/gabi/latest/contents.html
// System V Application Binary Interface - DRAFT - 26 October 2009
//
// Debugging information in Dwarf2 format is read from the object
// file to obtain mappings from locations to source code line.
//
// Copyright (c) R. Nigel Horspool,  August 2005 - September 2010

#define TRACE

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ArmAssembly {

public enum SHT {	// Section Header Table typecodes
		NULL = 0, PROGBITS = 1, SYMTAB = 2, STRTAB = 3, RELA = 4,
		HASH = 5, DYNAMIC = 6, OTHER = 7, NOBITS = 8, REL = 9,
		SHLIB = 10, DYNSYM = 11, INIT_ARRAY = 14, FINI_ARRAY = 15,
        PREINIT_ARRAY = 16, GROUP = 17, SYMTAB_SHNDX = 18,
        SHT_ARM_ATTRIBUTES = 0x70000003 };

public enum SHN {	// Special Section Numbers
		UNDEF = 0, LOPROC = 0xff00, HIPROC = 0xff1f, ABS = 0xfff1,
		COMMON = 0xfff2 };

public enum STB {
		LOCAL = 0, GLOBAL = 1, WEAK = 2 };

public struct ST {	// Symbol Table entry
	public string name;
	public uint	value, size;
	public byte	info, other;
	public ushort shndx;
}

public enum DW_LNS {  // Line number opcodes
		extended_op = 0, copy = 1, advance_pc = 2, advance_line = 3,
		set_file = 4, set_column = 5, negate_stmt = 6,
		set_basic_block = 7, const_add_pc = 8, fixed_advance_pc = 9 };
	
public enum DW_LNE {  // Line number extended opcodes
		None = 0, end_sequence = 1, set_address = 2, define_file = 3 };

public enum DW_AT {  // Attributes
        None = 0,
		sibling = 0x01, location = 0x02, name = 0x03,
		ordering = 0x09, subscr_data = 0x0a, byte_size = 0x0b,
		bit_offset = 0x0c, bit_size = 0x0d, element_list = 0x0f,
		stmt_list = 0x10, low_pc = 0x11, high_pc = 0x12,
		language = 0x13, member = 0x14, discr = 0x15,
		discr_value = 0x16, visibility = 0x17, import = 0x18,
		string_length = 0x19, common_reference = 0x1a,
		comp_dir = 0x1b, const_value = 0x1c, containing_type = 0x1d,
		default_value = 0x1e, inline = 0x20, is_optional = 0x21,
		lower_bound = 0x22, producer = 0x25, prototyped = 0x27,
		return_addr = 0x2a, start_scope = 0x2c, stride_size = 0x2e,
		upper_bound = 0x2f, abstract_origin = 0x31,
		accessibility = 0x32, address_class = 0x33, artificial = 0x34,
		base_types = 0x35, calling_convention = 0x36, count = 0x37,
		data_member_location = 0x38, decl_column = 0x39,
		decl_file = 0x3a, decl_line = 0x3b, declaration = 0x3c,
		discr_list = 0x3d, encoding = 0x3e, external = 0x3f,
		frame_base = 0x40, friend = 0x41, identifier_case = 0x42,
		macro_info = 0x43, namelist_items = 0x44, priority = 0x45,
		segment = 0x46, specification = 0x47, static_link = 0x48,
		type = 0x49, use_location = 0x4a, variable_parameter = 0x4b,
		virtuality = 0x4c, vtable_elem_location = 0x4d };

public enum DW_FORM {
        None = 0,
		addr = 0x01, block2 = 0x03, block4 = 0x04, data2 = 0x05,
		data4 = 0x06, data8 = 0x07, dwstring = 0x08, block = 0x09,
		block1 = 0x0a, data1 = 0x0b, flag = 0x0c, sdata = 0x0d,
		strp = 0x0e, udata = 0x0f, ref_addr = 0x10,
		ref1 = 0x11, ref2 = 0x12, ref4 = 0x13, ref8 = 0x14,
		ref_udata = 0x15, indirect = 0x16
	};

public enum DW_TAG {
		padding = 0x00, array_type = 0x01, class_type = 0x02,
		entry_point = 0x03, enumeration_type = 0x04, formal_parameter = 0x05,
		imported_declaration = 0x08, label = 0x0a, lexical_block = 0x0b,
		member = 0x0d, pointer_type = 0x0f, reference_type = 0x10,
		compile_unit = 0x11, string_type = 0x12, structure_type = 0x13,
		subroutine_type = 0x15, typedef = 0x16, union_type = 0x17,
		unspecified_parameters = 0x18, variant = 0x19, common_block = 0x1a,
		common_inclusion = 0x1b, inheritance = 0x1c, inlined_subroutine = 0x1d,
		module = 0x1e, ptr_to_member_type = 0x1f, set_type = 0x20,
		subrange_type = 0x21, with_stmt = 0x22, access_declaration = 0x23,
		base_type = 0x24, catch_block = 0x25, const_type = 0x26,
		constant = 0x27, enumerator = 0x28, file_type = 0x29,
		friend = 0x2a, namelist = 0x2b, namelist_item = 0x2c,
		packed_type = 0x2d, subprogram = 0x2e, template_type_param = 0x2f,
		template_value_param = 0x30, thrown_type = 0x31, try_block = 0x32,
		variant_part = 0x33, variable = 0x34, volatile_type = 0x35,
	};

public class ArmElfReader : IDisposable{
	protected ArmFileInfo af;
	protected string fileName;
	protected FileStream fs;
    protected long fsOrigin = 0;

	protected uint e_shoff;         // section header table offset
	protected ushort e_shentsize;	// size of one entry in section header table
	protected ushort e_shnum;		// number of entries in section header table
	protected ushort e_shstrndx;    // index of main string table
	protected byte[][] sectionHeaderTable;
	protected byte[][] sectionTable;  // string table or symbol table sections
	protected ST[][] symbolTable;   // symbol table indexed by section# and sym#
	protected int trace = 2;

	protected IDictionary<string,SyEntry> globalSymbols;  // symbols defined as externally visible
    protected IDictionary<string, SyEntry> localSymbols;  // symbols defined local to the file
    protected int codeSize = 0;
	protected int dataSize = 0;
	protected int roDataSize = 0;
    protected int roStrlDataSize = 0;
	protected SourceLineReader srcLineRdr = null;
    protected int heapSize = 32 * 1024;  // Kludge -- to be parameterized later!

	protected byte[] textSect = null;   // copy of .text section
	protected byte[] dataSect = null;   // copy of .data section
	protected byte[] rodataSect = null;   // copy of .rodata section
    protected byte[] roStrlDataSect = null;  // copy of .rodata.str1.4 section
	protected byte[] debugLineSect = null;   // copy of .debug_line section
	protected byte[] debugInfoSect = null;   // copy of .debug_info section
	protected byte[] debugAbbrevSect = null; // copy of .debug_abbrev section


	// accessor methods

	// The path to the compilation directory is needed for finding
	// the .s or .c source code file.
	// By inspection of the path, we can also determine whether the
	// object file was created by cygwin+gcc or by Windows + the
	// CodeSourcery version of gcc.
    public string CompilationDirectory { get; protected set; }

    public string SourceFileName { get; private set; }

    public int TextStart { get; private set; }

    public int DataStart { get; private set; }

    public int BssStart { get; private set; }

    public int DataEnd { get; private set; }

	// end of accessor methods

	public ArmElfReader( ArmFileInfo af )  {
		this.af = af;
		this.fileName = af.FileName;
        SourceFileName = null;
		openFile();
	}

    public ArmElfReader(ArmFileInfo af, FileStream fs)
    {
        this.af = af;
        this.fileName = af.FileName;
        SourceFileName = null;
        this.fs = fs;
        fsOrigin = fs.Position;
        openFile();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool d)
    {
        if (d) {
            //srcLineRdr.Dispose();
        }
    }

	// release all storage that is not needed after Pass 2
	public void CleanUp() {
		if (fs != null) {
            if (fsOrigin == 0) // it's not an archive
			    fs.Close();
			fs = null;
		}
		textSect = null;
		dataSect = null;
		rodataSect = null;
        roStrlDataSect = null;
		debugLineSect = null;
		debugInfoSect = null;
		debugAbbrevSect = null;
		sectionHeaderTable = null;
		sectionTable = null;
		symbolTable = null;
	}

	protected void openFile() {
		try {
            if (fs == null)
            {
                if (!File.Exists(fileName))
                    throw new AsmException("Object code file {0} not found", fileName);
                if (!fileName.EndsWith(".o"))
                    throw new AsmException(
                        "Object code file {0} does not have \".o\" suffix", fileName);
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                fsOrigin = 0;
            }
            string result = readHeader();
            if (result != null)
				throw new AsmException(
					"Object file {0} has invalid format: {1}", fileName, result);
		} catch( IOException e ) {
            if (fsOrigin == 0)
			    fs.Close();
			fs = null;
			throw new AsmException(
				"IO error reading file {0}:\n{1}", fileName, e);
		}
	}

	// In pass 1, we analyze the object code file just enough to
	// extract info about:
	// 1.  the sizes of the text, data and bss segments
	// 2.  symbols defined in the file which are externally visible
	// 3.  symbols referenced in the file which are not defined
	//     (and presumably are defined in another file)
	// 4.  comm symbols (which C compilers use to implement global
	//     variables).
	public void PerformPass1() {
		if (trace>1) Debug.WriteLine("Elf reader -- pass 1 started");
		this.globalSymbols = af.GlobalSymbols;
        this.localSymbols = af.LocalSymTable;
        int addr;

		// Make sure string tables are loaded into memory
		for( int i=0; i<e_shnum; i++ ) {
			byte[] htentry = sectionHeaderTable[i];
			SHT typeCode = (SHT)getInt32(htentry,4);
			if (typeCode != SHT.STRTAB) continue;
			if (!loadSectionTable((uint)i, SHT.STRTAB))
                throw new AsmException("ELF string table could not be loaded");
		}
		// Find sizes of text/data/bss code sections
		for( int i=0; i<e_shnum; i++ ) {
			byte[] htentry = sectionHeaderTable[i];
			int sectNameIx, size;
			string s;
			SHT typeCode = (SHT)getInt32(htentry,4);
			switch(typeCode) {
				case SHT.PROGBITS:
				case SHT.NOBITS:
                    addr = (int)getInt32(htentry,12);
					size = (int)getInt32(htentry,20);
					// is this the .text or .data or .bss segment ?
					SectionType st = SectionType.Unknown;
					sectNameIx = (int)getInt32(htentry,0);
					s = getString((uint)sectNameIx);
					switch(s) {
						case ".text":
							st = SectionType.Text;  break;
						case ".data":
							dataSize = size;
							st = SectionType.Data;  break;
						case ".rodata":
							roDataSize = (int)size;
							st = SectionType.Data;  break;
                        case ".rodata.str1.4":
                            roStrlDataSize = (int)size;
                            st = SectionType.Data; break;
						case ".bss":
							st = SectionType.Bss;   break;
						default:
							st = SectionType.None;  break;
					}
					if (trace>0)
						Debug.WriteLine(String.Format(" {0} segment, size = {1}, addr = {2}", s, size, addr));
					if (st != SectionType.None)
						af.SectionSize[(int)st] += (int)size;
					break;
                case SHT.SHT_ARM_ATTRIBUTES:
                    // some checking is needed
                    break;
				default:
					break;
			}
		}
		// process symbol tables
		for( int i=0; i<e_shnum; i++ ) {
			byte[] htentry = sectionHeaderTable[i];
			SHT typeCode = (SHT)getInt32(htentry,4);
			if (typeCode != SHT.SYMTAB) continue;
            if (!loadSectionTable((uint)i, SHT.SYMTAB))
                throw new AsmException("ELF symbol table could not be loaded");
			processSymTab((uint)i,htentry);
		}
	}

	// In pass 2, we do everything else which is needed.
	// The actions are:
	// 1.  copying text and data sections to the assembled program
	// 2.  applying relocation to locations in the assembled program
	// 3.  building a line number table, relating locations to
	//     source code lines, if the necessary debugging info has
	//     been included in the object code file.
	// Note that some of the debugging info is also subject to
	// relocation actions and therefore the order in which the
	// steps are performed has been chosen carefully.
	public void PerformPass2(int[] startAddresses) {
		if (trace>1) Debug.WriteLine("Elf reader -- pass 2 started");
		this.TextStart = startAddresses[(int)SectionType.Text];
		this.DataStart = startAddresses[(int)SectionType.Data];
		this.BssStart = startAddresses[(int)SectionType.Bss];
        this.DataEnd = af.ProgramSpace.DataEnd;

        fs.Seek(fsOrigin, SeekOrigin.Begin);

		// First, find all PROGBITS entries to read sections from the file
		// into memory ... these are all the entries which can be subjected
		// to relocation actions.
		for( int i=0; i<e_shnum; i++ ) {
			string name = getSectionName(i,SHT.PROGBITS);
            if (!loadSectionTable((uint)i, SHT.PROGBITS))
                continue;
			switch(name) {
				case ".text":
					textSect = sectionTable[i];  break;
				case ".data":
					dataSect = sectionTable[i];  break;
				case ".rodata":
					rodataSect = sectionTable[i];  break;
                case ".rodata.str1.4":
                    roStrlDataSect = sectionTable[i]; break;
				case ".debug_line":
					debugLineSect = sectionTable[i];  break;
				case ".debug_info":
					debugInfoSect = sectionTable[i];  break;
				case ".debug_abbrev":
					debugAbbrevSect = sectionTable[i]; break;
				default:
					break;
			}
		}
		// Second, apply relocation operations to the sections read above
		for( int i=0; i<e_shnum; i++ ) {
            string name = getSectionName(i, SHT.PROGBITS);
            byte[] htentry = sectionHeaderTable[i];
			bool abs;
			SHT typeCode = (SHT)getInt32(htentry,4);
			if (typeCode == SHT.RELA)
				abs = true;
			else if (typeCode == SHT.REL)
				abs = false;
			else
				continue;
			processRelocationTable(htentry,abs);
		}
		// Third, copy code sections into assembled program,
		// and build the line number table
		copyCodeSection(TextStart,textSect,".text");
		copyCodeSection(DataStart,rodataSect,".rodata");
        copyCodeSection(DataStart+roDataSize,roStrlDataSect,".rodata.str1.4");
		copyCodeSection(DataStart+roDataSize+roStrlDataSize,dataSect,".data");
		ProcessLineNumberInfo();
		processDebugInfo();
		// Finally, we can close the file and clean up
		CleanUp();
	}

	string getSectionName( int sectNum, SHT typeFilter ) {
		byte[] htentry = sectionHeaderTable[sectNum];
		SHT typeCode = (SHT)getInt32(htentry,4);
		if (typeFilter != SHT.NULL && typeCode != typeFilter)
			return null;
		uint nameix = getInt32(htentry,0);
		return getString(nameix);
	}

	protected void processSymbol( ST symbol ) {
		if (String.IsNullOrEmpty(symbol.name)) return;
		string s = "none";
		SectionType st = SectionType.Unknown;
		bool local = true;

		int offset = getSymbolValue(symbol, ref s, ref st, ref local);
        if (st == SectionType.None) // an external reference or a constant?
            return;

        if (local)
        {
            // it seems that we get funny Gnu-generated names like $d put into
            // the symbol table many times, so we will ignore duplicates that
            // begin with a $ symbol
            //if (localSymbols.TryGetValue(symbol.name, out sym))
            //{
            //    if (symbol.name.StartsWith("$")) return;
            //    throw new AsmException(
            //        "duplicate definition of local symbol {0}",
            //        symbol.name);
            //}
            localSymbols[symbol.name] = af.DefineSymbol(symbol.name, 0, offset, st);
            return;
        }

		// add the defined symbol into globals
		if (s == "common") {
			af.DefineCommSymbol(
				symbol.name, (int)symbol.size, (int)symbol.value);
			return;
		}

		globalSymbols[symbol.name] = af.DefineSymbol(symbol.name, 0, offset, st);
	}

	protected int getSymbolValue(
			ST symbol, ref string symbolKind, ref SectionType st, ref bool local ) 
	{
		string s = "none";
		st = SectionType.None;
		STB binding = (STB)((symbol.info >> 4) & 0x0f);
		if (binding == STB.WEAK) {
			Debug.WriteLine(String.Format(
				"weak binding for symbol ({0}) unsupported",symbol.name));
			symbolKind = s;
			return 0;
		}
		local = (binding != STB.GLOBAL);
		SHN sectNum = (SHN)symbol.shndx;
		if (sectNum == SHN.UNDEF)
			s = "external";
		else if ((uint)sectNum < (uint)SHN.LOPROC)  {
			byte[] htentry = sectionHeaderTable[(uint)sectNum];
			uint sectNameIx = getInt32(htentry,0);
			s = getString(sectNameIx);
        } else if (sectNum == SHN.ABS) {
            s = "absolute"; // for symbols with absolute addresses
		} else if (sectNum == SHN.COMMON)  {
			s = "common";  // for COMM symbols
		} else {
			Debug.WriteLine(String.Format(
				"Unhandled symbol type (shndx=0x{0:X})", (uint)sectNum));
			symbolKind = s;
			return 0;
		}
		if (trace>0)
			Debug.WriteLine(String.Format(
				" symbol {0}: kind = {1} value=0x{2:X} size=0x{3:X}",
				symbol.name, s, symbol.value, symbol.size));
		switch(s) {
			case ".text":
				st = SectionType.Text;  break;
			case ".data":
				st = SectionType.Data;  break;
			case ".rodata":
				st = SectionType.Data;  break;
            case ".rodata.str1.4":
                st = SectionType.Data;  break;
			case ".bss":
			case "common":
				st = SectionType.Bss;  break;
            case "absolute":
                st = SectionType.Abs;  break;
			case "external":
                af.DefineExternal(symbol.name);
                break;
			case "none":
			case ".debug_abbrev":
			case ".debug_line":
				break;
			default:
				Debug.WriteLine(String.Format(
					"Unsupported section type {0}", s));
				break;
		}
		symbolKind = s;
		int offset = (int)symbol.value;
        if (s == ".rodata.str1.4")
            offset += roDataSize;
		if (s == ".data")
			offset += roDataSize+roStrlDataSize;
		return offset;
	}

	protected string readHeader() {
		byte[] header = new byte[52];
		int len = fs.Read(header, 0, header.Length);
		if (len < header.Length) return "truncated header";
		if (getInt32(header,0) != 0x464c457f)
			return "bad magic number";	// bad magic number
		if (getInt16(header,4) != 0x0101)
			return "Not 32 bit LSB arch";	// not 32 bit arch, or not LSB format
		if (getInt16(header,16) != 0x0001)      // get e_type (1 = rel binary, 2 = executable)
			return "Not relocatable binary";	// not a relocatable binary file
		if (getInt16(header,18) != 0x0028)
			return "Not ARM object code";	// not ARM format
		if (getInt32(header,20) != 0x00000001)
			return "Not current version of format";	// not current version (1)
		e_shoff = getInt32(header,32);
		e_shentsize = getInt16(header,46);
		e_shnum = getInt16(header,48);
		e_shstrndx = getInt16(header,50);	// index of string table
		if (trace>1)
			Debug.WriteLine(String.Format(
				"* e_shoff=0x{0:X}, e_shentsize=0x{1:X}, e_shnum=0x{2}, e_shstrndx=0x{3}",
				e_shoff, e_shentsize, e_shnum, e_shstrndx));
		sectionHeaderTable = new byte[e_shnum][];
		sectionTable = new byte[e_shnum][];
		symbolTable = new ST[e_shnum][];
        fs.Seek((long)e_shoff + fsOrigin, SeekOrigin.Begin);
		for( int k=0;  k<e_shnum;  k++ ) {
			sectionHeaderTable[k] = new byte[e_shentsize];
			len = fs.Read(sectionHeaderTable[k], 0, (int)e_shentsize);
			if (len < e_shentsize) return "error reading section table";	// read error
		}
		return null;
	}

	// Gets a string from any section, given its offset.
	// On return, offset has advanced to the byte after the null terminator
	static protected string getString( ref int offset, byte[] section ) {
        if (section == null)
            throw new AsmException("unavailable object file section");
		StringBuilder sb = new StringBuilder();
		for( ; ; ) {
			byte b = section[offset++];
			if (b == 0) break;
			sb.Append((char)b);
		}
		return sb.ToString();
	}

	// gets a string from the main string table
	protected string getString( uint offset ) {
		int off = (int)offset;
		return getString(ref off, sectionTable[e_shstrndx]);
	}

    // returns False if a section of the desired type cannot be loaded
    // otherwise it is loaded, and True returned.
	protected bool loadSectionTable( uint whichSection, SHT whichKind ) {
		byte[] sectionBytes = sectionTable[whichSection];
		if (sectionBytes != null) return true;
		byte[] headerBytes = sectionHeaderTable[whichSection];
		uint type = getInt32(headerBytes,4) & 0xff;
        if ((SHT)type != whichKind) return false;
		uint sectionOffset = getInt32(headerBytes,16);
		uint sectionLength = getInt32(headerBytes,20);
		sectionBytes = new byte[sectionLength];
		sectionTable[whichSection] = sectionBytes;
        fs.Seek((long)sectionOffset + fsOrigin, SeekOrigin.Begin);
		int len = fs.Read(sectionBytes, 0, (int)sectionLength);
		if (len < sectionLength)
			throw new AsmException("read error (section 0x{0:X})", whichSection);
        return true;
	}

	// get little-endian 32 bit integer from a byte array
	static protected uint getInt32( byte[] arr, int offset ) {
		uint result = 0;
		for(int i=3;  i>=0;  i-- )
			result = (uint)((result<<8) + (arr[offset+i]&0xff));
		return result;
	}

	// get little-endian 16 bit integer from a byte array
	static protected ushort getInt16( byte[] arr, int offset ) {
		return (ushort)((arr[offset] & 0xff) + ((arr[offset+1]&0xff)<<8));
	}

	// store little-endian 32 byte integer into byte array
	static protected void putInt32( byte[] arr, int offset, uint val ) {
		for(int i=3;  i>=0;  i--) {
			arr[offset++] = (byte)(val & 0xff);
			val = (uint)(val >> 8);
		}
	}

	public void DumpHeaderTable() {
		if (!loadSectionTable(e_shstrndx, SHT.STRTAB))	// needed for getString() call
            throw new AsmException("ELF string table could not be loaded");
		for( int i=0; i<e_shnum; i++ ) {
			byte[] htentry = sectionHeaderTable[i];
			Console.WriteLine("    Header Table entry #{0}:", i);
			uint nameix = getInt32(htentry,0);
			string name = getString(nameix);
			uint typeix = getInt32(htentry,4);
			string typename = ArmElfReader.LookupType(typeix);
			uint addr = getInt32(htentry,12);
			uint offset = getInt32(htentry,16);
			uint size = getInt32(htentry,20);
			Debug.WriteLine(String.Format(
				"        name={0}, type={1}", name, typename));
			Debug.WriteLine(String.Format(
				"        addr=0x{0:X}, offset=0x{1:X}, size=0x{2:X}",
				addr, offset, size));
		}
		Console.WriteLine();
	}

	protected void processSymTab( uint sectNum, byte[] htentry )  {
		uint offset = getInt32(htentry,16);
		uint size = getInt32(htentry,20);
		uint shIndexStr = getInt32(htentry,24);
		if (!loadSectionTable(shIndexStr, SHT.STRTAB))
            throw new AsmException("ELF string table could not be loaded");
		byte[] syEntry = new byte[16];
		ST[] newSymTab =  new ST[size>>4];
		symbolTable[sectNum] = newSymTab;
		if (trace>1)
			Debug.WriteLine(String.Format(
				"Symbol Table Entries (section# 0x{0:X})", sectNum));
        fs.Seek((long)offset + fsOrigin, SeekOrigin.Begin);
		int symNum = 0;
		while( size > 0 )  {
			int len = fs.Read(syEntry, 0, 16);
			if (len < 16)
                throw new AsmException("Read error (symtab entry");	// read error
			int ix = (int)getInt32(syEntry,0);
			newSymTab[symNum].name = getString(ref ix,sectionTable[shIndexStr]);
			newSymTab[symNum].value = getInt32(syEntry,4);
			newSymTab[symNum].size = getInt32(syEntry,8);
			newSymTab[symNum].info = syEntry[12];
			newSymTab[symNum].other = syEntry[13];
			newSymTab[symNum].shndx = getInt16(syEntry,14);
			if (trace>1)
				Debug.WriteLine(String.Format(
					"  {0}: value=0x{1:X}, size=0x{2:X}, info=0x{3:X}, shndx=0x{4:X}",
					newSymTab[symNum].name, newSymTab[symNum].value, newSymTab[symNum].size,
					newSymTab[symNum].info, newSymTab[symNum].shndx));
			processSymbol(newSymTab[symNum]);
			symNum++;
			size -= 16;
		}
	}

	protected void copyCodeSection( int codePos, byte[] section, string sectName ) {
		if (section == null) {
			Debug.WriteLine(String.Format(
				"section not contained in obj file ({0})",sectName));
			return;
		}
		af.ProgramSpace.StoreMemory(codePos, section);
	}

	protected void processRelocationTable( byte[] htentry, bool abs ) {
		uint off = getInt32(htentry,16);
        // uint type = getInt32(htentry, 4);
		int size = (int)getInt32(htentry,20);
		uint symtabNum = getInt32(htentry,24);
		uint whichSect = getInt32(htentry,28);
		if (trace>0)
			Debug.WriteLine(String.Format(
				"Relocation Table Entries (symtab=0x{0:X}, section=0x{1:X})",
				symtabNum, whichSect));
		if (!loadSectionTable(whichSect, SHT.PROGBITS))
            return;
		string name = getSectionName((int)whichSect,SHT.PROGBITS);
		if (name == null) return;
		byte[] targetSection = sectionTable[whichSect];
		int sectStart = 0;
		switch(name) {
			case ".text":
				sectStart = TextStart;
				break;
			case ".data":
				sectStart = DataStart + roDataSize + roStrlDataSize;
				break;
			case ".rodata":
				sectStart = DataStart;
				break;
            case ".rodata.str1.4":
                sectStart = DataStart + roDataSize;
                break;
			case ".debug_line":
			case ".debug_info":
			case ".debug_abbrev":
				break;
			default:
				Debug.WriteLine(String.Format(
					"  relocation for {0} ignored", name));
				return;
		}
		int relEntryLen = abs? 12 : 8;
		byte[] relEntry = new byte[relEntryLen];
        fs.Seek((long)off + fsOrigin, SeekOrigin.Begin);
		while( size > 0 ) {
			int len = fs.Read(relEntry, 0, relEntryLen);
			if (len < relEntryLen)
                throw new AsmException("Read error (rel entry)");	// read error
			int offset = (int)getInt32(relEntry,0);
			uint info   = getInt32(relEntry,4);
			uint addend = abs? getInt32(relEntry,8) : 0;
			size -= relEntryLen;
			uint sym  = info>>8;
			uint rtype = info&0xFF;
			ST symbol = symbolTable[symtabNum][sym];
			if (trace>0)
				Debug.WriteLine(String.Format(
					"  offset=0x{0:X}, sym#=0x{1:X}, symName={2}, type#={3}, addend=0x{4:X}",
					offset, sym, symbol.name, rtype, addend));
			performRelocation((uint)(sectStart+offset),offset,targetSection,addend,symbol,rtype);
		}
	}

	// apply relocation to the in-memory copy of the code section
	//    addr:    the run-time address of the location being relocated
	//    offset:  the offset within the object code section of the
	//             location being relocated
	//    section: an in-memory copy of the object code section
	//    addend:  a constant to add to the value of the relocation symbol
	//    symbol:  the relocation symbol whose value is to be added to
	//             the location specified by addr and offset.
	//    type:    the type of relocation to be performed, as specified
	//             in the Elf documentation (and as discovered by
	//             inspection of Arm object code files).
	protected void performRelocation(
			uint addr, int offset, byte[] section, uint addend, ST symbol, uint type ) {
		uint word = getInt32(section,offset);
		uint newWord = 0;
		string name = symbol.name;
		int val;	// value of the symbol
		string symbolKind = "??";
		SectionType st = SectionType.Unknown;
		if (!String.IsNullOrEmpty(name)) {
            bool isNewUnknown = false;
			SyEntry sym = af.LookupSymbol(name, out isNewUnknown);
			if (sym == null) {
                if (name == "__cs3_heap_start" || name == "_end")
                {   // This is a temporary fixup; the proper solution would involve
                    // reading  loader script which defined __cs3_heap_start = _end.
                    val = DataEnd;
                    //af.DefineSymbol(name, -1, val-bssStart, SectionType.Bss);
                }
                //else if (name == "__cs3_heap_limit")
                //    val = dataEnd + heapSize;
                else
                {
                    if (isNewUnknown)
                        af.ParseError("reference to undefined external symbol {0}", name);
                    return;
                }
			} else
			    val = sym.SymValue;
		} else {
			bool local = true;
			val = getSymbolValue(symbol, ref symbolKind, ref st, ref local);
			switch(st) {
				case SectionType.Text:
					val += TextStart;  break;
				case SectionType.Data:
					val += DataStart;  break;
				case SectionType.Bss:
					val += BssStart;  break;
				default:
					Debug.WriteLine(String.Format(
						"- anon symbol, kind={0}, section type {1} ignored",
						symbolKind, st.ToString()));
					return;
			}
		}
		uint opnd;
		bool supported = true;
		switch(type) {
			case 0:
				return;
			case 2:			// R_ARM_ABS32
				newWord = (uint)(word + val + addend);
				break;
            case 1:			// R_ARM_PC24   (used by gcc from Australia)
            case 27:		// R_ARM_PLT32  (deprecated but used by CodeSourcery)
			case 28:		// R_ARM_CALL   (used by CodeSourcery)
			case 29:		// R_ARM_JUMP24 (used by CodeSourcery)
				opnd = (uint)((word & 0x00ffffff) << 2);
                if ((opnd & 0x02000000) != 0)  // sign extend?
                    opnd |= 0xFC000000;
				opnd += (uint)(val+addend-addr);
				opnd = (uint)((opnd >> 2) & 0xFFFFFF);
				newWord = (uint)((word & 0xFF000000) | opnd);
				break;
			
			default:
				supported = false;
				break;
		}
		if (!supported || trace>0)
			Debug.WriteLine(String.Format(
				"  Relocation: old=0x{0:X}, new=0x{1:X}, val=0x{2:X}, add=0x{3:X}, type={4}",
				word, newWord, val, addend, type));
		if (!supported)
			throw new AsmException("unhandled relocation type: {0}", type);
		putInt32(section,offset,newWord);
	}

	// This method makes a copy of the line number information section
	// in an Elf object code file, where the info is in Dwarf2 format.
	// Specs for Dwarf2 taken from www.arm.com/pdfs/TIS-DWARF2.pdf
    protected SourceLineReader ProcessLineNumberInfo() {
		if (debugLineSect == null) return null;
		srcLineRdr = new SourceLineReader(debugLineSect,fileName);
        return srcLineRdr;
	}

	protected void processDebugInfo() {
		if (debugInfoSect == null || debugAbbrevSect == null)
			return;
		uint length = getInt32(debugInfoSect,0);
		uint version = getInt16(debugInfoSect,4);
		if (version != 2) {
			Debug.WriteLine("Not a Dwarf2 object file");
			return;
		}
		int abbrevOffset = (int)getInt32(debugInfoSect,6);
		if (debugInfoSect[10] != 4)  {
			Debug.WriteLine(String.Format(
				"Address size is {0}, cannot process debug info", debugInfoSect[10]));
			return;
		}
		List<int> abbreviationSectionTable = buildAbbreviationTable(abbrevOffset);
		int ix = 11;  // index of first debug info entry
		while(ix < length) {
			processDebugInfoEntry(ref ix,abbreviationSectionTable);
		}
		// this next assignment is the whole reason for processing
		// the attributes information!!!
		if (srcLineRdr != null)
			srcLineRdr.SetCompDirectory(CompilationDirectory);
	}

	protected List<int> buildAbbreviationTable( int abix ) {
		List<int> result = new List<int>(10);
		result.Add(-1);			// initialize result[0]
		int maxOffset = debugAbbrevSect.Length;
		// skip through abbreviation table to find the definitions
		// with that abbreviation code
		while(abix < maxOffset) {
			uint abbreviationSection = getLEB128(ref abix,debugAbbrevSect);
			while(result.Count <= abbreviationSection) {
				result.Add(-1);
			}
			result[(int)abbreviationSection] = abix;
			if (trace>1)
				Debug.WriteLine(String.Format(
					" -- Abbrev section {0} at index = {1}", abbreviationSection, abix));
			abix += 2;	// skip over DW_TAG and DW_CHILDREN codes
			while(abix < maxOffset && debugAbbrevSect[abix] != 0) {
				abix += 2;  // skip over DW_AT and DW_FORM codes
			}
			abix += 2;  // skip over the null byte pair
		}
		return result;
	}

	protected void processDebugInfoEntry( ref int ix, List<int> abbreviationSectionTable ) {
		// get abbreviation code number from the debug_info section
		uint abbreviationCode = getLEB128(ref ix,debugInfoSect);
		if (abbreviationCode == 0) return;
		int abix = -1;
		if (abbreviationCode < abbreviationSectionTable.Count)
			abix = abbreviationSectionTable[(int)abbreviationCode];
		if (abix < 0) {
			Debug.WriteLine(String.Format(
				"could not locate abbreviation code {0}",abbreviationCode));
			ix += 1000;  // action to force termination of caller's loop
			return;
		}
        DW_TAG tag = (DW_TAG)debugAbbrevSect[abix++]; // do not delete
        bool hasChildren = (debugAbbrevSect[abix++]) != 0; // do not delete
		while(debugAbbrevSect[abix] != 0) {
			DW_AT attribute = (DW_AT)debugAbbrevSect[abix++];
			DW_FORM f = (DW_FORM)debugAbbrevSect[abix++];
			int len;
			string s;
			uint dummy;
			for( ; ; ) {
				switch(f) {
					case DW_FORM.addr:
						ix += 4;
						break;
					case DW_FORM.block:
						len = (int)getLEB128(ref ix,debugInfoSect);
						ix += len;
						break;
					case DW_FORM.block1:
						len = (int)(debugInfoSect[ix++] & 0xff);
						ix += len;
						break;
					case DW_FORM.block2:
						len = (int)getInt16(debugInfoSect,ix);
						ix += len+2;
						break;
					case DW_FORM.block4:
						len = (int)getInt32(debugInfoSect,ix);
						ix += len+4;
						break;
					case DW_FORM.data1:
					case DW_FORM.ref1:
					case DW_FORM.flag:
						ix += 1;
						break;
					case DW_FORM.data2:
					case DW_FORM.ref2:
						ix += 2;
						break;
					case DW_FORM.data4:
					case DW_FORM.ref4:
						ix += 4;
						break;
					case DW_FORM.data8:
					case DW_FORM.ref8:
						ix += 8;
						break;
					case DW_FORM.dwstring:
						s = getString(ref ix,debugInfoSect);
						if (attribute == DW_AT.comp_dir)
							CompilationDirectory = s;
						if (attribute == DW_AT.name)
							SourceFileName = s;
						if (trace>1)
							Debug.WriteLine(String.Format(
								" -- Attribute {0} = {1}", attribute.ToString(), s));
						break;
					case DW_FORM.sdata:
					case DW_FORM.udata:
					case DW_FORM.ref_udata:
						dummy = getLEB128(ref ix,debugInfoSect); // do not delete
						break;
					case DW_FORM.strp:
						// skip over offset into .debug_str section
						ix += 4;
						break;
					case DW_FORM.ref_addr:
						ix += 4;
						break;
					case DW_FORM.indirect:
						f = (DW_FORM)getLEB128(ref ix,debugInfoSect);
						continue;
					default:
						Debug.WriteLine(String.Format(
							"unimplemented DW_FORM tag (value 0x{0:X})",f));
						return;
				}
				break;
			}
		}
	}

	// Decode variable length number in 'Little endian base 128' format
	static protected uint getLEB128( ref int ix, byte[] section ) {
		uint result = 0;
		int shift = 0;
		for( ; ; ) {
			uint b = (uint)section[ix++];
			result |= (uint)((b & 0x7f) << shift);
			if ((b & 0x80) == 0) break;
			shift += 7;
		}
		return result;
	}

	public static string LookupType(uint typecode) {
		string name = Enum.GetName(typeof(SHT),typecode);
		if (name==null) name = String.Format("type#{0:X}", typecode);
		return name;
	}

	public static string LookupType(SHT typecode)  {
		return LookupType((uint)typecode);
	}

}
}