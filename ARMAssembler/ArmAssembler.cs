// File ArmAssembly.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005 - May 2015


using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace ArmAssembly {

// This class controls the steps of the assembling and linking process.
// The sequence of steps is ...
//   0. ArmAssembler(.)		-- create an instance, specifying file names
//   1. PerformPass()		-- to perform pass 1 on all files
//   2. PlaceCode(.)		-- to determine the memory layout
//   3. PerformPass(.)		-- to perform pass 2 on all files
public class ArmAssembler {
	private IList<string> fileList;
	private IDictionary<string,SyEntry> globalSymbols =
				new Dictionary<string,SyEntry>();  // entries = SyEntry or CommEntry
    private IDictionary<string, string> externSymbols =
                new Dictionary<string, string>();
	private int pass = 0;
	private bool[] passComplete = new bool[3];
	private PlaceCodeSections pcs;
	private AssembledProgram ap;
    private string tempFolder;
    private string assemblerProg = null;
    private int errorCnt = 0;
    private string fileName = "";
    private int listingState = 0;
    private IList<string> listing;
    private IList<string> symTabListing;

    // accessor methods

    public IList<ArmFileInfo> FileInfoTable { get; protected set; }
    public int LoadPoint { get { return pcs.LoadPoint; } }
    public int Length { get { return pcs.Length; } }


	// general version of constructor which handles several files
	public ArmAssembler( string[] fileList ) {
		if (fileList == null)
            throw new ARMSimException("no source files provided");
		this.fileList = new List<string>(fileList);
		initialize();
	}

	// another version of constructor which handles several files
	public ArmAssembler( IList<string> fileList ) {
		if (fileList == null)
            throw new ARMSimException("no source files provided");
		this.fileList = fileList;
		initialize();
	}

	// version of constructor when there is only one file
	public ArmAssembler( string fileName ) {
		fileList = new List<string>(1);
		fileList[0] = fileName;
		initialize();
	}

    // methods

	private void initialize() {
		// ArmInstructionTemplate.ForceInitialization();
        AssemblerErrors.Initialize();
        FileInfoTable = new List<ArmFileInfo>();
        for (int i = 0; i < fileList.Count; i++)
            FileInfoTable.Add(null);
		for( int i=0; i<passComplete.Length; i++)
			passComplete[i] = false;
        //tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        tempFolder = System.IO.Path.GetTempPath();
        listing = new List<string>(128);
        symTabListing = new List<string>(32);
        assemblerProg = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

		string assemblerExecutableName;
		if (ARMSim.ARMSimUtil.RunningOnWindows)
			assemblerExecutableName = "arm-none-eabi-as.exe";
		else
			assemblerExecutableName = "arm-none-eabi-as";

        assemblerProg = Path.Combine(assemblerProg, assemblerExecutableName);
        if (!File.Exists(assemblerProg))
            throw new AsmException("Cannot access the Gnu Assembler at path "+assemblerProg);
	}

	// perform pass 1 over all the input files
	public void PerformPass() {
		pass = 1;
		for( int i=0;  i<fileList.Count;  i++ )
			processFile(i);
        if (externSymbols.Count > 0)
            revisitLibraries();
		passComplete[1] = true;
        //Debug.WriteLine(String.Format(
        //    "{0} errors detected in pass 1", AssemblerErrors.ErrorReports.Count));

        // We are finished with the libraries, remove them from the list
        for (int j = fileList.Count - 1; j >= 0; j--)
        {
            string fn = fileList[j];
            if (fn != null)
            {
                string suffix = getSuffix(fn);
                if (suffix != ".a")
                {
                    // we also remove included library files when errors were discovered;
                    // this helps reduce the codeview tab list to a reasonable length
                    if (suffix != ".o)" || AssemblerErrors.ErrorReports.Count == 0)
                        continue;
                }
            }
            fileList.RemoveAt(j);
            FileInfoTable.RemoveAt(j);
        }
	}

    // perform pass 2 over all the source files
    public void PerformPass(AssembledProgram assp)
    {
		this.ap = assp;
		pass = 2;
        ap.TextStart = pcs.TextStart;
        ap.DataStart = pcs.DataStart;
        ap.BssStart = pcs.BssStart;
        ap.DataEnd = pcs.DataEnd;
		for( int i=0;  i<fileList.Count;  i++ )
			processFile(i);
		passComplete[2] = true;
        //Debug.WriteLine(String.Format(
        //    "{0} errors detected in pass 2", AssemblerErrors.ErrorReports.Count));
		//ap.TextStart = pcs.TextStart;
		//ap.DataStart = pcs.DataStart;
		//ap.BssStart  = pcs.BssStart;
        //ap.DataEnd   = pcs.DataEnd;
	}

    private void outputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        string line = outLine.Data;
        if (line == null) return;
        switch(listingState) {
            case 0:
                if (line.StartsWith("DEFINED SYMBOLS"))
                    listingState = 1;
                else
                    listing.Add(line);
                break;
            case 1:
                if (line.StartsWith("UNDEFINED SYMBOLS"))
                    listingState = 2;
                else
                    symTabListing.Add(line);
                break;
            case 2:
                // ignore any further lines
                break;
        }
    }

    private void errorHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        // parse the line of error output; it has the following format
        //      FILENAME:LINENUMBER: ERROR MESSAGE
        string line = outLine.Data;
        if (line == null) return;
        int lineNum;
        // look for 2 consecutive colons separated only by decimal digits
        int colon1pos = -1;
        int colon2pos = -1;
        do {
            colon2pos = line.IndexOf(':', colon1pos + 1);
            if (colon2pos <= colon1pos) return;
            lineNum = 0;
            for (int i = colon1pos + 1; i < colon2pos; i++)
            {
                char c = line[i];
                if (!Char.IsDigit(c)) {
                    colon1pos = colon2pos;
                    break;
                }
                lineNum = lineNum * 10 + (int)c - (int)'0';
            }
        } while (colon1pos == colon2pos);
        string message = line.Substring(colon2pos + 1);
        AssemblerErrors.AddError(fileName, lineNum, 0, message);
        errorCnt++;
    }

    private bool runAssembler(string asFileName, string objFileName)
    {
        errorCnt = 0;
        listingState = 0;
        listing.Clear();
        symTabListing.Clear();
        using (Process p = new Process())
        {
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler(outputHandler);
            p.ErrorDataReceived += new DataReceivedEventHandler(errorHandler);
            p.StartInfo.FileName = assemblerProg;
            //p.StartInfo.Arguments = asFileName + " -alns -mcpu=arm7tdmi-s -mfpu=vfp -o " + objFileName;
            p.StartInfo.Arguments = String.Format("\"{0}\" -alns -march=armv7-a -mfpu=vfp -o \"{1}\"", asFileName, objFileName);
            fileName = asFileName;
            try
            {
                p.Start();
            }
            catch (Exception)
            {
                throw new AsmException("Unable to execute {0};\n\tArguments: {1}", assemblerProg, p.StartInfo.Arguments);
            }
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            if (errorCnt == 0 && !File.Exists(objFileName))
                throw new AsmException("Object code file not created, path: "+objFileName);
        }
        return errorCnt == 0;
    }

    private string mapSrcToTempObj(string srcFileName) {
        string s = Path.GetFileNameWithoutExtension(srcFileName)+".o";
        return Path.Combine(tempFolder, s);
    }

	// define all labels given a specific load point
	public void PlaceCode( int loadPoint ) {
        pcs = new PlaceCodeSections(FileInfoTable, globalSymbols, loadPoint);
	}

	// define all labels, assuming default load point
	public void PlaceCode() {
        pcs = new PlaceCodeSections(FileInfoTable, globalSymbols);
	}

	// perform next pass over the i-th file in the list
	private void processFile( int i ) {
		string currFileName = fileList[i];
		if (currFileName == null) {
			Debug.WriteLine("error? -- missing file name");
			return;
		}
		if (!File.Exists(currFileName))
			throw new AsmException("Cannot access file "+currFileName);
        ArmFileInfo fileInfo = FileInfoTable[i];
        switch (getSuffix(currFileName))
        {
			case ".s":
				// process ARM assembly language file by invoking the CodeSourcery version
                // of Gnu's as program, translating it into a .o file -- then the .o file
                // is handled in the same way as any .o file
                bool asmOK = true;
                if (pass == 1) {
                    string objFileName = mapSrcToTempObj(currFileName);
                    asmOK = runAssembler(currFileName, objFileName);
                    if (asmOK)
                        fileInfo = new ObjFromAsmFileInfo(objFileName, currFileName,
                                    listing, symTabListing, globalSymbols, externSymbols);
                    else
                        fileInfo = new AsmFileInfo(currFileName, globalSymbols, externSymbols);
                    FileInfoTable[i] = fileInfo;
                    listing.Clear();
                    symTabListing.Clear();
                } else {
                    fileInfo.ProgramSpace = ap;
                }
                if (fileInfo.Pass < pass && asmOK)  {
                    fileInfo.Pass = pass;
                    fileInfo.StartPass();
                }
				break;
			case ".o":   // object code file
            case ".o)":  // member of a library archive
				// process ELF format object code file
				if (fileInfo == null) {
                    fileInfo = new ObjFileInfo(currFileName, globalSymbols, externSymbols);
                    FileInfoTable[i] = fileInfo;
				}
				if (pass == 2)  {
					fileInfo.ProgramSpace = ap;
				}
                if (fileInfo.Pass < pass)
                {
                    fileInfo.Pass = pass;
                    fileInfo.StartPass();
                }
				break;
            case ".a":
                // accept a Gnu format archive file containing ELF object code members
                if (fileInfo == null)
                    searchLibrary(currFileName);
                else
                    throw new AsmException(
                        "unexpected library file in pass 2: {0}", currFileName);
                break;
			default:
				throw new AsmException("unsupported file type ({0})", currFileName);
		}

	}

    private void revisitLibraries()
    {
        bool keepGoing = true;
        while (keepGoing)
        {
            keepGoing = false;
            for (int i = 0; i < fileList.Count; i++)
            {
                string currFileName = fileList[i];
                if (currFileName == null)
                    continue;
                if (getSuffix(currFileName) != ".a")
                    continue;
                if (searchLibrary(currFileName))
                    keepGoing = true;
            }
        }
    }

    static private string getSuffix(string currFileName)
    {
        int dotPos = currFileName.LastIndexOf('.');
        if (dotPos < 0) dotPos = 0;
        return currFileName.Substring(dotPos).ToLower();
    }

    // searches the library for unresolved symbols
    // the result is true if new symbols were added
    private bool searchLibrary(string currFileName)
    {
        if (AssemblerErrors.ErrorReports.Count > 0)
            return false; // don't bother searching if there were errors
        bool result = false;
        using (ArmElfLibReader archive = new ArmElfLibReader(currFileName))
        {
            IList<ArmFileInfo> newLibMembers = new List<ArmFileInfo>();
            bool progress = true;
            while (progress)
            {
                progress = false;
                IList<string> defined = new List<string>();
                // copy the list of externals so that we can modify it inside the loop
                string[] externs = new string[externSymbols.Keys.Count];
                externSymbols.Keys.CopyTo(externs, 0);
                foreach (string sy in externs)
                {
                    if (globalSymbols.ContainsKey(sy))
                    {
                        defined.Add(sy);
                        continue;
                    }
                    string fn;
                    FileStream member = archive.GetLibraryFile(sy, out fn);
                    if (member == null) continue;
                    defined.Add(sy);
                    ArmFileInfo fileInfo = new ObjFileInfo(member, currFileName, fn, globalSymbols, externSymbols);
                    newLibMembers.Add(fileInfo);
                    fileInfo.Pass = pass;
                    fileInfo.StartPass();  // this call might change externSymbols
                    progress = true;
                    result = true;
                }
                foreach (string sy in defined)
                    externSymbols.Remove(sy);
            }
            foreach (ArmFileInfo af in newLibMembers)
            {
                FileInfoTable.Add(af);
                fileList.Add(af.FileName);
            }
        }
        return result;
    }

	public SyEntry LookupGlobalSymbol( string name ) {
		return globalSymbols[name.ToLower()];
	}

	public int LookupGlobalLabelValue( string name ) {
		SyEntry entry = LookupGlobalSymbol(name);
		if (entry == null || entry.Kind != SymbolKind.Label)
			return -1;
		return entry.SymValue;
	}

	public int EntryPointAddress {
		get{
			if (pass < 2) return -1;
			int ep = LookupGlobalLabelValue("_start");
			if (ep > 0) return ep;
			ep = LookupGlobalLabelValue("main");	// added by NH; 07-03-06
			return (ep>=0)? ep : pcs.LoadPoint;
		}
	}

	public void DumpInfo() {
		if (pass == 0) {
			Console.WriteLine("No assembly or object files processed yet");
			return;
		}
		if (passComplete[2])
			Console.WriteLine("Pass 2 completed");
		else if (passComplete[1])
			Console.WriteLine("Pass 1 completed");
	}	
	
#if STANDALONETEST
	static void Main( string[] args ) {
		bool tf = false;
		AssembledProgram ap;
		List<string> fl = new List<string>();
		foreach(string s in args) {
			if (s == "/t" || s == "/trace")
				tf = true;
			else
				fl.Add(s);
		}
		ArmAssembler arms = new ArmAssembler(fl);
		arms.TraceParse = tf;
		arms.PerformPass();
		if (AssemblerErrors.ErrorReports.Count == 0) {
			arms.DumpInfo();
			arms.PlaceCode();
			ap = new AssembledProgram(arms.pcs.LoadPoint, arms.pcs.Length);
			arms.PerformPass(ap);
		}
		arms.DumpInfo();
	}
#endif
}

} // end namespace

