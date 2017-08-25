using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ARMPluginInterfaces;

namespace ARMSim.Simulator.ExtensionInstructions
{
    /// <summary>
    /// This class is an ARMSim plugin that handles the swi instructions in the range 0x0000 - 0x00ff
    /// These instructions perform some basic operations required by the simulator, file io, console io
    /// </summary>
    public class SWIInstructions : IARMPlugin
    {
        private IARMHost mIhost;
        private List<ARMSimStream> mFiles = new List<ARMSimStream>();
        private uint mNextFileHandle;
        private uint mStdioConsole;

        /// <summary>
        /// Called when plugin is first created. Host passes in interface here
        /// </summary>
        /// <param name="ihost"></param>
        public void Init(IARMHost ihost)
        {
            //save the reference to the simulator
            mIhost = ihost;

            //subscribe to the Load event
            mIhost.Load += onLoad;

        }//Init

        /// <summary>
        /// Called when all the plugins have been created and init.
        /// </summary>
        public void onLoad(object sender, EventArgs e)
        {
            //setup for file handles and file io

            //request that we get all SWI calls for the range 0x00 - 0xff (first 256)
            mIhost.RequestOpCodeRange(0x0f000000, 0x0fffff00, this.Execute);

            //setup callbacks needed
            mIhost.Restart += onRestart;
            mIhost.Unload += onUnload;

            SetupStreams();

        }//onLoad

        private static ARMSimStream CreateInputStream(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            return new InputARMSimFileStream(new StreamReader(File.OpenRead(filename)));
        }
        private static ARMSimStream CreateOutputStream(string filename, bool overwrite)
        {
            if (string.IsNullOrEmpty(filename))
                return null;
            return new OutputARMSimFileStream(
                overwrite ? File.CreateText(filename) : File.AppendText(filename));
        }

        private void SetupStreams()
        {
            closeAllStreams();
            mIhost.CloseStandardConsole(mStdioConsole);

            mStdioConsole = mIhost.CreateStandardConsole("stdin/stdout/stderr");

            ARMPluginInterfaces.Preferences.IGeneralPreferences prefs = mIhost.ARMPreferences.IGeneralPreferences;
            IARMSimArguments args = mIhost.ARMSimArguments;

            ARMSimStream armSimStream = CreateInputStream(args.StdinFileName);
            if (armSimStream == null)
                armSimStream = CreateInputStream(prefs.StdinFileName);
            if (armSimStream == null)
                armSimStream = new ARMSimConsoleStream(mStdioConsole, mIhost);

            mFiles.Add(armSimStream);  // index = 0

            bool overwrite = true;
            string filename = null;
            if (args.StdoutFileName != null)
            {
                overwrite = true;
                filename = args.StdoutFileName;
            }
            else if (args.StdoutAppendMode != null)
            {
                overwrite = false;
                filename = args.StdoutAppendMode;
            }
            armSimStream = CreateOutputStream(filename, overwrite);
            if (armSimStream == null)
                armSimStream = CreateOutputStream(prefs.StdoutFileName, prefs.StdoutOverwrite);
            if (armSimStream == null)
                armSimStream = new ARMSimConsoleStream(mStdioConsole, mIhost);

            mFiles.Add(armSimStream);  // index = 1

            if (args.StderrFileName != null)
            {
                overwrite = true;
                filename = args.StderrFileName;
            }
            else if (args.StderrAppendMode != null)
            {
                overwrite = false;
                filename = args.StderrAppendMode;
            }
            armSimStream = CreateOutputStream(filename, overwrite);
            if (armSimStream == null)
                armSimStream = CreateOutputStream(prefs.StderrFileName, prefs.StderrOverwrite);
            if (armSimStream == null)
                armSimStream = new ARMSimConsoleStream(mStdioConsole, mIhost);

            mFiles.Add(armSimStream);  // index = 2
            mNextFileHandle = 3;
        }//SetupStreams

        private void closeAllStreams()
        {
            //close all streams
            foreach (ARMSimStream astream in mFiles)
            {
                if (astream is ARMSimConsoleStream)
                    continue;
                if (astream != null)
                    astream.Close();
            }
            mFiles.Clear();
            mNextFileHandle = 0;
        }

        /// <summary>
        /// Called when the plugin is unloaded(application is terminating)
        /// </summary>
        public void onUnload(object sender, EventArgs e)
        {
            closeAllStreams();
            mIhost.CloseStandardConsole(mStdioConsole);

            //_ihost.StopSimulation -= onRestart;
            mIhost.Restart -= onRestart;
            mIhost.Unload -= onUnload;
            mIhost.Load -= onLoad;

        }//onUnload

        /// <summary>
        /// Called when the simulation is restarting
        /// </summary>
        public void onRestart(object sender, EventArgs e)
        {
            SetupStreams();
        }//onRestart

        //Execute an SWI instruction.
        /// <summary>
        /// Called when an swi 0x0000   to   swi 0x00ff is executed
        /// </summary>
        /// <param name="op_code">opcode being executed</param>
        /// <returns>number of cycles executed. 0 if none</returns>
        public uint Execute(uint op_code)
        {
            //extract the bottom 8 bits to get swi code.
            uint swi_code = op_code & 0x000000ff;

            try
            {
                //and execute based on the swi code.
                switch (swi_code)
                {
                    //write a character to Outputview
                    //r0: character to output
                    case 0x00:
                        {
                            char c = (char)mIhost.getReg(0);
                            mIhost.OutputConsoleString(new string(c, 1));
                        } break;

                    //write a character string to Outputview
                    //r0: address of (null terminated) string to display
                    case 0x02:
                        {
                            string s = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, mIhost.getReg(0), 256);
                            mIhost.OutputConsoleString(s);
                        } break;

                    //SWI_Exit
                    case 0x11:
                        mIhost.HaltSimulation();
                        mIhost.GPR[15] -= 4;//todo thumb mode?
                        this.closeAllStreams();
                        break;

                    //SWI_HeapMalloc
                    //r0: size of block in bytes
                    //Output: C set on error
                    //r0: address of allocated block (or 0 if error)
                    case 0x12:
                        mIhost.setReg(0, mIhost.Malloc(mIhost.getReg(0)));
                        mIhost.cf = (mIhost.getReg(0) == 0);
                        break;

                    //SWI_HeapClear
                    case 0x13:
                        mIhost.HeapClear();
                        break;

                    //write a character to an open file
                    //r0: file handle (value 1 == screen)
                    //r1: character to output
                    //Output: C flag set on error
                    case 0x60:
                        {
                            string text = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, mIhost.getReg(1), 1);
                            uint fileHandle = mIhost.getReg(0);
                            if (fileHandle < mFiles.Count)
                            {
                                mFiles[(int)fileHandle].Write(text);
                                mIhost.cf = false;
                            }
                            else
                                mIhost.cf = true;
                        } break;

                    //read one byte (an ASCII character?) from an open file
                    //r0: file handle (value 0 == keyboard)
                    //Output: C flag set on error
                    //r0: the byte read from the file, or -1 if at EOF
                    case 0x61:
                        {
                            uint fileHandle = mIhost.getReg(0);
                            bool error = fileHandle >= mFiles.Count;
                            if (!error)
                            {
                                int ch = (char)mFiles[(int)fileHandle].Read();
                                mIhost.setReg(0, (uint)ch);
                            }
                            mIhost.cf = error;
                        } break;

                    //read bytes from an open file
                    //r0: file handle
                    //r1: input buffer address
                    //r2: buffer size (number of bytes to read)
                    case 0x62:
                        {
                            uint fileHandle = mIhost.getReg(0);
                            uint address = mIhost.getReg(1);
                            uint maxbytes = mIhost.getReg(2);

                            bool error = fileHandle >= mFiles.Count;

                            //special case if maxbytes is 0. Just return a success.
                            if (maxbytes == 0 || error)
                            {
                                mIhost.cf = error;
                                mIhost.setReg(0, 0);
                                break;
                            }

                            uint storedBytes = 0;
                            while (storedBytes < maxbytes)
                            {
                                int ch = (int)mFiles[(int)fileHandle].Read();
                                if (ch < 0) break;
                                mIhost.SetMemory(address++, ARMPluginInterfaces.MemorySize.Byte, (uint)ch);
                                storedBytes++;
                            }
                            mIhost.cf = false;
                            mIhost.setReg(0, storedBytes);
                        } break;

                    //write bytes to an open file
                    //r0: file handle
                    //r1: output buffer address
                    //r2: buffer size (number of bytes to write)
                    //Output: C flag set on error
                    //r0: the number of bytes successfully written to the file
                    case 0x63:
                        {
                            uint fileHandle = mIhost.getReg(0);
                            uint address = mIhost.getReg(1);
                            uint maxbytes = mIhost.getReg(2);

                            bool error = fileHandle >= mFiles.Count;

                            //special case if maxbytes is 0. Just return a success.
                            if (maxbytes == 0 || error)
                            {
                                mIhost.cf = error;
                                mIhost.setReg(0, 0);
                                break;
                            }

                            uint copiedBytes = 0;
                            while (copiedBytes < maxbytes)
                            {
                                uint ch = mIhost.GetMemory(address++, ARMPluginInterfaces.MemorySize.Byte);
                                mFiles[(int)fileHandle].Write((char)ch);
                                copiedBytes++;
                            }
                            mIhost.cf = false;
                            mIhost.setReg(0, copiedBytes);
                        }
                        break;

                    //open text file for input or output
                    //r0: pointer to filename to open, null terminated
                    //r1: mode -- 0 means input, 1 means output, 2 means append, 0x100 - stdconsole(filename used as title).
                    //Output: C flag set on error
                    //r0: file handle(-1 on error)
                    case 0x66:
                        {
                            bool bError = true;
                            string str = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, mIhost.getReg(0), 256);

                            string currDir = Directory.GetCurrentDirectory();
                            if (!string.IsNullOrEmpty(mIhost.UserDirectory))
                                Directory.SetCurrentDirectory(mIhost.UserDirectory);

                            ARMSimStream astream = null;
                            try
                            {
                                switch (mIhost.getReg(1)) {
                                    case 0:
                                        astream = new InputARMSimFileStream(new StreamReader(File.OpenRead(str)));
                                        break;
                                    case 1:
                                        astream = new OutputARMSimFileStream(new StreamWriter(File.Open(str, FileMode.Create)));
                                        break;
                                    case 2:
                                        astream = new OutputARMSimFileStream(new StreamWriter(File.Open(str, FileMode.Append)));
                                        break;
                                    case 0x100:
                                        {//create a new standardConsole
                                            uint stdHandle = mIhost.CreateStandardConsole(str);
                                            astream = new ARMSimConsoleStream(stdHandle, mIhost);
                                        }
                                        break;
                                    default:
                                        break;
                                }//switch

                                uint fileHandle = uint.MaxValue;
                                if (astream != null) {
                                    fileHandle = mNextFileHandle++;
                                    mFiles.Add(astream);
                                    bError = false;
                                }
                                mIhost.setReg(0, fileHandle);
                            }//try
                            catch (Exception)
                            {
                                mIhost.setReg(0, uint.MaxValue);
                            }
                            Directory.SetCurrentDirectory(currDir);
                            mIhost.cf = bError;
                        } break; //case 0x66

                    //close an open file
                    //r0: file handle
                    //Output: C flag set on error
                    case 0x68:
                        {
                            uint handle = mIhost.getReg(0);
                            if (handle < mFiles.Count) {
                                mFiles[(int)handle].Close();
                                mFiles[(int)handle] = null;
                                mIhost.cf = false;
                            } else
                                mIhost.cf = true;
                        } break;

                    //write null-terminated string to screen or to open file
                    //r0: file handle (value 1 == screen)
                    //r1: address of string to read from memory
                    //Output: C flag set on error
                    case 0x69:
                        {
                            uint addr = mIhost.getReg(1);
                            string text = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, addr, 256);
                            uint fileHandle = mIhost.getReg(0);
                            if (fileHandle < mFiles.Count) {
                                mFiles[(int)fileHandle].Write(text);
                                mIhost.cf = false;
                            } else
                                mIhost.cf = true;
                        } break;

                    //read line of text from open file
                    //r0: file handle
                    //r1: location to put read data
                    //r2: max number of bytes to read (including added null terminator)
                    //Output: C flag set on error
                    //r0: number of bytes stored (including added null terminator)
                    case 0x6a:
                        {
                            uint fileHandle = mIhost.getReg(0);
                            uint maxbytes = mIhost.getReg(2);

                            bool error = fileHandle >= mFiles.Count;

                            //special case if maxbytes is 0. Just return a success.
                            if (maxbytes == 0) {
                                mIhost.cf = error;
                                mIhost.setReg(0, 0);
                                break;
                            }

                            uint storedBytes = 0;
                            string text = null;
                            if (!error) {
                                text = mFiles[(int)fileHandle].ReadLine();
                                error = text == null;
                            }

                            if (!error) {
                                storedBytes = (uint)(text.Length + 1);
                                if (maxbytes < text.Length) {
                                    storedBytes = maxbytes;
                                }

                                uint address = mIhost.getReg(1);
                                for (uint i = 0; i < storedBytes - 1; i++) {
                                    uint data = (uint)(text[(int)i]);
                                    mIhost.SetMemory(address++, ARMPluginInterfaces.MemorySize.Byte, data);
                                }
                                mIhost.SetMemory(address++, ARMPluginInterfaces.MemorySize.Byte, 0);

                            }

                            mIhost.setReg(0, storedBytes);
                            mIhost.cf = error;
                        } break;

                    //write signed integer to screen or to open file
                    //r0: file handle (value 1 == screen)
                    //r1: integer value to write
                    //Output: C flag set on error
                    case 0x6b:
                        {
                            uint fileHandle = mIhost.getReg(0);
                            string text = ((int)mIhost.getReg(1)).ToString();
                            if (fileHandle < mFiles.Count) {
                                mFiles[(int)fileHandle].Write(text);
                                mIhost.cf = false;
                            } else {
                                mIhost.cf = true;
                            }
                        } break;

                    //read signed integer from open file
                    //r0: file handle
                    //Output: C flag set on error
                    //r0: value of the integer read from file
                    case 0x6c:
                        {
                            int number = 0;
                            uint fileHandle = mIhost.getReg(0);

                            ARMSimStream sr = null;
                            if (fileHandle < mFiles.Count) {
                                sr = mFiles[(int)fileHandle];
                            }

                            bool error = true;
                            if (sr != null) {
                                error = sr.GetInteger(ref number);
                            }
                            if (!error) {
                                mIhost.setReg(0, (uint)number);
                            }
                            mIhost.cf = error;
                        } break;

                    case 0x6d:
                        //return milliseconds in r0
                        mIhost.setReg(0, (uint)(DateTime.Now.Ticks / 10000));
                        break;

                    //output a double FP number to the console
                    //r0:register to output (0-15)
                    //outputs in the same format as the registers view, uses the formatstring "0.###E+0"
                    case 0x6e:
                        {
                            uint reg = mIhost.getReg(0);
                            if (reg > 15) {
                                reportBadRegister(reg, 0x6e);
                                return 0;
                            }
                            double value = mIhost.getFPDoubleReg(reg);
                            string str = ARMSim.Simulator.VFP.FloatingPointProcessor.DoubleToString(value);
                            mIhost.OutputConsoleString(str);
                        } break;

                    //output a float FP number to the console
                    //r0:register to output (0-31)
                    //outputs in the same format as the registers view, uses the formatstring "0.###E+0"
                    case 0x6f:
                        {
                            uint reg = mIhost.getReg(0);
                            if (reg > 31) {
                                reportBadRegister(reg, 0x6f);
                                return 0;
                            }
                            float value = mIhost.getFPSingleReg(reg);
                            string str = ARMSim.Simulator.VFP.FloatingPointProcessor.FloatToString(value);
                            mIhost.OutputConsoleString(str);
                        } break;

                    //read double value from open file
                    //r0: file handle
                    //r1: register to read value into (0-15)
                    case 0x7b:
                        {
                            double number = 0.0;
                            uint fileHandle = mIhost.getReg(0);
                            uint reg = mIhost.getReg(1);
                            if (reg > 15) {
                                reportBadRegister(reg, swi_code);
                                return 0;
                            }
                            ARMSimStream sr = null;
                            if (fileHandle < mFiles.Count) {
                                sr = mFiles[(int)fileHandle];
                            }

                            bool error = true;
                            if (sr != null) {
                                error = sr.GetDouble(ref number);
                            }
                            if (!error) {
                                mIhost.setFPDoubleReg(0, number);
                            }
                            mIhost.cf = error;
                        } break;

                    //read float value from open file
                    //r0: file handle
                    //r1: register to read value into (0-31)
                    case 0x7c:
                        {
                            double number = 0.0;
                            uint fileHandle = mIhost.getReg(0);
                            uint reg = mIhost.getReg(1);
                            if (reg > 31) {
                                reportBadRegister(reg, swi_code);
                                return 0;
                            }
                            ARMSimStream sr = null;
                            if (fileHandle < mFiles.Count) {
                                sr = mFiles[(int)fileHandle];
                            }//else

                            bool error = true;
                            if (sr != null) {
                                error = sr.GetDouble(ref number);
                            }
                            if (!error) {
                                mIhost.setFPSingleReg(0, (float)number);
                            } else
                                mIhost.cf = error;
                        } break;

                    //output a double FP number to screen or to open file
                    //r0: file handle (value 1 == screen)
                    //r1:register to output (0-15)
                    //outputs in the same format as the registers view, uses the formatstring "0.###E+0"
                    case 0x7e:
                        {
                            uint reg = mIhost.getReg(1);
                            if (reg > 15) {
                                reportBadRegister(reg, swi_code);
                                return 0;
                            }
                            double value = mIhost.getFPDoubleReg(reg);
                            string text = ARMSim.Simulator.VFP.FloatingPointProcessor.DoubleToString(value);

                            uint fileHandle = mIhost.getReg(0);
                            if (fileHandle < mFiles.Count)
                            {
                                mFiles[(int)fileHandle].Write(text);
                                mIhost.cf = false;
                            }
                            else
                                mIhost.cf = true;
                        } break;

                    //output a float FP number to screen or to open file
                    //r0: file handle (value 1 == screen)
                    //r1:register to output (0-31)
                    //outputs in the same format as the registers view, uses the formatstring "0.###E+0"
                    case 0x7f:
                        {
                            uint reg = mIhost.getReg(1);
                            if (reg > 31) {
                                reportBadRegister(reg, swi_code);
                                return 0;
                            }
                            float value = mIhost.getFPSingleReg(reg);
                            string text = ARMSim.Simulator.VFP.FloatingPointProcessor.FloatToString(value);

                            uint fileHandle = mIhost.getReg(0);
                            if (fileHandle < mFiles.Count) {
                                mFiles[(int)fileHandle].Write(text);
                                mIhost.cf = false;
                            } else {
                                mIhost.cf = true;
                            }
                        } break;

                    default:
                        //if it is an unknown swi code then return 3 clock cycles executed but
                        //do no actual work. This prevents the swi exception from being raised
                        //for swi codes not handled here
                        break;

                }//switch
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(
                    "Exception caught while processing SWI call: 0x" +
                    swi_code.ToString("X2") + " Message: " + ex.Message);
                return 0;
            }
            return 3;
        }//Execute

        protected static void reportBadRegister( uint reg, uint swiNum )
        {
            ARMPluginInterfaces.Utils.OutputDebugString(String.Format(
                "Invalid register specified in SWI 0X{0:x2}: register {1}", swiNum, reg));
        }

        /// <summary>
        /// The name of the extended swi instructions plugin
        /// </summary>
        public string PluginName { get { return "LegacySWIInstructions"; } }

        /// <summary>
        /// And its description
        /// </summary>
        public string PluginDescription { get { return "Legacy SWI extension instructions"; } }

    }//class SWIInstructions
}