using System;
using System.Windows.Forms;

using System.Text;

using ARMSim.GUI;
using ARMSim.CommandLine;
using ARMSim.Batch;
using ARMSim.Simulator;
using ARMSim.Preferences;
using System.Runtime.InteropServices;

namespace ARMSim
{
    static class Program
    {
        // External references to kernel for attaching/detaching from the Console
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ARMSimArguments parsedArgs = new ARMSimArguments();

            //Batch mode is the simulator running with no UI and standard I/O instead of a console.
            if (args.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                if (!Parser.ParseArgumentsWithUsage(args, parsedArgs, delegate(string str) { sb.Append(str); sb.Append("\n"); }))
                {
                    MessageBox.Show(sb.ToString(), "Command line parsing Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                parsedArgs.Batch = true;
                bool attachedOK = false;
                if (ARMSim.ARMSimUtil.RunningOnWindows)
                {
                    if (!AttachConsole(ATTACH_PARENT_PROCESS))
                        attachedOK = AllocConsole();
                }
                System.Console.WriteLine("\nARMSim# running in batch mode ...");

                // Run with the command-line arguments
                BatchMode.Run(parsedArgs);

                // Let the user know we are stopping
                System.Console.WriteLine("\nARMSim# is exiting");
                if (attachedOK)
                    FreeConsole();
            }
            else
            {
                //Otherwise we run the simulator as a normal GUI application
                parsedArgs.Batch = false;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //pass in an empty list of arguments
				Application.Run(new ARMSimForm(parsedArgs));
            }

        }//Main
    }//class Program
}
