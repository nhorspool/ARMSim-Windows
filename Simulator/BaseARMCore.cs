#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;
using ARMSim.Preferences;
using ARMSim.Simulator.VFP;

namespace ARMSim.Simulator
{
    /// <summary>
    /// This class is the core engine that interprets and executes ARM instructions.
    /// </summary>
    public abstract partial class BaseARMCore
    {

		public abstract uint ExecuteInstruction(uint opCode, out bool swiInstruction);

        public bool HaltRequested = false;

        /// <summary>
        /// Get data from main memory.
        /// This is abstract so that the Application level can handle cache and plugin logic.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public abstract uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms);

        /// <summary>
        /// Write data to main memory.
        /// This is abstract so that the Application level can handle cache and plugin logic.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <param name="data"></param>
        public abstract void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data);

        /// <summary>
        /// call this function if the simulation stops either by swi 0x11 instruction or fatal error.
        /// </summary>
        public abstract void HaltSimulation();

        /// <summary>
        /// Write strings to the ARMSim outputview console.
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="parms"></param>
        public abstract void OutputConsoleString(string fmt, params object[] parms);

        /// <summary>
        /// If an unknown opcode is encountered, this function is called.
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public abstract uint UnknownOpCode(uint opCode);

        /// <summary>
        /// When cpu cycles are executed, this function is called to notify any interested modules.
        /// </summary>
        /// <param name="count"></param>
        public abstract void HandleCycles(ushort count);

        /// <summary>
        /// Test if a given memory address and size are within the bounds of main memory.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public abstract bool InRange(uint address, ARMPluginInterfaces.MemorySize ms);

        /// <summary>
        /// Restart the simulation engine.
        /// </summary>
        public abstract void Restart();

        /// <summary>
        /// Fetch memory from the main memory module and bypass the cache system.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public abstract uint GetMemoryNoSideEffect(uint address, ARMPluginInterfaces.MemorySize ms);

        //current instruction and cycle count
        public uint InstructionCount { get; set; }
        public uint CycleCount { get; set; }

        /// <summary>
        /// This enum specifies the exception types of the ARM processor.
        /// Each is a separate bit so we can represent more than one exception
        /// being raised during a single instruction execution.
        /// </summary>
        [Flags]
        public enum ARMExceptions
        {
            None = 0x0,
            Reset = 0x01,
            UndefinedInstruction = 0x02,
            SoftwareInterrupt = 0x04,
            PreFetchAbort = 0x08,
            DataAbort = 0x10,
            IRQ = 0x20,
            FIQ = 0x40
        }

        //the cpu CPSR register
        private CPSR mCPSR;

        //the ARM general purpose registers(r0-r15)
        protected GeneralPurposeRegisters mGPR;

        //the floating point processor.
        protected FloatingPointProcessor mFPP;

        //the single block of RAM that the processor has control over
        protected MemoryBlock mMemoryBlock;

        //the data and instruction caches.
        protected Cache.DataCache mL1DataCache;
        protected Cache.L1Cache mL1InstructionCache;

        /// <summary>
        /// Flag indicates if the cpu exception vectors are located in
        /// "high" or "low" memory. Todo - this should be a preference setting
        /// if they are low they start at 0x00000000
        /// if they are high they start at 0xffff0000
        /// </summary>
        public bool HighVectors { get; set; }

        //This uint is set when the engine wants to raise an exception.
        //Each bit in the uint represents an exception. ( ARMExceptions enum defined above )
        //This is kept as a single uint for efficiency.
        private uint mRequestedException;

        /// <summary>
        /// Flag indicates if unaligned memory access should stop the simulation.
        /// If false, unaligned access will be fixed-up
        /// </summary>
        public bool TrapUnalignedMemoryAccess { get; set; }
        public bool ProtectTextArea { get; set; }

        protected BaseARMCore()
        {
            this.TrapUnalignedMemoryAccess = true;

            mCPSR = new CPSR();
            mGPR = new GeneralPurposeRegisters(mCPSR);
            mFPP = new FloatingPointProcessor(this);
            this.Reset();
        }//Jimulator ctor

        /// <summary>
        /// Performs a reset on the CPU. Reset all devices back to their initial state.
        /// </summary>
        public void Reset()
        {
            //clear any pending exception requests
            mRequestedException = 0;

            //current instruction/cycle count back to 0
            this.InstructionCount = 0;
            this.CycleCount = 0;

            mGPR.Reset();       //zero all the cpu registers
            mCPSR.Reset();      //reset the cpsr
            mFPP.reset();       //reset the vfp(vector floating point processor)

            //reset the caches back to their initial sate, 0's all cache memory
            this.ResetCache();

            this.HaltRequested = false;

        }//reset

        public CPSR CPSR { get { return mCPSR; } }
        public ARMPluginInterfaces.IGPR GPR { get { return mGPR; } }
        //public GeneralPurposeRegisters GPR { get { return _gpr; } }
        public FloatingPointRegisters FPR { get { return mFPP.FPR; } }
        public FloatingPointProcessor FPP { get { return mFPP; } }

        /// <summary>
        /// Purge the caches if they exist. This will sync the contents on the cache
        /// to main memory.
        /// </summary>
        public void PurgeCache()
        {
            if (mL1DataCache != null)
                mL1DataCache.Purge();
            if (mL1InstructionCache != null)
                mL1InstructionCache.Purge();
        }//PurgeCache

        /// <summary>
        /// Reset the caches back to their original configured state.
        /// </summary>
        public void ResetCache()
        {
            if (mL1DataCache != null)
                mL1DataCache.Reset();
            if (mL1InstructionCache != null)
                mL1InstructionCache.Reset();
        }//ResetCache

        /// <summary>
        /// Define the caches according to the cache preferences.
        /// </summary>
        /// <param name="preferences"></param>
        public void DefineCache(CachePreferences preferences)
        {
            mL1DataCache = new Cache.DataCache(mMemoryBlock, preferences.DataCachePreferences);
            if (preferences.UnifiedCache)
                mL1InstructionCache = mL1DataCache;
            else
                mL1InstructionCache = new Cache.L1Cache(mMemoryBlock, preferences.InstructionCachePreferences);

        }//DefineCache

        /// <summary>
        /// malloc obtains from a region of memory (the heap) which
        /// (a) grows up from the end of the program if the stack grows down,
        /// (b) grows down from the end of the memory block if the stack grows up.
        /// In either case, memory is exhausted when the heapPointer collides with
        /// the stack.
        /// </summary>
        uint heapPointer;

        /// <summary>
        /// Define the main memory block according to the preferences.
        /// </summary>
        /// <param name="preferences"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public uint DefineMemory(SimulatorPreferences preferences, int len)
        {
            //extract start of memory address from preferences
            uint address = preferences.MemoryStart;
            uint stackPointer;

            uint programSizeBytes = (uint)((((len * 4) + 1023) / 1024) * 1024);
            uint stackSizeBytes = preferences.StackAreaSize * 1024;
            uint heapSizeBytes = preferences.HeapAreaSize * 1024;
            if (preferences.StackGrowsDown)
            {
                stackPointer = (uint)(address + programSizeBytes + heapSizeBytes + stackSizeBytes);
                heapPointer = (uint)(address + programSizeBytes);
            }
            else
            {
                stackPointer = (uint)(address + programSizeBytes);
                heapPointer = (uint)(address + programSizeBytes + heapSizeBytes + stackSizeBytes);
            }

            //compute size of main memory in words
            uint sizeWords = (uint)((programSizeBytes + stackSizeBytes + heapSizeBytes) / 4);

            //init the main memory block
            mMemoryBlock = new MemoryBlock(address, sizeWords, preferences);
            mMemoryBlock.HeapStart = heapPointer;
            mMemoryBlock.StackStart = stackPointer;
            if (preferences.StackGrowsDown)
            {
                mMemoryBlock.HeapEnd = heapPointer + heapSizeBytes;
                mMemoryBlock.StackEnd = stackPointer - stackSizeBytes;
            }
            else
            {
                mMemoryBlock.HeapEnd = heapPointer - heapSizeBytes;
                mMemoryBlock.StackEnd = stackPointer + stackSizeBytes;
            }

            return stackPointer;
        }//DefineMemory

        public Cache.DataCache DataCacheMemory { get { return mL1DataCache; } }
        public Cache.L1Cache InstructionCacheMemory { get { return mL1InstructionCache; } }
        public ARMPluginInterfaces.IMemoryBlock MainMemory { get { return mMemoryBlock; } }



        /// <summary>
        /// Determine if the current opcode pointed to by the PC is a BL instruction.
        /// </summary>
        /// <returns></returns>
        public bool isCurrentOpCodeBL()
        {
            uint opCode;
            //fetch the current opcode, make sure the cache is not affected as this is an artificial test
            if (this.CPSR.tf) // is it Thumb mode
            {
                opCode = this.GetMemoryNoSideEffect(mGPR.PC, ARMPluginInterfaces.MemorySize.HalfWord);
                if ((opCode & 0xe000) == 0xe000 && (opCode & 0x1800) != 0x0)
                    return true;  // a bl or blx(1) instruction
                return (opCode & 0xff80) == 0x4780; // blx(2) instruction
            }
            else // it's normal ARM mode
            {
                opCode = this.GetMemoryNoSideEffect(mGPR.PC, ARMPluginInterfaces.MemorySize.Word);
                return ((opCode & 0x0f000000) == 0x0b000000 || (opCode & 0x0ff000f0) == 0x01200030);
            }
        }
        /// <summary>
        /// Determine if the current opcode pointed to by the PC is a swi which requested a halt
        /// </summary>
        /// <returns></returns>
        public bool isCurrentOpCodeSWIExit()
        {
            if (!this.InRange(mGPR.PC,ARMPluginInterfaces.MemorySize.HalfWord))
                return true;  // encourage a halt in this situation!
            return HaltRequested;
        }

        private void pcOutofRange( uint oldPC )
        {
            ReportRuntimeError(oldPC, "Control transfer to illegal address {0:X8}", mGPR.PC);
            HaltSimulation();
            return;
        }

        public void RequestException(ARMExceptions exception)
        {
            mRequestedException |= (uint)exception;
        }

        /// <summary>
        /// Allocate size bytes from the main memory heap.
        /// </summary>
        /// <param name="size">size to allocate in bytes</param>
        /// <returns>address of allocated block. 0 if failed.</returns>
        public uint malloc(uint size)
        {
            return mMemoryBlock.malloc(size);
        }

        /// <summary>
        /// Clear the main memory heap.
        /// </summary>
        public void HeapClear()
        {
            mMemoryBlock.HeapClear();
        }

        /// <summary>
        /// Switches the CPU into an exception state. Each of the 7 exceptions that can be
        /// raised are unique. The CPU is switched into a new cpu mode, and interrupts
        /// may be disabled. The PC is set to the exception vector.
        /// Vector address is initially set to the "low" values. However of the HighVectors flag
        /// is set, then ARM exception vectors start at 0xffff0000
        /// </summary>
        /// <param name="armException"></param>
        private void SetExceptionState(ARMExceptions armException)
        {
            uint newPC;
            switch (armException)
            {
                case ARMExceptions.Reset:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.Supervisor);
                    mCPSR.IRQDisable = true;
                    mCPSR.FIQDisable = true;
                    newPC = 0x00000000;
                    break;

                case ARMExceptions.UndefinedInstruction:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.Undefined);
                    mCPSR.IRQDisable = true;
                    newPC = 0x00000004;
                    break;

                case ARMExceptions.SoftwareInterrupt:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.Supervisor);
                    mCPSR.IRQDisable = true;
                    newPC = 0x00000008;
                    break;

                case ARMExceptions.PreFetchAbort:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.Abort);
                    mCPSR.IRQDisable = true;
                    newPC = 0x0000000c;
                    break;

                case ARMExceptions.DataAbort:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.Abort);
                    mCPSR.IRQDisable = true;
                    newPC = 0x00000010;
                    break;

                case ARMExceptions.IRQ:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.IRQ);
                    mCPSR.IRQDisable = true;
                    newPC = 0x00000018;
                    break;

                case ARMExceptions.FIQ:
                    mCPSR.SwitchCPUMode(CPSR.CPUModeEnum.FIQ);
                    mCPSR.IRQDisable = true;
                    mCPSR.FIQDisable = true;
                    newPC = 0x0000001c;
                    break;
                default: return;
            }//switch

            //save the PC into the new modes banked lr
            mGPR.LR = mGPR.PC;

            //if the "high" vectors mode is on, then set the top 16bits to a 1 of the new pc.
            mGPR.PC = this.HighVectors ? (newPC | 0xffff0000) : newPC;

            //always process the exception in ARM mode. When the exception returns and reloads
            //the cpsr from the scpsr, thumb mode will be reenabled if that was the starting mode.
            mCPSR.tf = false;

        }//SetExceptionState

        protected void ReportRuntimeError(uint loc, string format, params object[] args)
        {
            string where = loc!=uint.MaxValue? String.Format("Location {0:X}: ", loc) : "Unknown location: ";
            string str = where + String.Format(format, args) + "\n";
            OutputConsoleString(str);
            ARMPluginInterfaces.Utils.OutputDebugString(str);
        }

        /// <summary>
        /// Execute the next instruction pointed to by the PC.
        /// Can be in either ARM or Thumb mode.
        /// </summary>
        public void Execute()
        {
            uint pcBefore = mGPR.PC;  // PC before op is executed
            try {
                ARMPluginInterfaces.MemorySize opcodeSize = mCPSR.tf ? ARMPluginInterfaces.MemorySize.HalfWord : ARMPluginInterfaces.MemorySize.Word;

                //this should never happen as the PC is checked after the opcode is executed,
                //but just being careful ...
                if (!this.InRange(pcBefore, opcodeSize))
                {
                    pcOutofRange(uint.MaxValue);  // we don't know the old address??
                    return;
                }

                //track the number of instructions executed.
                ++InstructionCount;

                //get the opcode and increment the PC. Fetch through the cache system if available.
                uint opcode = this.mL1InstructionCache.GetMemory(pcBefore, opcodeSize);

                //increment the PC by the current opcode size (ARM or Thumb mode).
                mGPR.PC = pcBefore + (uint)opcodeSize;

                uint cycleCount;
                bool swiInstruction;

                try
                {
					cycleCount = ExecuteInstruction(opcode, out swiInstruction);
                }
                catch (UnalignedAccessException e)
                {
                    ReportRuntimeError(pcBefore, "{0}", e.Message);
                    HaltSimulation();
                    mGPR.PC = pcBefore;  // undo any advance of the PC
                    return;
                }
                catch (MemoryAccessException e)
                {
                    ReportRuntimeError(pcBefore, "{0}", e.Message);
                    HaltSimulation();
                    mGPR.PC = pcBefore;  // undo any advance of the PC
                    return;
                }
                // a cycle count of zero indicates an unknown instruction (including an swi instruction)
                if (cycleCount == 0)
                {
                    //let plugins have first crack at the unknown instruction.
                    //a return of 0 indicates it cannot handle it.
                    cycleCount = UnknownOpCode(opcode);
                }//if

                //if the cycle count is still 0 and it's an swi instruction, it wasn't handled by a plugin
                //invoke an ARM swi exception
                if (cycleCount == 0 && swiInstruction)
                {
                    this.RequestException(ARMExceptions.SoftwareInterrupt);
                }//if
                //otherwise its an unknown instruction that is not an swi instruction
                //so raise an ARM undefined instruction exception.
                else if (cycleCount == 0)
                {
                    this.RequestException(ARMExceptions.UndefinedInstruction);
                }//else if

                //if we actually expended cycles, let the plugins know and update local cycle count
                if (cycleCount > 0)
                {
                    CycleCount += cycleCount;
                    //todo - check overflow
                    HandleCycles((ushort)cycleCount);
                }//if

                //test the PC against valid memory. If it is not within the memory block, we have a problem
                if (!this.InRange(mGPR.PC, opcodeSize))
                {
                    pcOutofRange(pcBefore);
                }//if

                //check if any exceptions have been raised.
                //it is assumed that only 1 exception can be raised in a single instruction cycle.
                if (mRequestedException != 0)
                {
                    if ((mRequestedException & (uint)ARMExceptions.SoftwareInterrupt) != 0)
                    {
                        SetExceptionState(ARMExceptions.SoftwareInterrupt);
                    }//if
                    //FIQ has highest priority
                    else if (((mRequestedException & (uint)ARMExceptions.FIQ) != 0) && !mCPSR.FIQDisable)
                    {
                        SetExceptionState(ARMExceptions.FIQ);
                    }
                    else if (((mRequestedException & (uint)ARMExceptions.IRQ) != 0) && !mCPSR.IRQDisable)
                    {
                        SetExceptionState(ARMExceptions.IRQ);
                    }
                    else if ((mRequestedException & (uint)ARMExceptions.UndefinedInstruction) != 0)
                    {
                        SetExceptionState(ARMExceptions.UndefinedInstruction);
                    }
                    else if ((mRequestedException & (uint)ARMExceptions.Reset) != 0)
                    {
                        SetExceptionState(ARMExceptions.Reset);
                    }
                    mRequestedException = 0;
                }//if

                //test the PC against valid memory. If it is not within the memory block, we have a problem
                if (!this.InRange(mGPR.PC, opcodeSize))
                {
                    ARMExceptions possibleReason =  interruptKind(mGPR.PC);
                    if (possibleReason != ARMExceptions.None)
                    {
                        if (possibleReason == ARMExceptions.SoftwareInterrupt)
                            ReportRuntimeError(pcBefore, "Unimplemented SWI code: (0x{0:X6})\n" +
                                "[Check File/Preferences/Plugins to see which SWI sets have been enabled]",
                                opcode & 0xFFFFFF);
                        else
                            ReportRuntimeError(pcBefore, "Unhandled interrupt of kind {0}",
                                interruptKind(mGPR.PC));
                        HaltSimulation();
                        mGPR.PC = pcBefore; // leave PC stuck on the interrupting instruction
                    } else
                        pcOutofRange(pcBefore);
                }

            }//try
            catch (ARMPluginInterfaces.MemoryAccessException ex)
            {
               ReportRuntimeError(pcBefore, "Attempt to access memory out of valid range: 0x{0:X8}",
                    ex.Address);
                HaltSimulation();
                mGPR.PC = pcBefore; // leave PC stuck on the bad instruction
            }//catch
        }//Execute

        static private ARMExceptions interruptKind(uint loc)
        {
            switch (loc) {
            case 0x00: return ARMExceptions.Reset;
            case 0x04: return ARMExceptions.UndefinedInstruction;
            case 0x08: return ARMExceptions.SoftwareInterrupt;
            case 0x0C: return ARMExceptions.PreFetchAbort;
            case 0x10: return ARMExceptions.DataAbort;
            case 0x18: return ARMExceptions.IRQ;
            case 0x1c: return ARMExceptions.FIQ;
            }
            return ARMExceptions.None;
        }
    }//class Jimulator
}