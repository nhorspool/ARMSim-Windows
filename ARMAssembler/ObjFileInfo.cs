// File ObjFileInfo.cs
//
// Copyright (c) R. Nigel Horspool,  April 2006 - May 2011

using System;
using System.IO;
using System.Collections.Generic;

namespace ArmAssembly {


public class ObjFileInfo : ArmFileInfo {

	public ObjFileInfo( string fileName,
                IDictionary<string,SyEntry> globalSymbols, IDictionary<string,string> externSymbols )
        : base(fileName, globalSymbols, externSymbols)
    {
		Elf = new ArmElfReader(this);
	}

    public ObjFileInfo(FileStream arMember, string libraryName, string member,
            IDictionary<string, SyEntry> globalSymbols, IDictionary<string, string> externSymbols)
        : base(String.Format("{0}({1})", libraryName, member),
            globalSymbols, externSymbols)
    {
        Elf = new ArmElfReader(this, arMember);
    }

	// accessor methods

    public ArmElfReader Elf { get; private set; }

	// end of accessor methods

	public override void StartPass() {
		if (Pass == 1) {
			Elf.PerformPass1();
		} else if (Pass == 2) {
			Elf.PerformPass2(this.SectionAddress);
		}
	}

	protected override void endPass1() { }

	protected override void endPass2() { }

}

}  // end of namespace ArmAssembly

