using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;
using System.IO;

namespace ARMSim.Simulator
{
    /// <summary>
    /// Interface exposed to the script engine and the plugins.
    /// </summary>
    public interface ISimulator
    {
        ARMSim.Preferences.ARMPreferences ARMPreferences { get; }

        /// <summary>
        /// Load file(s) into simulator
        /// </summary>
        /// <param name="fileNames">list of filenames to load</param>
        /// <param name="preferences">the preferences of the simulator</param>
        /// <returns>true if successful</returns>
        bool Load(IList<string> fileNames);

        void Restart();

        /// <summary>
        /// Execute one instruction
        /// </summary>
        void Execute();

        /// <summary>
        /// Check if simulator is in stopped state
        /// </summary>
        //bool Stopped { get; }

        /// <summary>
        /// Access to main memory
        /// </summary>
        IMemoryBlock MainMemory { get; }

        /// <summary>
        /// Access to code labels
        /// </summary>
        CodeLabels CodeLabels { get; }

        /// <summary>
        /// Assembler errors for the last load of file(s)
        /// </summary>
        ArmAssembly.AssemblerErrorsArray ErrorReports { get; }

        /// <summary>
        /// CPU registers
        /// </summary>
        //GeneralPurposeRegisters GPR { get; }
        IGPR GPR { get; }

        VFP.FloatingPointRegisters FPR { get; }

        /// <summary>
        /// CSPR
        /// </summary>
        CPSR CPSR { get; }

        /// <summary>
        /// Checks if a memory access will be in range
        /// </summary>
        /// <param name="address">address to check</param>
        /// <param name="ms">size of memory operation</param>
        /// <returns>true if OK</returns>
        bool InRange(uint address, ARMPluginInterfaces.MemorySize ms);

        ///// <summary>
        ///// Access to stdout file stream
        ///// </summary>
        //OutputARMSimFileStream Stdout { get; }

        ///// <summary>
        ///// Access to stderr file stream
        ///// </summary>
        //OutputARMSimFileStream Stderr { get; }

        ///// <summary>
        ///// Access to stdin file stream
        ///// </summary>
        //InputARMSimFileStream Stdin { get; }

        /// <summary>
        /// Request that an exception be raised.
        /// </summary>
        /// <param name="exception">exception type to raise</param>
        void RequestException(BaseARMCore.ARMExceptions exception);
        //void AssertInterrupt(Jimulator.ARMExceptions exception);

        void OutputConsoleString(string fmt, params object[] parms);

        ///// <summary>
        ///// Write a character to the OutputView console.
        ///// </summary>
        ///// <param name="ch">character to write</param>
        //void WriteConsole(char chr);
        ///// <summary>
        ///// Write a character and linefeed to the OutputView console.
        ///// </summary>
        ///// <param name="ch">character to write</param>
        //void WriteLineConsole(char chr);
        ///// <summary>
        ///// Write a string to the OutputView console.
        ///// </summary>
        ///// <param name="str">string to write</param>
        //void WriteConsole(string str);
        ///// <summary>
        ///// Write a string and a linefeed to the OutputView console.
        ///// </summary>
        ///// <param name="str">string to write</param>
        //void WriteLineConsole(string str);

        /// <summary>
        /// Instruct simulator to stop executing the program.
        /// </summary>
        //void ExitSimulation();

        /// <summary>
        /// Return the current user directory.
        /// </summary>
        string UserDirectory { get; }

        //void EnablePlugin(IARMPlugin plugin);

        //bool TerminateInput { get; set; }
        void HaltSimulation();

        IARMSimArguments ARMSimArguments { get; }

    }//interface ISimulator
}
