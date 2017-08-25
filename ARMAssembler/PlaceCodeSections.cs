// File PlaceCodeSections.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005 - August 2007


using System;
using System.IO;
using System.Collections.Generic;

namespace ArmAssembly {

// After pass 1 of the assembly process has been performed on all the
// source files, we know the offsets of all labels relative to the
// start of each section.
// We now consolidate all these pieces, determining their placements
// at absolute memory locations.  The offsets of all labels are updated
// to be absolute memory locations.  (This is work that would normally
// be performed by a loader.)
//
// After this step, the label addresses follow a pattern as shown in
// the diagram below.  All the text sections are consolidated, all
// the data sections are consolidated, as are all the bss sections.
//
//        program load point  -->  text section for file 1
//                                 text section for file 2
//                                 ...
//                                 data section for file 1
//                                 data section for file 2
//                                 ...
//                                 bss section for file 1
//                                 bss section for file 2
//                                 ...
// The program load point is an address which may be supplied as a
// parameter to the consolidation process.  If omitted, a default
// value of 1000 (hex) is used.
//
public class PlaceCodeSections {
	static int defaultLoadPoint = 0x1000;
	private int nextFreeAddress;
    IList<ArmFileInfo> fileInfoList;
	IDictionary<string,SyEntry> globalSymbols;

	public PlaceCodeSections( IList<ArmFileInfo> fileInfoList, IDictionary<string,SyEntry> globalSymbols, int startAddress ) {
        LoadPoint = startAddress;
		this.fileInfoList = fileInfoList;
		this.globalSymbols = globalSymbols;
		performPlacement();
		processCommEntries();
	}

    public PlaceCodeSections(IList<ArmFileInfo> fileInfoList, IDictionary<string, SyEntry> globalSymbols)
		: this(fileInfoList,globalSymbols,defaultLoadPoint) { }

	// accessor methods

    public int LoadPoint { get; private set; }

    public int Length { get { return nextFreeAddress - LoadPoint; } }

    public int TextStart { get; private set; }

    public int DataStart { get; private set; }

    public int BssStart { get; private set; }

    public int DataEnd { get; private set; }

	// placement for all sections in all files
	private void performPlacement() {
        LoadPoint = (int)(((uint)LoadPoint + 3) & 0xFFFFFFFC);
        nextFreeAddress = LoadPoint;
        TextStart = nextFreeAddress;
		performPlacement(SectionType.Text);
		DataStart = nextFreeAddress;
		performPlacement(SectionType.Data);
		BssStart = nextFreeAddress;
		performPlacement(SectionType.Bss);
        DataEnd = nextFreeAddress;
	}

	// placement for one main section type in all files
	private void performPlacement( SectionType st ) {
		foreach( ArmFileInfo fileInfo in fileInfoList )
			performPlacement(st,fileInfo);
	}
	
	// placement for one main section type in one file
	private void performPlacement( SectionType st, ArmFileInfo fileInfo ) {
		fileInfo.SectionAddress[(int)st] = nextFreeAddress;
		int size = fileInfo.SectionSize[(int)st];
		// place the labels in this section only
		foreach( SyEntry sy in fileInfo.LocalSymTable.Values ) {
			if (sy.Kind != SymbolKind.Label) continue;
			if (sy.Section != st) continue;
			if (sy.Subsection != 0)
				throw new AsmException("internal error in performPlacement, 2");
			sy.SymValue += nextFreeAddress;
		}
		nextFreeAddress += size;
		nextFreeAddress = (int)(((uint)nextFreeAddress + 3) & 0xFFFFFFFC);
	}

	// all remaining COMM entries after pass 1 are converted to BSS labels
	private void processCommEntries() {
		foreach( SyEntry sy in globalSymbols.Values ) {
			if (sy.Kind != SymbolKind.CommSymbol) continue;
            uint mask = (uint)(-sy.Align);
			int addr = (int)(((uint)nextFreeAddress + (sy.Align-1)) & mask);
			nextFreeAddress = addr + sy.Size;
			sy.Kind = SymbolKind.Label;
			sy.Section = SectionType.Bss;
			sy.Subsection = 0;
			sy.SymValue = addr;
		}
		nextFreeAddress = (int)(((uint)nextFreeAddress + 3) & 0xFFFFFFFC);
	}
}

} // end of namespace ArmAssembly