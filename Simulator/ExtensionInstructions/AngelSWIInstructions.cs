// AngelSWI.cs


using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ARMPluginInterfaces;

namespace ARMSim.Simulator.ExtensionInstructions
{
    /// <summary>
    /// This class is an ARMSim plugin that handles the instruction
    ///    swi 0x123456
    /// This one instruction mimics the functionality provided by ARM's Angel RDI services.
    /// The services include low-level I/O and a few other actions needed to be provided
    /// by an OS similar to unix.
    /// </summary>
    public class AngelSWIInstructions : IARMPlugin
    {

		// Codes defined in arm-2010q1-188-arm-none-eabi/newlib-2010q1/newlib/libc/sys/arm/swi.h
		public enum AngelSWI {
                None = 0x00,
				Reason_Open = 0x01,
				Reason_Close = 0x02,
				Reason_WriteC = 0x03,
				Reason_Write0 = 0x04,
				Reason_Write = 0x05,
				Reason_Read = 0x06,
				Reason_ReadC = 0x07,
				Reason_IsTTY = 0x09,
				Reason_Seek = 0x0A,
				Reason_FLen = 0x0C,
				Reason_TmpNam = 0x0D,
				Reason_Remove = 0x0E,
				Reason_Rename = 0x0F,
				Reason_Clock = 0x10,
				Reason_Time = 0x11,
				Reason_System = 0x12,
				Reason_Errno = 0x13,
				Reason_GetCmdLine = 0x15,
				Reason_HeapInfo = 0x16,
				Reason_EnterSVC = 0x17,
				Reason_ReportException = 0x18
				};

		// Error codes copied from Unix file errno.h
		public enum Errno {
				NONE = 0,		/* There is no error!! */
				EPERM = 1,		/* Not super-user */
				ENOENT = 2,		/* No such file or directory */
				ESRCH = 3,		/* No such process */
				EINTR = 4,		/* Interrupted system call */
				EIO = 5,		/* I/O error */
				ENXIO = 6,		/* No such device or address */
				E2BIG = 7,		/* Arg list too long */
				ENOEXEC = 8,	/* Exec format error */
				EBADF = 9,		/* Bad file number */
				ECHILD = 10,	/* No children */
				EAGAIN = 11,	/* No more processes */
				ENOMEM = 12,	/* Not enough core */
				EACCES = 13,	/* Permission denied */
				EFAULT = 14,	/* Bad address */
				ENOTBLK = 15,	/* Block device required */
				EBUSY = 16,		/* Mount device busy */
				EEXIST = 17,	/* File exists */
				EXDEV = 18,		/* Cross-device link */
				ENODEV = 19,	/* No such device */
				ENOTDIR = 20,	/* Not a directory */
				EISDIR = 21,	/* Is a directory */
				EINVAL = 22,	/* Invalid argument */
				ENFILE = 23,	/* Too many open files in system */
				EMFILE = 24,	/* Too many open files */
				ENOTTY = 25,	/* Not a typewriter */
				ETXTBSY = 26,	/* Text file busy */
				EFBIG = 27,		/* File too large */
				ENOSPC = 28,	/* No space left on device */
				ESPIPE = 29,	/* Illegal seek */
				EROFS = 30,		/* Read only file system */
				EMLINK = 31,	/* Too many links */
				EPIPE = 32,		/* Broken pipe */
				ENOTSUP = 134	/* Not supported */
		};

        // prefix for temporary file names
        private string TmpDirectory = Path.Combine(Path.GetTempPath(), "ArmSimTmp");

        private IARMHost mIhost;
        private List<ARMSimStream> mFiles = new List<ARMSimStream>();
        private uint mStdioConsole;
        private Errno mErrno = Errno.NONE;
        private System.Diagnostics.Stopwatch timer;
        private uint stdFileNum = 0;

		private uint[] paramBlock = {0, 0, 0, 0};

		private bool fetchParams( int nwords ) {
        	uint r1 = mIhost.getReg(1);  // pointer to parameter block
        	try {
	        	for( int i=0;  i<nwords;  i++ ) {
	        		paramBlock[i] = mIhost.GetMemory(r1, ARMPluginInterfaces.MemorySize.Word);
	        		r1 += 4;
	        	}
			} catch( Exception ) {
				mErrno = Errno.EFAULT;
				return false;
			}
			mErrno = Errno.NONE;
			return true;
		}

        /// <summary>
        /// Called when plugin is first created. Host passes in interface here
        /// </summary>
        /// <param name="ihost"></param>
        public void Init(IARMHost iHost)
        {
            //save the reference to the simulator
            mIhost = iHost;

            //subscribe to the Load event
            mIhost.Load += onLoad;

            timer = System.Diagnostics.Stopwatch.StartNew();  // start the timer
        }//init

        /// <summary>
        /// Called when all the plugins have been created and init.
        /// </summary>
        public void onLoad(object sender, EventArgs e)
        {
            //setup for file handles and file io
            mFiles.RemoveAll(x => true);

            //We only need SWI calls for swi 0x123456
            mIhost.RequestOpCodeRange(0x0f123456, 0x0fffffff, this.Execute);

            //setup callbacks needed
            mIhost.Restart += onRestart;
            mIhost.Unload += onUnload;
        }//onLoad

        static private ARMSimStream CreateInputStream(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return null;
            if (!File.Exists(filename))
                return null;
            StreamReader sr = new StreamReader(File.OpenRead(filename));
            return new InputARMSimFileStream(sr);
        }

        static private ARMSimStream CreateOutputStream(string filename, bool overwrite)
        {
            if (String.IsNullOrEmpty(filename))
                return null;
            ARMSimStream armSimStream = null;
            if (overwrite)
            {
                armSimStream = new OutputARMSimFileStream(File.CreateText(filename));
            }
            else
            {
                armSimStream = new OutputARMSimFileStream(File.AppendText(filename));
            }
            return armSimStream;
        }

        private void SetupStreams()
        {
            closeAllStreams();
            mIhost.CloseStandardConsole(mStdioConsole);

            mStdioConsole = mIhost.CreateStandardConsole("stdin/stdout/stderr");

            ARMPluginInterfaces.Preferences.IGeneralPreferences prefs = mIhost.ARMPreferences.IGeneralPreferences;
            IARMSimArguments args = mIhost.ARMSimArguments;

            ARMSimStream armSimStream = CreateInputStream(args.StdinFileName);
            if(armSimStream == null)
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
            if (armSimStream == null)
                armSimStream = CreateOutputStream(prefs.StderrFileName, prefs.StderrOverwrite);
            if (armSimStream == null)
                armSimStream = new ARMSimConsoleStream(mStdioConsole, mIhost);

            mFiles.Add(armSimStream);  // index = 2
            stdFileNum = 0;
        }//SetupStreams

        public void closeAllStreams()
        {
            //close all streams except standard I/O console streams
            for(int i=0; i<mFiles.Count; i++) {
                ARMSimStream astream = mFiles[i];
                if (astream == null || astream is ARMSimConsoleStream)
                    continue;
                astream.Close();
            }
            mFiles.RemoveAll(x => true);
        }

        /// <summary>
        /// Called when the plugin is unloaded (application is terminating)
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
        /// Called when swi 0x00123456 is executed
        /// </summary>
        /// <param name="op_code">opcode being executed</param>
        /// <returns>number of cycles executed. 0 if none</returns>
        public uint Execute(uint op_code)
        {
            if ((op_code & 0x00ffffff) != 0x123456)
            	return 3;  // 3 cycles for an unhandled swi

            try
            {
            	uint r0 = mIhost.getReg(0);  // reason code
            	
                //and execute based on the reason code.
                switch ((AngelSWI)r0)
                {
					case AngelSWI.Reason_Open:
						r0 = swiOpen();
                        mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Close:
						r0 = swiClose();
                        mIhost.setReg(0, r0);
                        break;
 
 					case AngelSWI.Reason_Write:
						r0 = swiWrite();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Read:
						r0 = swiRead();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_IsTTY:
						r0 = swiIsTTy();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Seek:
						r0 = swiSeek();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_FLen:
						r0 = swiFlen();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Errno:
						mIhost.setReg(0, (uint)mErrno);
                        break;

					case AngelSWI.Reason_Remove:
						r0 = swiRemove();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Rename:
						r0 = swiRename();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Clock:
						r0 = SwiClock();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_Time:
						r0 = SwiTime();
						mIhost.setReg(0, r0);
                        break;

                    case AngelSWI.Reason_HeapInfo:
                        r0 = HeapInfo();
                        mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_TmpNam:
                        r0 = TmpNam();
						mIhost.setReg(0, r0);
                        break;

					case AngelSWI.Reason_WriteC:
					case AngelSWI.Reason_Write0:
					case AngelSWI.Reason_ReadC:
					case AngelSWI.Reason_System:
					case AngelSWI.Reason_GetCmdLine:
					case AngelSWI.Reason_EnterSVC:
						mErrno = Errno.ENOTSUP;
						mIhost.setReg(0, uint.MaxValue);
						break;

                    case AngelSWI.Reason_ReportException:
                        r0 = mIhost.getReg(1);
                        if (r0 != 0x0 && r0 != 0x20026)
                            // 20026 is a normal exit
                            // For convenience, we take 0 as a normal exit too
                            ARMPluginInterfaces.Utils.OutputDebugString(
                                "report exception SWI executed: 0x" + r0.ToString("x8"));
                        mIhost.HaltSimulation();
                        mIhost.GPR[15] -= 4;//todo thumb mode?
                        this.closeAllStreams();
                        break;

                    default:
                        //if it is an unknown reason then return 3 clock cycles executed but
                        //do no actual work. This prevents the swi exception from being raised
                        //for swi codes not handled here
                        ARMPluginInterfaces.Utils.OutputDebugString("unhandled Angel SWI code: " + r0.ToString());
                        break;

                }//switch
            }//try
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while processing SWI call, message:" + ex.Message);
                return 0;
            }
            return 3;
        }//Execute

		// Open a file
		private uint swiOpen() {
			// Params:  filename, length of filename, mode
			// Result:  r0 = file handle
			if (!fetchParams(3))
				return errorCode(Errno.EFAULT);

			try {
	        	string str = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, paramBlock[0], paramBlock[1]);
				uint mode = paramBlock[2];
				bool binary = false;
				if ((mode & 1) != 0) {
					binary = true;
					mode &= 0xffffffe;
				}
                if (binary)
                    ARMPluginInterfaces.Utils.OutputDebugString("binary mode files unsupported");

				mErrno = Errno.NONE;
	            ARMSimStream astream = null;
				if (String.IsNullOrEmpty(str) || str == ":tt") {
                    // First three uses of this filename map to the standard streams
                    // stdin, stdout, & stderr which have already been opened
                    if (stdFileNum <= 2)
                        return stdFileNum++;
                    // We want additional console windowss??
                    uint stdHandle = mIhost.CreateStandardConsole(str);
                    astream = new ARMSimConsoleStream(stdHandle, mIhost);
                }
                else
                {
                    string currDir = null;
                    if (mIhost.UserDirectory != null)
                    {
                        currDir = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(mIhost.UserDirectory);
                    }
                    switch (mode)
                    {
                        case 0:  // read
                            astream = new InputARMSimFileStream(new StreamReader(File.OpenRead(str)));
                            break;
                        case 2:  // read-write mode
                            mErrno = Errno.ENOTSUP;  // unsupported mode
                            break;
                        case 4:  // write (with create or truncate)
                            astream = new OutputARMSimFileStream(new StreamWriter(File.Open(str, FileMode.OpenOrCreate)));
                            break;
                        case 8:  // write in append mode
                            astream = new OutputARMSimFileStream(new StreamWriter(File.Open(str, FileMode.Append)));
                            break;
                        default:
                            mErrno = Errno.EINVAL;  // invalid mode
                            break;
                    }
                    if (currDir != null)
                        Directory.SetCurrentDirectory(currDir);
                    if (mErrno != Errno.NONE)
                        return uint.MaxValue;
                }
	            int fileHandle = 0;
	            // search for the first available handle
	            while(fileHandle < mFiles.Count) {
	            	if (mFiles[fileHandle] == null) {
	            		mFiles[fileHandle] = astream;
	            		return (uint)fileHandle;
	            	}
	            	fileHandle++;
	            }
				mFiles.Add(astream);
	            return (uint)fileHandle;
            } catch( Exception ) {
            	return errorCode(Errno.EIO);
            }
		}

		// Close a file
		private uint swiClose() {
			// Params:  file handle
			if (!fetchParams(1))
				return errorCode(Errno.EFAULT);
			int fileHandle = (int)paramBlock[0];
            if (fileHandle < mFiles.Count) {
                mFiles[fileHandle].Close();
                mFiles[fileHandle] = null;
                mErrno = Errno.NONE;
                return 0;
            }
            return errorCode(Errno.EBADF);
		}

		// Write to a file
		private uint swiWrite() {
			// Params:  file handle, address of buffer, # of bytes to write
			// Result:  r0 = # of bytes NOT written
			if (!fetchParams(3))
				return errorCode(Errno.EFAULT);
			//uint fileHandle = paramBlock[0];
			uint addr = paramBlock[1];
			uint nbytes = paramBlock[2];
            ARMSimStream acstream = getStream(paramBlock[0]);
            if (!(acstream is ARMSimConsoleStream) && !(acstream is OutputARMSimFileStream)) {
				mErrno = Errno.EBADF;
				return nbytes;
			}
			while(nbytes != 0) {
                uint ch;
				try {
					ch = mIhost.GetMemory(addr, ARMPluginInterfaces.MemorySize.Byte);
				} catch( Exception ) {
					mErrno = Errno.EFAULT;  break;
				}
				try {
					acstream.Write((char)ch);
				} catch( Exception ) {
					mErrno = Errno.EIO;  break;
				}
				addr++;
                nbytes--;
            }
			return nbytes;
		}

		// Read from a file
		private uint swiRead() {
			// Params:  file handle, address of buffer, # of bytes to read
			// Result:  r0 = # of bytes NOT read
			if (!fetchParams(3))
				return uint.MaxValue;
			uint addr = paramBlock[1];
			uint nbytes = paramBlock[2];
            ARMSimStream acstream = getStream(paramBlock[0]);
            if (!(acstream is ARMSimConsoleStream) && !(acstream is InputARMSimFileStream))
            {
                mErrno = Errno.EBADF;
                return nbytes;
            }

			while(nbytes != 0) {
				int ch;
				try {
					ch = acstream.Read();
					if (ch < 0) {
						mErrno = Errno.EIO;  break;
					}
				} catch( Exception ) {
					mErrno = Errno.EIO;  break;
				}
                try {
                    mIhost.SetMemory(addr, ARMPluginInterfaces.MemorySize.Byte, (uint)ch);
                }
                catch(MemoryAccessException)
                {
                    mErrno = Errno.EFAULT; break;
                }
				addr++;
                nbytes--;
            }
			return nbytes;
		}

		private uint swiIsTTy() {
			// Params:  file handle
			// Result:  r0 = 1 if handle refers to a tty, 0 otherwise
			if (!fetchParams(1))
				return errorCode(Errno.EFAULT);
			ARMSimStream astream = getStream(paramBlock[0]);
            if (astream == null)
				return uint.MaxValue;
			return (uint)((astream is ARMSimConsoleStream)? 1 : 0);
		}

		// Seek to an absolute file position
		private uint swiSeek() {
			// Params:  file handle, absolute file position in bytes
			// Result:  r0 = -1 if error, 0 otherwise
			if (!fetchParams(2))
				return errorCode(Errno.EFAULT);
			ARMSimStream astream = getStream(paramBlock[0]);
            if (astream == null)
				return uint.MaxValue;
            InputARMSimFileStream iastream = astream as InputARMSimFileStream;
			if (iastream != null) {
				try {
                    Stream s = iastream.Stream.BaseStream;
					if (s.CanSeek) {
						s.Position = (long)paramBlock[1];
						return 0;
					}
				} catch( Exception ) {
					return errorCode(Errno.EIO);
				}
			}
			return errorCode(Errno.ENOTSUP);
		}

		// Get length of file
		private uint swiFlen() {
			// Params:  file handle
			// Result:  r0 = length of file, -1 if error
			if (!fetchParams(1))
				return errorCode(Errno.EFAULT);
			ARMSimStream astream = getStream(paramBlock[0]);
            if (astream == null)
				return uint.MaxValue;
            try {
                long r = astream.Length();
                if (r >= 0)
                    return (uint)r;
            } catch( Exception ) {
		    }
            if (astream is ARMSimConsoleStream) // another kludge for Angel RDI
                return 0;
            return errorCode(Errno.ENOTSUP);
		}

		// Remove a file
		private uint swiRemove() {
			// Params:  filename, length of filename
			// Result:  r0 = 0 if success, -1 if error
			if (!fetchParams(2))
				return errorCode(Errno.EFAULT);
			try {
				string str = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, paramBlock[0], paramBlock[1]);
                string currDir = null;
                if (mIhost.UserDirectory != null) {
                	currDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(mIhost.UserDirectory);
                }
				File.Delete(str);
				if (currDir != null)
					Directory.SetCurrentDirectory(currDir);
			} catch( Exception ) {
				return errorCode(Errno.EIO);
			}
			return 0;
		}

		// Rename a file from filename1 to filename2
		private uint swiRename() {
			// Params:  filename1, length of filename1, filename2, length of filename2
			// Result:  r0 = 0 if success, -1 if error
			if (!fetchParams(4))
				return errorCode(Errno.EFAULT);
			try {
				string str1 = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, paramBlock[0], paramBlock[1]);
				string str2 = ARMPluginInterfaces.Utils.loadStringFromMemory(mIhost, paramBlock[2], paramBlock[3]);
                string currDir = null;
                if (mIhost.UserDirectory != null) {
                	currDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(mIhost.UserDirectory);
                }
				File.Move(str1,str2);
				if (currDir != null)
					Directory.SetCurrentDirectory(currDir);
			} catch( Exception ) {
				return errorCode(Errno.EIO);
			}
			return 0;
		}

        // Generates a temporary file name
        private uint TmpNam()
        {
            // Params:  pointer to buffer, file number (in 0 to 255 range), buffer length
            // Result:  r0 = 0 if success, -1 if error
            if (!fetchParams(3))
                return errorCode(Errno.EFAULT);
            try
            {
                uint addr = paramBlock[0];
                uint fnum = paramBlock[1];
                uint buflen = paramBlock[2];
                if (fnum > 255)
                {
                    return errorCode(Errno.EINVAL);
                }
                string tmpname = String.Format("{0}{1}", TmpDirectory, fnum);
                if (tmpname.Length >= buflen) {
                    return errorCode(Errno.EINVAL);
                }
                foreach( char ch in tmpname ) {
					mIhost.SetMemory(addr++, ARMPluginInterfaces.MemorySize.Byte, (uint)ch);
                }
                mIhost.SetMemory(addr, ARMPluginInterfaces.MemorySize.Byte, 0);
            }
            catch (Exception)
            {
               return errorCode(Errno.EIO);
            }
            return 0;
        }

		// Returns user time in 1/100 second units
		private uint SwiClock() {
			long ticks = timer.ElapsedTicks;
            return (uint)(ticks * 100 / System.Diagnostics.Stopwatch.Frequency);
		}

		// Returns # seconds since Unix epoch
		static private uint SwiTime() {
			DateTime epoch = new DateTime(1970, 1, 1);
			DateTime now = DateTime.Now;
			TimeSpan diff = now - epoch;
			return (uint)(diff.Ticks / 10000000);
		}

        // Provide info about stack and heap areas
        private uint HeapInfo()
        {
            // Parameter: r1 is a pointer to a 4 word block of memory to receive the results
            try
            {
                uint addr = mIhost.getReg(1);
                mIhost.SetMemory(addr, ARMPluginInterfaces.MemorySize.Word, mIhost.GetMemoryParameter("HeapStart"));
                mIhost.SetMemory(addr + 4, ARMPluginInterfaces.MemorySize.Word, mIhost.GetMemoryParameter("HeapEnd"));
                mIhost.SetMemory(addr + 8, ARMPluginInterfaces.MemorySize.Word, mIhost.GetMemoryParameter("StackStart"));
                mIhost.SetMemory(addr + 12, ARMPluginInterfaces.MemorySize.Word, mIhost.GetMemoryParameter("StackEnd"));
            }
            catch (MemoryAccessException)
            {
                return errorCode(Errno.EFAULT);
            }
            return 0;
        }

		private ARMSimStream getStream( uint fileHandle ) {
            ARMSimStream result = null;
            if (fileHandle < mFiles.Count)
            	result = mFiles[(int)fileHandle];
            mErrno = (result == null)? Errno.EBADF : Errno.NONE;
			return result;
		}

        /// <summary>
        /// The name of the extended swi instructions plugin
        /// </summary>
        public string PluginName { get { return "AngelSWIInstructions"; } }

        /// <summary>
        /// And its description
        /// </summary>
        public string PluginDescription { get { return "Angel RDI SWI extension instructions"; } }

        private uint errorCode(Errno code)
        {
            mErrno = code;
            return uint.MaxValue;
        }

    }//class SWIInstructionsS
}
