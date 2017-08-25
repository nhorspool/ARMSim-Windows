// File AssembledProgram.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005 - October 2007
//

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ArmAssembly {

// This class provides a memory image of the assembled program.
// When instantiated, the start address for the memory image is
// specified.  Methods for loading and storing 1, 2 or 4 byte
// values into the memory image are provided.

public class AssembledProgram {
	static string hexchar = "0123456789ABCDEF";
	int numBytes;
	int numWords;
	uint[] memory;

	public AssembledProgram( int startAddress, int length ) {
		this.StartAddress = startAddress;
		numBytes = length;
		numWords = (numBytes + 3) >> 2;
		memory = new uint[numWords];
	}

	public AssembledProgram( ArmAssembler ar ):
		this(ar.LoadPoint, ar.Length)
	{ }

    public int StartAddress { get; private set; }

    public int EndAddress { get { return StartAddress + numBytes; } }

	public uint[] Memory { get{ return memory; } }

    public int TextStart { get; set; }

    public int DataStart {get; set; }

    public int DataEnd { get; set; }

    public int BssStart { get; set; }

	public uint LoadWord( int addr ) {
        var off = convertAddrToWordOffset(addr);
        // Displaying a memory view can cause accesses to memory
        // locations out of range if a new program is loaded
        if (off > memory.Length) return 0x81818181;
		return memory[off];
	}

	public void StoreWord( int addr, uint value ) {
		memory[convertAddrToWordOffset(addr)] = value;
	}

	public uint LoadHalfword( int addr ) {
		int offset = convertAddrToByteOffset(addr);
		uint value = memory[offset >> 2];
		switch(addr & 0x3) {
		case 0x2:	value >>= 16;  break;
		case 0x0:	break;
		default:	badAlignment(addr);  break;
		}
		return value &= 0x0000FFFF;
	}

	public void StoreHalfword( int addr, uint value ) {
		value &= 0xFFFF;
		int offset = convertAddrToByteOffset(addr);
		uint mask = 0;
		switch(offset & 0x3) {
		case 0x0:	mask = 0xFFFF0000;  break;
		case 0x2:	mask = 0x0000FFFF;  value <<= 16;   break;
		default:	badAlignment(addr);  break;
		}
		offset >>= 2;
		memory[offset] = (memory[offset] & mask) | (uint)value;
	}

	public uint LoadByte( int addr ) {
		int offset = convertAddrToByteOffset(addr);
		uint value = memory[offset >> 2];
		switch(addr & 0x3) {
		case 0x0:	break;
		case 0x1:	value >>= 8;   break;
		case 0x2:	value >>= 16;  break;
		case 0x3:	value >>= 24;  break;
		}
		return value & 0xFF;
	}

	public void StoreByte( int addr, int value ) {
		value &= 0xFF;
		int offset = convertAddrToByteOffset(addr);
		uint mask = 0;
		switch(offset & 0x3) {
		case 0x0:	mask = 0xFFFFFF00;  break;
		case 0x1:	mask = 0xFFFF00FF;  value <<= 8;   break;
		case 0x2:	mask = 0xFF00FFFF;  value <<= 16;  break;
		case 0x3:	mask = 0x00FFFFFF;  value <<= 24;  break;
		}
		offset >>= 2;
		memory[offset] = (memory[offset] & mask) | (uint)value;
	}

	public void StoreMemory( int addr, byte[] mem ) {
		int len = mem.Length;
		int i=0;
		// prelude to handle unaligned addr
		while((addr & 0x3) != 0 && i<len) {
			StoreByte(addr++,mem[i++]);
		}
		// now handle groups of 4 bytes
		// (these are already in little-endian order)
		int offset = convertAddrToWordOffset(addr);
		int len4 = i + (int)((len-i) & 0xfffffffc);
		while(i < len4) {
			uint value = (uint)((mem[i+3]<<24) | (mem[i+2]<<16) | (mem[i+1]<<8) | mem[i]);
			memory[offset++] = value;
			i += 4;
			addr += 4;
		}
		// postlude to handle excess trailing bytes
		while(i < len) {
			StoreByte(addr++,mem[i++]);
		}
	}

	int convertAddrToWordOffset( int addr ) {
        int offset = (addr - StartAddress);
		if (offset < 0 || offset > numBytes)
			addressOutOfRange(addr);
		if ((offset & 0x3) != 0)
			badAlignment(addr);
		return offset>>2;
	}

	int convertAddrToByteOffset( int addr ) {
        int offset = (addr - StartAddress);
		if (offset < 0 || offset > numBytes)
			addressOutOfRange(addr);
		return offset;
	}

	static void addressOutOfRange( int addr ) {
        throw new ARMSimException(String.Format(
				"address {0} out of range", addr));
	}

	static void badAlignment( int addr ) {
        throw new ARMSimException(String.Format(
				"non-aligned access to memory: address {0}", addr));
	}

	public void Hexdump( int startAddress, int nBytes ) {
		// we will display 32 bytes per line, so start
		// at an address divisible by 32
		int addr = (int)((uint)startAddress & 0xFFFFFFE0);
		int endAddr = startAddress + nBytes;
		do {
			StringBuilder byteLine = new StringBuilder();
			StringBuilder wordLine = new StringBuilder();
			Console.Write("{0,6:X}:", addr);
			for( int k = 0;  k < 32;  k++ ) {
				if (addr >= endAddr)
					break;
				if ((addr & 0x00000003) == 0) {
					if (addr < startAddress) {
						wordLine.Append("         ");
					} else {
						uint wordVal = LoadWord(addr);
						wordLine.Append(string.Format(" {0,8:X}",wordVal));
					}
					byteLine.Append(' ');
				}
				if (addr < startAddress) {
					byteLine.Append(' ');
					byteLine.Append(' ');
				} else {
					int val = (int)(LoadByte(addr));
					byteLine.Append(hexchar[val >> 4]);
					byteLine.Append(hexchar[val & 0x0F]);
				}
				addr++;
			}
			Console.WriteLine(byteLine.ToString());
			Console.WriteLine("       {0}",wordLine.ToString());
		} while(addr < endAddr);
	}
	
	public void Hexdump() {
        Hexdump(StartAddress, numBytes);
	}

} //class AssembledProgram


}  // end of namespace ArmAssembly
