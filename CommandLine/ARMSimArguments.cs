using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.CommandLine
{
    /// <summary>
    /// This class defines the commandline arguments that are valid for the ARM Simulator application.
    /// These attributes define the arguments and their behaviour.
    /// The CommandLine parser and this class were all written by a chap named Peter Hallam at Microsoft.
    /// See the CommandLineArguments class for full license agreements and documentation.
    /// </summary>
    public class ARMSimArguments : ARMPluginInterfaces.IARMSimArguments
    {
        /// <summary>
        /// Specifies the directory of the layout file
        /// </summary>
        //[ArgumentAttribute(ArgumentType.AtMostOnce, HelpText = "Location of layout file.")]
        //public string layoutDirectory = null;

        /// <summary>
        /// Specifies stdout file
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName="O", HelpText = "Standard output.")]
        public string Stdout = null;
        public string StdoutFileName { get { return Stdout; } }

        [ArgumentAttribute(ArgumentType.AtMostOnce, LongName = "Stdout+", ShortName = "O+", HelpText = "Standard output in append mode.")]
        public string StdoutAppend = null;
        public string StdoutAppendMode { get { return StdoutAppend; } }

        /// <summary>
        /// Specifies stdin file
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "I", HelpText = "Standard input.")]
        public string Stdin = null;
        public string StdinFileName { get { return Stdin; } }

        /// <summary>
        /// Specifies stderr file
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "E", HelpText = "Standard error.")]
        public string Stderr = null;
        public string StderrFileName { get { return Stderr; } }

        [ArgumentAttribute(ArgumentType.AtMostOnce, LongName = "Stderr+", ShortName = "E+", HelpText = "Standard error in append mode.")]
        public string StderrAppend = null;
        public string StderrAppendMode { get { return StderrAppend; } }

        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "S", HelpText = "SWI extension to use")]
        public string SWI = null;

#if false
        /// <summary>
        /// Specifies script file to execute in batch mode
        /// </summary>
        //[ArgumentAttribute(ArgumentType.AtMostOnce, HelpText = "Script file to run.")]
        //public string scriptFile = null;
        [ArgumentAttribute(ArgumentType.MultipleUnique, HelpText = "Script file(s) to run.")]
        public string[] ScriptFiles = null;
#endif
        /// <summary>
        /// Selects batch mode execution mode (no GUI)
        /// </summary>
        [ArgumentAttribute(ArgumentType.AtMostOnce, HelpText = "Choose batch mode execution.")]
        public bool Batch = false;

        [ArgumentAttribute(ArgumentType.AtMostOnce, ShortName = "X", HelpText = "Max instructions to execute.")]
        public int ExecLimit = Int32.MaxValue;

        /// <summary>
        /// Specifies a list of memory regions to print after execution, format = words (4 bytes)
        /// </summary>
        [ArgumentAttribute(ArgumentType.Multiple, ShortName = "P", HelpText = "Memory area to print (default form is words)")]
        public string[] PrintMemory = null;

        /// <summary>
        /// Specifies a list of files to load into the simulator
        /// </summary>
        [DefaultArgumentAttribute(ArgumentType.MultipleUnique, HelpText = "Files to assemble/load.")]
        public string[] Files = null;

    }//class ARMSimArguments
}