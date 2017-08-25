// File ArmFileInfo.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005 - November 2007

using System;
using System.IO;
using System.Collections.Generic;

namespace ArmAssembly {

// SectionType defines the type of an identifier (normally a label) or expression
// * Text, Data, Bss describe a label or address within the corresponding region
// * None describes a pure number
// * Unknown is used for a label which has not yet been seen; it is OK to have
//   such labels during Pass 1; after pass 1, it is an error.
// * Reg is used for an identifier that matches a register name -- attempting to use
//   such a value as a number or a label is invalid
// * Abs is used for an absolute memory location
// Some arithmetic operations involving these values are valid.
    public enum SectionType { Text = 0, Data = 1, Bss = 2, Abs = 3, None = 4, Unknown = 5, Reg = 6 };

public enum CodeType { TextArm = 0, TextThumb = 1, LtorgData = 2, Data = 3 };

// one instance of this type is created per source code line to record
// what the source line translates to.
public struct SourceLineInfo {
    public int NumBytes;
    public int Address;
    public CodeType Content;
    public SourceLineInfo(int n, CodeType ct, int a)
    {
        NumBytes = n; Content = ct; Address = a;
    }
}

public abstract class ArmFileInfo {
	public bool trace = false;

	static string lastFileNamePrinted = null;

    /*
	public struct NameInt {
		public string name;
		public int val;
		public NameInt( string s, int v ) {
			name = s;  val = v;
		}
	}
    */

	protected ArmFileInfo( string fileName,
            IDictionary<string,SyEntry> globalSymbols, IDictionary<string,string> externSymbols ) {
		this.FileName = fileName;
		this.GlobalSymbols = globalSymbols;
        this.ExternSymbols = externSymbols;
        this.UndefLocals = new List<string>();
		LocalSymTable = new Dictionary<string,SyEntry>();
		SectionSize = new int[3];
		SectionAddress = new int[5];
        for (int i = 0; i < SectionAddress.Length; i++) SectionAddress[i] = 0;
        SourceLine = null;
		Pass = 0;
	}

	// accessor methods

    public int Pass { get; set; }

    public string FileName { get; private set; }

    public IList<string> SourceLine { get; set; }     // source code lines (if available)

    public int[] SectionSize { get; private set; }  // contains info only after Pass 1

    public int[] SectionAddress { get; private set; }  // contains info only after start of Pass 2

    public IDictionary<string, SyEntry> LocalSymTable { get; set; }

    public IDictionary<string, SyEntry> GlobalSymbols { get; private set; }

    public IDictionary<string, string> ExternSymbols { get; private set; }

    protected IList<string> UndefLocals { get; private set; }

    public AssembledProgram ProgramSpace { get; set; }

	// end of accessor methods

    public void DefineExternal(string name)
    {
        if (GlobalSymbols.ContainsKey(name)) return;
        if (ExternSymbols.ContainsKey(name)) return;
        ExternSymbols[name] = FileName;
    }

	public SyEntry DefineSymbol( string name, int lineNum, int value, SectionType section ) {
		SyEntry sym;
		if (Pass > 1 && lineNum >= 0) {
			if (!LocalSymTable.TryGetValue(name, out sym))
				throw new AsmException("symbol {0} not found", name);
			sym.SymValue = value;
			return sym;
		}
		if (section != SectionType.None) {
			sym = new SyEntry(name, lineNum, section, 0, value);
		} else
			sym = new SyEntry(name, lineNum, value);
		LocalSymTable[name] = sym;
		return sym;
	}

	public virtual SyEntry LookupNumericLabel( string name )  {
		// overridden in AsmFileInfo subclass
		return null;
	}

	public SyEntry LookupSymbol( string name, out bool isNewUnknown ) {
        isNewUnknown = false;
		if (Char.IsDigit(name[0]))
			return LookupNumericLabel(name);
		SyEntry sy = null;
		if (LocalSymTable.TryGetValue(name,out sy))
			return sy;
        if (GlobalSymbols.TryGetValue(name, out sy))
            return sy;
        if (!UndefLocals.Contains(name))
        {
            UndefLocals.Add(name);
            isNewUnknown = true;
        }
		return null;
	}

	public virtual void DefineCommSymbol( string name, int size, int align ) {
		if (Pass > 1) return;
		if (LocalSymTable.ContainsKey(name)) return;
		if (size <= 0) size = 1;
		// round up alignment to a power of 2
		int al = 1;
		while(al < align) {
			al <<= 1;
		}
		align = al;
		SyEntry newsym;
		if (!GlobalSymbols.TryGetValue(name,out newsym)) {
			newsym = new SyEntry(name,0,size,align);
			GlobalSymbols[name] = newsym;
		} else if (newsym.Kind == SymbolKind.CommSymbol) {
			// combine new size and alignment with old values
			newsym.Size = size;
			newsym.Align = align;
		} // else ignore the .comm definition because it's
		  // already defined as a normal label
		LocalSymTable[name] = newsym;
	}

	public abstract void StartPass();

	public void EndPass() {
		if (Pass == 1)
			endPass1();
		else
			endPass2();
	}

	protected abstract void endPass1();

	protected abstract void endPass2();

    public void ParseError(string msg, params object[] args) {
        if (lastFileNamePrinted != FileName) {
            lastFileNamePrinted = FileName;
            Console.WriteLine("File {0}:", lastFileNamePrinted);
        }
        AssemblerErrors.AddError(FileName, String.Format(msg, args));
    }

}



public enum SymbolKind { Label, Symbol, CommSymbol };


public class SyEntry {
	int align;	// if sykind=CommSymbol, alignment in bytes 

	// Create a Label form of symbol
	public SyEntry(string name, int ln, SectionType section,
			int subsection, int pos) {
		Name = name;
        LineNumber = ln;
		Kind = SymbolKind.Label;	// label on text/data/bss loc
		Section = section;
		Subsection = subsection;
        SymValue = pos;
	}

	// Create a .COMM form of symbol
    public SyEntry(string name, int ln, int size, int align) {
        Name = name;
        LineNumber = ln;
		Kind = SymbolKind.CommSymbol;
		Section = SectionType.Bss;
		Subsection = 0;
        SymValue = size;
		this.align = align;
    }

	public SyEntry(string name, int ln, int val) {
        Name = name;
        LineNumber = ln;
		Kind = SymbolKind.Symbol;	// symbol
		Section = SectionType.None;
        SymValue = val;
	}

    // if Kind=Label, pos relative to subsection start
    // if Kind=CommSymbol, size in bytes
    public int SymValue
    {
        get;
        set;
    }

    public string Name { get; private set; }

    public SectionType Section
    {
        get;
        set;
    }

    public int Subsection // number in range 0 to 8191
    {
        get;
        set;
    }

    public SymbolKind Kind
    {
        get;
        set;
    }

    public int LineNumber { get; private set; }

	public int Size {
		get { return Kind==SymbolKind.CommSymbol? SymValue : 0; }
		set {
            if (Kind == SymbolKind.CommSymbol) {
                if (value > SymValue)
                    SymValue = value;
            }  else
                throw new AsmException("attempt to set size of non-comm symbol");
			}
	}
	
	public int Align {
		get { return Kind==SymbolKind.CommSymbol? align : 0; }
		set {
			if (Kind==SymbolKind.CommSymbol) {
                if (value > align)
				    align = value;
            } else
                throw new AsmException("attempt to set alignment of non-comm symbol");
			}
	}
	
	public override string ToString() {
		if (Kind == SymbolKind.Label)
			return String.Format("Label {0} [line {4}]: pos {1} in {2} {3}",
                Name, SymValue, Section, Subsection, LineNumber);
		else if (Kind==SymbolKind.CommSymbol)
			return String.Format("Comm Symbol {0} [line {3}]: size={1}, align={2}",
                Name, SymValue, align, LineNumber);
		else 
			return String.Format("Symbol {0} [line {2}] = {1}",
                Name, SymValue, LineNumber);
	}
}

}  // end of namespace ArmAssembly

