using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ARMSim.Preferences;
using ARMSim.CommandLine;
using ARMPluginInterfaces;

namespace ARMSim.Simulator
{
    /// <summary>
    /// This delegate defines a callback when a view wishes to output some text to the OutputView
    /// </summary>
    /// <param name="c"></param>
    public delegate void CharacterOutputDelegate(char chr);

    /// <summary>
    /// Delegate defines a callback when a CPU register is written to
    /// </summary>
    /// <param name="reg"></param>
    //public delegate void RegisterChangedDelegate(uint reg);

    /// <summary>
    /// Delegate defines callback when memory is written to.
    /// </summary>
    /// <param name="address">address written</param>
    /// <param name="ms">size written</param>
    /// <param name="oldValue">previous value</param>
    /// <param name="newValue">new value</param>
    public delegate void MemoryChangedDelegate(uint address, ARMPluginInterfaces.MemorySize ms, uint oldValue, uint newValue);

    /// <summary>
    /// Delegate defines callback when cache memory is written to.
    /// </summary>
    /// <param name="line">cache line written</param>
    /// <param name="word">word written</param>
    public delegate void CacheChangedDelegate(uint line, uint word);

    /// <summary>
    /// This delegate defines a callback when a view wishes to select a
    /// line of code in the CodeView based on a line number.
    /// Currently only used when a user double-clicks an error message in the outputView
    /// </summary>
    /// <param name="fileName">source filename containing line of code </param>
    /// <param name="line">line number in file</param>
    public delegate void CodeSelectionDelegate(string fileName, int line);

    /// <summary>
    /// This delegate defines a callback to get the current code label class
    /// </summary>
    /// <returns>CodeLabels of the current program</returns>
    public delegate CodeLabels CodeLabelsDelegate();

    /// <summary>
    /// This delegate defines a callback when a view wishes to resolve a symbol
    /// defined in the currently loaded program.
    /// </summary>
    /// <param name="symbol">symbol to resolve</param>
    /// <param name="address">address of symbol</param>
    /// <returns>true if successful</returns>
    public delegate bool ResolveSymbolDelegate(string symbol, ref uint address);

    /// <summary>
    /// This class extends the raw ARM simulator engine. It adds application specific
    /// functionality. Loads/Unloads programs, handles the assembly process, manages
    /// the plugins.
    /// </summary>
    public class ApplicationJimulator : ARM7TDMICore, ISimulator, IScriptingInterface
    {
        public ARMPreferences ARMPreferences { get; set; }

        private ArmAssembly.AssembledProgram   mAssembledProgram;
        private ArmAssembly.ArmAssembler       mArmAssembler;

        //flag indicating that the simulation has stopped because:
        //1) an swi 0x11 instruction encountered
        //2) bad memory reference caused sim to stop
        //3) misaligned memory access(and trap flag is set)
        //public bool Stopped { get; set; }

        //flag indicates that the simulation engine has a valid compiled program loaded.
        public bool ValidLoadedProgram { get; private set; }

        // flag indicates that the files for the code view are in the process of being created
        public bool CreatingCodeView { get; set; }

        //Flag indicates that the armsim application is shutting down. This is used to inform the
        //plugins that the app is stopping and not to block (wait for input), or if they are waiting
        //for input, to stop and return.
        //private bool mTerminateInput;

        /// <summary>
        /// Tracks all code labels in the target program.
        /// </summary>
        private CodeLabels mCodeLabels;

        /// <summary>
        /// Sets/Gets the current user directory. This is normally the same directory
        /// that the source code program was loaded from.
        /// </summary>
        public string UserDirectory { get; set; }

        private uint    mEntryPoint;    //the starting address in the prgram. usually the _start symbol
        private uint    mStackPointer;  //the initial stack pointer, computed at load time

        /// <summary>
        /// Obtain an instance of the current breakpoints
        /// </summary>
        public Breakpoints Breakpoints { get; private set; }

        /// <summary>
        /// Tracks the standard file handles(stdout,stdin,stderr)
        /// </summary>
        //private StandardFiles mStandardFiles;

        /// <summary>
        /// Manages all the simulator plugins
        /// </summary>
        private Plugins.PluginManager mPluginManager;

        private ARMSimArguments mParsedArgs;
        /// <summary>
        /// ApplicationJimulator ctor
        /// Startup the simulator
        /// </summary>
        /// <param name="parsedArgs">command line arguments</param>
        public ApplicationJimulator(ARMSimArguments parsedArgs)
        {
            mParsedArgs = parsedArgs;
            Breakpoints = new Breakpoints();
            mPluginManager = new ARMSim.Simulator.Plugins.PluginManager(this);
            //this.Stopped = true;

            //mStandardFiles = new StandardFiles(parsedArgs);
        }//ApplicationJimulator ctor

        /////<summary>Access to the Stdout stream</summary>
        //public OutputARMSimFileStream Stdout { get { return mStandardFiles.Stdout; } }
        /////<summary>Access to the Stderr stream</summary>
        //public OutputARMSimFileStream Stderr { get { return mStandardFiles.Stderr; } }
        /////<summary>Access to the Stdin stream</summary>
        //public InputARMSimFileStream Stdin { get { return mStandardFiles.Stdin; } }

        ///<summary>Access to info about the assembled program</summary>
        public ArmAssembly.ArmAssembler ArmAssemblerInstance { get { return mArmAssembler; } } 

        ///<summary>Return the error reports forloaded program</summary>
        public ArmAssembly.AssemblerErrorsArray ErrorReports
        {
            get { return ArmAssembly.AssemblerErrors.ErrorReports; }
        }

        /// <summary>
        /// Delegate that handles output to console.
        /// </summary>
        public CharacterOutputDelegate ConsoleOutput { get; set; }

        public override void OutputConsoleString(string fmt, params object[] parms)
        {
            if (ConsoleOutput == null)
                return;

            string str = string.Format(fmt, parms);
            foreach (char ch in str)
            {
                ConsoleOutput(ch);
            }
        }

        ///// <summary>
        ///// Write a character to the OutputView console.
        ///// </summary>
        ///// <param name="ch">character to write</param>
        //public override void WriteConsole(char chr)
        //{
        //    if (this.ConsoleOutput != null)
        //        this.ConsoleOutput(chr);
        //}
        ///// <summary>
        ///// Write a character and line feed to the OutputView console.
        ///// </summary>
        ///// <param name="ch">character to write</param>
        //public override void WriteLineConsole(char chr)
        //{
        //    this.WriteConsole(chr);
        //    this.WriteConsole('\n');
        //}
        ///// <summary>
        ///// Write a string to the OutputView console.
        ///// </summary>
        ///// <param name="str">string to write</param>
        //public override void WriteConsole(string str)
        //{
        //    foreach (char ch in str)
        //    {
        //        this.WriteConsole(ch);
        //    }
        //}
        ///// <summary>
        ///// Write a string and line feed to the OutputView console.
        ///// </summary>
        ///// <param name="str">string to write</param>
        //public override void WriteLineConsole(string str)
        //{
        //    this.WriteConsole(str);
        //    this.WriteConsole('\n');
        //}

        ///<summary>Access to the CodeLabels of currently loaded program</summary>
        public CodeLabels CodeLabels { get { return mCodeLabels; } }

        public ICodeLabels GetCodeLabels { get { return mCodeLabels; } }

        ///<summary>Access to the assembled program of currently loaded program</summary>
        public ArmAssembly.AssembledProgram AssembledProgram { get { return mAssembledProgram; } }

        public bool Load(string[] fileNames)
        {
            return Load(new List<string>(fileNames));
        }

        /// <summary>
        /// Load a new program into the simulator. This function will compile it, load it and set up the
        /// simulator for a run.
        /// </summary>
        /// <param name="fileNames">list of filenames to load</param>
        /// <param name="preferences">ARMSim preferences</param>
        /// <returns>true if sucessful</returns>
        public bool Load(IList<string> fileNames)
        {
            this.ValidLoadedProgram = false;

            foreach (string str in fileNames)
            {
                if (str.EndsWith(".o") || str.EndsWith(".O"))
                    OutputConsoleString("Loading object code file {0}\n", str);
                else if (str.EndsWith(".a") || str.EndsWith(".A"))
                    OutputConsoleString("Searching library archive {0}\n", str);
                else
                    OutputConsoleString("Loading assembly language file {0}\n", str);
            }//foreach

            mArmAssembler = new ArmAssembly.ArmAssembler(fileNames);

            try
            {
                //perform pass 1 on all the files
                mArmAssembler.PerformPass();
                //ar.DumpInfo();
                if (ArmAssembly.AssemblerErrors.ErrorReports.Count <= 0)
                {

                    // now determine where everything will go in memory
                    mArmAssembler.PlaceCode((int)this.ARMPreferences.SimulatorPreferences.MemoryStart);
                    //ar.DumpInfo();

                    // allocate a block of memory for the assembled
                    // and linked program
                    mAssembledProgram = new ArmAssembly.AssembledProgram(mArmAssembler);

                    // perform pass 2 on all the files
                    mArmAssembler.PerformPass(mAssembledProgram);
                    //ar.DumpInfo();
                    //ap.Hexdump();	// display all code
                }
            }
           catch (Exception ex)
            {
                OutputConsoleString("A fatal error occurred while assembling the program, Reason:{0}\n", ex.Message);
                OutputConsoleString("Please keep a copy of the ARM source code and report this ARMSim bug!\n");
                return false;
            }//catch

            if (ArmAssembly.AssemblerErrors.ErrorReports.Count > 0)
            {
                OutputConsoleString("The following assembler/loader errors occurred ...\n");
                IDictionary<string, IList<ArmAssembly.ErrorReport>> xxx = ArmAssembly.AssemblerErrors.ErrorReports.ErrorLists;

                //display each reported error in the outputview
                foreach(string fileName in xxx.Keys) {
                    OutputConsoleString("** File: {0}\n", fileName);
                    IList<ArmAssembly.ErrorReport> ht = ArmAssembly.AssemblerErrors.ErrorReports.GetErrorsList(fileName);
                    foreach (ArmAssembly.ErrorReport ce in ht)
                    {
                        string fmt;
                        if (ce.Col == 0)
                            if (ce.Line == 0)
                                fmt = "   Message = {0}\n";
                            else
                                fmt = "   Line {1}: Message = {0}\n";
                        else
                            fmt = "   Line {1}, col {2}: Message = {0}\n";
                        OutputConsoleString(fmt, ce.ErrorMsg, ce.Line, ce.Col);
                    }
                }
                OutputConsoleString("End of assembler errors\n");

                //all done for error case
                return false;
            }

            //Construct the code labels data structure from the assembled program.
            mCodeLabels = new CodeLabels(mArmAssembler);

            //tell the simulator which directory to try for user files
            this.UserDirectory = System.IO.Path.GetDirectoryName(fileNames[0]);

            // set the program entry point to be the _start label if it exists;
            //   otherwise set it to the address of main if it exists;
            //   otherwise set it to the start of memory
            //save the entry point in case the user does a "Restart" but the preferences may have changed - dale
            if (!mCodeLabels.LabelToAddress("_start", ref mEntryPoint))
                if (!mCodeLabels.LabelToAddress("main", ref mEntryPoint))
                    mEntryPoint = (uint)mAssembledProgram.StartAddress;

            //define the simulation memory
            //save the stack pointer in case the user does a "Restart" but the preferences may have changed - dale
            mStackPointer = this.DefineMemory(this.ARMPreferences.SimulatorPreferences, mAssembledProgram.Memory.Length);

            //create the caches based on the user preferences
            this.DefineCache(this.ARMPreferences.CachePreferences);

            //perform a restart - loads code into memory, zeros cpu registers, sets pc and sp
            //note sets the ValidLoadedProgram flag to true
            this.Restart();

            return true;
        }//Load

        /// <summary>
        /// Restart the simulator. Reload programs, reset all the views and plugins.
        /// </summary>
        /// <param name="preferences">simulator preferences</param>
        public override void Restart()
        {
            //reset the simulation engine. Note that it will now be in an invalid state until a program is loaded.
            this.Reset();
            this.TrapUnalignedMemoryAccess = this.ARMPreferences.SimulatorPreferences.StopOnMisaligned;
            this.ProtectTextArea = this.ARMPreferences.SimulatorPreferences.ProtectTextArea;

            //restart all the plugins
            mPluginManager.onRestart();

            //fill our memory block with the preferences fill pattern.
            this.mMemoryBlock.Reset(this.ARMPreferences.SimulatorPreferences.FillPattern,
                this.ARMPreferences.SimulatorPreferences.StackGrowsDown);

            //load the simulation memory block from the assembled program one opcode at a time
            uint address = (uint)mAssembledProgram.StartAddress;
            bool oldPTA = this.MainMemory.ProtectTextArea;
            this.MainMemory.ProtectTextArea = false;  // allow text area to be rewritten
            foreach (uint data in mAssembledProgram.Memory)
            {
                this.MainMemory.SetMemory(address, ARMPluginInterfaces.MemorySize.Word, data);
                address += 4;
            }
            this.MainMemory.ProtectTextArea = oldPTA;

            // pass on the info of where the static data area starts and ends
            this.MainMemory.DataStart = (uint)mAssembledProgram.DataStart;
            this.MainMemory.DataEnd = (uint)mAssembledProgram.DataEnd;

            //set the start program counter
            this.GPR.PC = mEntryPoint;
            this.GPR.SP = mStackPointer;
            this.MainMemory.StackStart = mStackPointer;

            //indicate that the simulation engine has a valid compiled program loaded.
            this.ValidLoadedProgram = true;

        }//Restart

        /// <summary>
        /// Shutdown the simulator. The application is terminating
        /// </summary>
        public void Shutdown()
        {
            mPluginManager.onShutdown();            //shutdown all plugins
            this.ValidLoadedProgram = false;             //set simulator to a non valid state
        }//ShutDown

        /// <summary>
        /// Get data from memory.
        /// If address is in range of main memory, get from main memory block.
        /// If address is not in range, pass to the plugins to see if they are interested.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <returns>read value</returns>
        public override uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            if (this.MainMemory.InRange(address, ms))
            {
                return mL1DataCache.GetMemory(address, ms);
            }
            return mPluginManager.MemoryRead(address, ms);
        }//GetMemory

        /// <summary>
        /// Store data in memory.
        /// If address is in range of main memory, send to main memory block.
        /// If address is not in range, pass to the plugins to see if they are interested.
        /// Throw an exception if address out of range and no plugin handles the store.
        /// </summary>
        /// <param name="address">address to write</param>
        /// <param name="ms">size to write</param>
        /// <param name="data">data to write</param>
        public override void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data)
        {
            if (this.ProtectTextArea)
            {
                if (this.MainMemory.InDataRange(address, ms))
                {
                    mL1DataCache.SetMemory(address, ms, data);
                    return;
                }
            }
            else
                if (this.MainMemory.InRange(address, ms))
                {
                    mL1DataCache.SetMemory(address, ms, data);
                    return;
                }
            mPluginManager.MemoryWrite(address, ms, data);
        }//SetMemory

        /// <summary>
        /// Sets the memoryChanged delegate
        /// This function is called when memory is written to.
        /// </summary>
        public void SetMemoryChangedHandler( MemoryChangedDelegate d )
        {
            mMemoryBlock.MemoryChangedHandler = d;
        }

        /// <summary>
        /// Initialize the plugins.
        /// Find the plugins, load them calling their init method, then when all load call onLoad method
        /// </summary>
        /// <param name="boardView">reference to BoardControlsView</param>
        /// <param name="outputView">reference to OutputView</param>
        public void InitPlugins(GUI.Views.PluginsUIView boardView, GUI.Views.IOutputView outputView)
        {
            mPluginManager.Init(boardView, outputView, this.ARMPreferences.PluginPreferences);
        }//initPlugins

        /// <summary>
        /// When an opcode is being executed and the simulator cannot decode it,
        /// pass it here. We will let the plugins have a crack at it.
        /// </summary>
        /// <param name="opcode">opcode to execute</param>
        /// <returns>clock cycles executed</returns>
        public override uint UnknownOpCode(uint opCode)
        {
            return mPluginManager.onExecute(opCode);
        }

        /// <summary>
        /// This is called after each instruction is executed.
        /// The number of cycles is passed to the plugins and any plugins
        /// that subscribed to the Cycles event will get notified.
        /// </summary>
        /// <param name="count"></param>
        public override void HandleCycles(ushort count)
        {
            mPluginManager.onCycles(count);
        }

        /// <summary>
        /// Test if a memory operation is in range of the main memory block.
        /// </summary>
        /// <param name="address">address to test</param>
        /// <param name="ms">size of memory operation</param>
        /// <returns>true if in range</returns>
        public override bool InRange(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            return mMemoryBlock.InRange(address, ms) || mPluginManager.InRange(address, ms);
        }//InRange

        public void DefinePlugins(PluginPreferences preferences)
        {
            mPluginManager.DeactivateAllPlugins();
            mPluginManager.ActivatePlugins(preferences.SettingsPlugins);
        }

        public ICollection<Plugins.PluginManager.PluginItem> AvailablePlugins { get { return mPluginManager.AvailablePlugins; } }

        public override uint GetMemoryNoSideEffect(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            if (mMemoryBlock == null)
                return 0;  // no memory allocated yet
            //System.Diagnostics.Debug.Assert(this.InRange(address, ms));
            if (mMemoryBlock.InRange(address, ms))
                return mMemoryBlock.GetMemory(address, ms);
            if (mPluginManager.InRange(address, ms))
                return mPluginManager.MemoryRead(address, ms);
            throw new MemoryAccessException(address, false);
        }

        /// <summary>
        /// Notify plugins that a simulation run is starting
        /// </summary>
        public void StartSimulation()
        {
            //if (!this.Stopped)
            //    return;
            //clear the stopped state
            //this.Stopped = false;
            mPluginManager.onStart();
        }

        /// <summary>
        /// Stop the simulation.
        /// This is called:
        /// 1) when an SWI_Exit instruction was encountered
        /// 2) a memory exception is caught (read or write outside of memory bounds)
        /// 3) an unaligned memory access (and trap flag is set)
        /// </summary>
        public override void HaltSimulation()
        {
            //if (this.Stopped)
            //    return;
            //set the stopped state
            //this.Stopped = true;

            //if the Sync cache on exit is set in the preferences and the cache is defined
            //then go ahead and resync the cache with main memory
            if (this.ARMPreferences.GeneralPreferences.SyncCacheOnExit && mL1DataCache != null)
            {
                mL1DataCache.Purge();
            }
            mPluginManager.onStop();

            HaltRequested = true;
        }//SimulationExit

        public IARMSimArguments ARMSimArguments { get { return mParsedArgs; } }

        public IDictionary<string, IList<IErrorReport>> GetErrorReports
        {
            get
            {
                ArmAssembly.AssemblerErrorsArray errorReports = this.ErrorReports;
                IDictionary<string, IList<ArmAssembly.ErrorReport>> errorLists = errorReports.ErrorLists;

                IDictionary<string, IList<IErrorReport>> dictionary = new Dictionary<string, IList<IErrorReport>>();
                foreach (string filename in errorLists.Keys)
                {
                    List<IErrorReport> list = new List<IErrorReport>();
                    IList<ArmAssembly.ErrorReport> errorItems = errorReports.GetErrorsList(filename);
                    foreach (ArmAssembly.ErrorReport errorItem in errorItems)
                    {
                        list.Add(errorItem);
                    }
                    dictionary[filename] = list;
                }
                return dictionary;
            }
        }

    }//class ApplicationJimulator
}
