using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
//required for Panel and Size objects
using System.Windows.Forms;
using System.Drawing;

namespace ARMPluginInterfaces
{
    public class CyclesEventArgs : EventArgs
    {
        public int CycleCount { get; set; }
        public CyclesEventArgs(int count)
        {
            this.CycleCount = count;
        }
    }

    // These delegates define the callback that can be made to a plugin.
    // They are subscribed to by the plugin when the init method is called.

    /// <summary>
    /// This callback is used when declaring additional instructions to the parser.
    /// When the parser scan an extension instruction, this delegate is called with
    /// the scanned operands. This lets the plugin form the final opcode.
    /// </summary>
    /// <param name="baseCode"></param>
    /// <param name="operands"></param>
    /// <returns></returns>
    public delegate uint pluginFormOpCodeEventHandler(uint baseCode, params uint[] operands);

    /// <summary>
    /// This delegate defines the callback when an instruction is encountered by the
    /// simulator that has been overridden by a plugin
    /// </summary>
    /// <param name="opcode"></param>
    /// <returns></returns>
    public delegate uint pluginInstructionExecuteEventHandler(uint opCode);

    /// <summary>
    /// This delegate defines the callback when the simulator performs a memory read and the address
    /// is one reserved by a plugin.
    /// </summary>
    /// <param name="re">
    /// Defines the arguments to the read operation.
    /// </param>
    /// <returns>
    /// The value read
    /// </returns>
    public delegate uint pluginMemoryAccessReadEventHandler(object sender, MemoryAccessReadEventArgs re);

    /// <summary>
    /// This delegate defines the callback when the simulator performs a memory read and the address
    /// is one reserved by a plugin.
    /// </summary>
    /// <param name="we">
    /// Defines the arguments to the write operation.
    /// </param>
    public delegate void pluginMemoryAccessWriteEventHandler(object sender, MemoryAccessWriteEventArgs we);

    /// <summary>
    /// This delegate defines a callback when a view wishes to output some text to the OutputView
    /// </summary>
    /// <param name="c"></param>
    public delegate void CharacterOutputEventHandler(char c);

    /// <summary>
    /// This enum defines the three types of memory access sizes.
    /// </summary>
    public enum MemorySize
    {
        None = 0,
        /// <summary>byte size(1 byte)</summary>
        Byte = 1,
        /// <summary>half word size(2 bytes)</summary>
        HalfWord = 2,
        /// <summary>word size(4 bytes)</summary>
        Word = 4
    }

    /// <summary>
    /// This class represetns the read arguments for a memory read.
    /// </summary>
    public class MemoryAccessReadEventArgs : EventArgs
    {
        /// <summary>
        /// MemoryAccessReadEventArgs ctor
        /// </summary>
        /// <param name="address">address being read</param>
        /// <param name="size">read size</param>
        public MemoryAccessReadEventArgs(uint address, MemorySize size)
        {
            Address = address;
            Size = size;
        }
        /// <summary>Returns address</summary>
        public uint Address { get; private set; }
        /// <summary>Returns size</summary>
        public MemorySize Size { get; private set; }
    }

    /// <summary>
    /// This class represetns the write arguments for a memory write.
    /// Derives from the read version, so just add the value to write.
    /// </summary>
    public class MemoryAccessWriteEventArgs : MemoryAccessReadEventArgs
    {

        /// <summary>
        /// MemoryAccessWriteArgs ctor
        /// </summary>
        /// <param name="address">address being written</param>
        /// <param name="size">memory size</param>
        /// <param name="value">data value being written</param>
        public MemoryAccessWriteEventArgs(uint address, MemorySize size, uint value)
            : base(address, size)
        {
            Value = value;
        }
        /// <summary>Returns value being written</summary>
        public uint Value { get; private set; }
    }//class MemoryAccessWriteArgs

    /// <summary>
    /// This interface is given to the plugins to use to make calls back into the simulator.
    /// It is initially passed into the plugin via the init method.
    /// </summary>
    public interface IARMHost
    {
        /// <summary>
        /// get the value of a general purpose register(r0-r15)
        /// </summary>
        /// <param name="reg">register number to get</param>
        /// <returns></returns>
        uint getReg(uint reg);
        IGPR GPR { get; }

        /// <summary>
        /// Sets the value of a general purpose register
        /// </summary>
        /// <param name="reg">register to set</param>
        /// <param name="data"></param>
        void setReg(uint reg, uint data);

        /// <summary>
        /// Get a floating point double value. (64 bit)
        /// </summary>
        /// <param name="reg">Valid registers are 0-15</param>
        /// <returns></returns>
        double getFPDoubleReg(uint reg);

        /// <summary>
        /// Get a floating point float value. (32 bit)
        /// </summary>
        /// <param name="reg">Valid registers are 0-31</param>
        /// <returns></returns>
        float getFPSingleReg(uint reg);

        /// <summary>
        /// Put a floating point double value. (64 bit)
        /// </summary>
        /// <param name="reg">Valid registers are 0-15</param>
        /// <returns></returns>
        void setFPDoubleReg(uint reg, double value);

        /// <summary>
        /// Put a floating point float value. (32 bit)
        /// </summary>
        /// <param name="reg">Valid registers are 0-31</param>
        /// <returns></returns>
        void setFPSingleReg(uint reg, float value);

        void OutputConsoleString(string fmt, params object[] parms);

        /// <summary>
        /// Read from mainmemory.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">memory size</param>
        /// <returns></returns>
        uint GetMemory(uint address, MemorySize ms);

        /// <summary>
        /// Write to mainmemory
        /// </summary>
        /// <param name="address">address to write</param>
        /// <param name="ms">memory size</param>
        /// <param name="data">data to write</param>
        void SetMemory(uint address, MemorySize ms, uint data);

        /// <summary>
        /// Calling this function causes the simulator to stop executing the program.
        /// Same as "swi 0x11" instruction.
        /// </summary>
        void HaltSimulation();

        /// <summary>
        /// Access zero flag of cpsr
        /// </summary>
        bool zf { get; set; }

        /// <summary>
        /// Access carry flag of cpsr
        /// </summary>
        bool cf { get; set; }

        /// <summary>
        /// Access negative flag of cpsr
        /// </summary>
        bool nf { get; set; }

        /// <summary>
        /// Access overflow flag of cpsr
        /// </summary>
        bool vf { get; set; }

        /// <summary>
        /// Access the cpsr flags value
        /// </summary>
        uint Flags { get; }

        /// <summary>
        /// Allocate memory from mainmemory heap.
        /// </summary>
        /// <param name="size">size to allocate in bytes</param>
        /// <returns>address of block. 0 if failed.</returns>
        uint Malloc(uint size);

        /// <summary>
        /// Clear the heap and reset the heap pointer to empty.
        /// </summary>
        void HeapClear();

        /// <summary>
        /// Interrogate the memory to obtain properties such as stack start,
        /// data start, etc.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        uint GetMemoryParameter(string propertyName);

        /// <summary>
        /// Returns the current directory. This is defined as to the directory of where
        /// the user program loaded from.
        /// </summary>
        string UserDirectory { get; }

        /// <summary>
        /// Plugins call this to request an area in the user interface.
        /// </summary>
        /// <returns></returns>
        Panel RequestPanel(string title);

        /// <summary>
        /// Plugins call this to request an area of memory be reserved for itself and a delegate to be called
        /// when that memory is accessed.
        /// </summary>
        /// <param name="baseAddress">base address of memory block</param>
        /// <param name="mask">mask of memory range</param>
        /// <param name="readDelegate">delegate to call on memory read</param>
        /// <param name="writeDelegate">delegate to call on memory write</param>
        void RequestMemoryBlock(uint baseAddress, uint mask, pluginMemoryAccessReadEventHandler readDelegate, pluginMemoryAccessWriteEventHandler writeDelegate);

        /// <summary>
        /// Plugins can invoke a cpu interrupt. Either irq(false) or fiq(true) can be fired.
        /// </summary>
        /// <param name="fiqType">true for fiq, false for irq</param>
        void AssertInterrupt(bool fiqType);

        /// <summary>
        /// Plugins call this to request a range of opcodes to be reserved, and a delegate to be called
        /// when they are executed.
        /// </summary>
        /// <param name="baseOpcode">base opcode</param>
        /// <param name="mask">opcode mask</param>
        /// <param name="executeHandler">delegate to call when opcode encountered</param>
        void RequestOpCodeRange(uint baseOpCode, uint mask, pluginInstructionExecuteEventHandler executeHandler);

        /// <summary>
        /// Request an instruction be added to the parse tables.
        /// </summary>
        /// <param name="mnemonic"></param>
        /// <param name="code"></param>
        /// <param name="operands"></param>
        /// <param name="formOpcode"></param>
        void RequestMnemonic(string mnemonic, uint code, string operands, pluginFormOpCodeEventHandler formOpCode);

        /// <summary>
        /// The Load delegate is called when all the plugins have been loaded and their init methods called.
        /// </summary>
        //pluginLoadHandlerDelegate Load { get; set; }
        event EventHandler<EventArgs> Load;

        /// <summary>
        /// The Start delegate is called when the simulator starts a simulation session
        /// </summary>
        event EventHandler<EventArgs> StartSimulation;

        /// <summary>
        /// The Stop delegate is called when the simulator is stopped.
        /// </summary>
        event EventHandler<EventArgs> StopSimulation;

        /// <summary>
        /// The Restart delegate is called when the simulator is restarting
        /// </summary>
        event EventHandler<EventArgs> Restart;

        /// <summary>
        /// The Unload delegate is called when the simulator is shutting down
        /// </summary>
        event EventHandler<EventArgs> Unload;

        /// <summary>
        /// The Cycles delegate is called when an instruction is finished executing.
        /// The number of cycles expired is reported.
        /// </summary>
        //event pluginCyclesEventHandler Cycles;
        event EventHandler<CyclesEventArgs> Cycles;

        //void SetWaitIndicator(uint handle, bool state);

        /// <summary>
        /// Create a new standard console.
        /// </summary>
        /// <param name="title">Title to place in title bar</param>
        /// <returns></returns>
        uint CreateStandardConsole(string title);

        /// <summary>
        /// Close a standard console
        /// </summary>
        /// <param name="handle">handle of console</param>
        void CloseStandardConsole(uint handle);

        /// <summary>
        /// Write a character to standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <param name="ch">character to write</param>
        void WriteStandardConsole(uint handle, char chr);

        /// <summary>
        /// Read from standard console
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        char ReadStandardConsole(uint handle);

        /// <summary>
        /// Peek at the standard console input stream. Do not remove char from input.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character in input buffer. 0 if none</returns>
        char PeekStandardConsole(uint handle);

        //bool TerminateInput { get; }

        Preferences.IARMPreferences ARMPreferences { get; }

        IARMSimArguments ARMSimArguments { get; }

    }//interface IARMHost
}