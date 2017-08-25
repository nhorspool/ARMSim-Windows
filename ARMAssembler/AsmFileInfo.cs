// File AsmFileInfo.cs
//
// Copyright (c) R. Nigel Horspool,  May 2011

using System;
using System.IO;
using System.Collections.Generic;

namespace ArmAssembly {

    // This class is only instantiated if the assembler file contains errors.
    // The object simply provides access to a clean copy of the source file.
public class AsmFileInfo : ArmFileInfo {
	 
	public AsmFileInfo( string fileName,
                IDictionary<string,SyEntry> globalSymbols, IDictionary<string,string> externSymbols)
        : base(fileName, globalSymbols, externSymbols)
    {
        readSourceLines();
	}


    private void readSourceLines()
    {
        SourceLine = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(FileName))
            {
                for (; ; )
                {
                    string s = sr.ReadLine();
                    if (s == null) break;
                    SourceLine.Add(s);
                }
            }
        }
        catch (IOException e)
        {
            throw new AsmException("I/O error when reading file {0}: {1}", FileName, e);
        }
    }

    public override void StartPass()
    {
        throw new ARMSimException("unexpected call to Startpass()");
    }

    protected override void endPass1()
    {
        throw new ARMSimException("unexpected call to endPass1()");
    }

    protected override void endPass2()
    {
        throw new ARMSimException("unexpected call to endPass2()");
    }

}

}  // end of namespace ArmAssembly

