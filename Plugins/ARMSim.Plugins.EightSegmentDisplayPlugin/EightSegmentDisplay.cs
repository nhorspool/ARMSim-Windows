using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.EightSegmentDisplayPlugin
{
    /// <summary>
    /// This ARMSim plugin demonstrates the ability to simulate an Eight segment display
    /// hardware device by intercepting reads and writes to the memory map and also by extending the SWI
    /// instruction and creating new ARM instructions.
    /// All these techniques are shown here to demonstrate the different ways hardware can be simulated by ARMSim
    /// 
    /// Memory Mapped Method
    ///     In this case we have an eight segment display mapped to the physical memory address 0x02140000.
    ///     The word (32 bits) at this address represents the pattern to display in the control.
    ///     Only the bottom 8 bits are significant the upper 24 bits are ignored.
    ///     Note that non word reads/writes to this address are ignored.
    ///     
    /// SWI Instruction Method
    ///     We will extend the SWI instruction   "swi 0x100" to perform a write operation to the display.
    ///     R0 will contain a digit(0-9) to display and R1 will contain a flag to indicate if the Point segment should be shown.
    /// 
    /// New Instruction Method
    ///     We will create 2 new ARM instructions, SEGDIGIT and SEGPATTERN that will update the display based on a register value.
    ///     These instructions work like this:
    ///     SEGDIGIT reg, where reg is a register number  ie      SEGDIGIT r5
    ///         This instruction will take the digit (0-9) in register 5 and write it to the display
    ///         (values outside 0-9 are ignored)
    ///     SEGPATTERN reg, where reg is a register number   ie    SEGPATTERN r7
    ///         This instruction will take the 8 bit pattern in the least significant bits of r7 and write to the display
    ///     
    ///     This example will also show how to insert the new instruction mnemonic into the parsing tables so that
    ///     code can be written using the new symbols.
    ///     
    /// The included test file shows ARM code to test these methods of simulating the Eight segment display.
    /// 
    /// </summary>
    public class EightSegmentDisplay : IARMPlugin, IDisposable
    {
        private IARMHost mIHost;
        private UIControls.EightSegmentDisplay mEightSegmentDisplayControl;

        /// <summary>
        /// The init function is called once the plugin has been loaded.
        /// From this function you can subscribe to the events the
        /// simulator supports.
        /// </summary>
        /// <param name="IHost"></param>
        public void Init(IARMHost ihost)
        {
            //cache a copy of the host reference so we can communicate back to ARMSim
            mIHost = ihost;

            //and subscribe to the Load event so we will be notified once all the plugins have been created
            mIHost.Load += onLoad;

        }//init

        /// <summary>
        /// The onLoad function is called after all the plugins have been loaded and their
        /// init methods called.
        /// </summary>
        public void onLoad(object sender, EventArgs e)
        {
            //Subscribe to the restart event so we can clear the eight segment display when the simulation is restarted.
            mIHost.Restart += onRestart;

            //Create the Eight segment display user control
            mEightSegmentDisplayControl = new UIControls.EightSegmentDisplay();

            //request a panel from the UI and add our control
            Panel panel = mIHost.RequestPanel(this.PluginName);
            panel.Controls.Add(mEightSegmentDisplayControl);

            //request reads/writes to the memory address of the display
            mIHost.RequestMemoryBlock(0x02140000, 0xffffffff, this.onMemoryAccessRead, this.onMemoryAccessWrite);

            //request that we get the SWI instruction (swi 0x100) for our plugin and specify the function
            // to call when it is encountered by the simulation engine.
            mIHost.RequestOpCodeRange(0x0f000100, 0x0fffffff, this.onExecuteSWI);

            //Here we are going to request part of the undefined opcode space of the ARM processor.
            //We are going to create 2 instructions to write to the Eight Segment Display as follows:
            // SEGDIGIT reg,  where reg is a gernal purpose register
            //    This instruction will take the digit in register reg and write that digit to the display.
            //    If the digit is NOT 0-9, nothing is written to the display
            //
            // SEGPATTERN reg, where reg is a gernal purpose register
            //    This instruction will take the 8 bit pattern in register reg and write that pattern to the display.
            //    The top 24 bits are ignored, only the bottom 8 bits are used.
            //
            // We will encode which instruction in bit 8 of the opcode and encode the register number in the
            // bottom 4 bits of the opcode (bits 0-3) (See onExecuteEightSegmentOpcode for details)
            mIHost.RequestOpCodeRange(0x0ef000f0, 0x0ffffef0, this.onExecuteEightSegmentOpcode);
            //mIHost.RequestOpcodeRange("*ef00ef*", this.onExecuteEightSegmentOpcode);

            // To make testing of these instructions easier, we will insert some mnemonics into the parsing
            // tables so we can reference the instructions by their symbols. We need to provide a base opcode
            // and the expected paramaters. In this case we expect a single register so we use the operand "R"
            // We also need to provide a delegate callback that the parser will call when this mnemonic is parsed.
            //
            // However, RequestMnemonic is no longer supported in ARMSim#2.0
            // these requests will throw an exception
            mIHost.RequestMnemonic("SEGDIGIT", 0x0ef000f0, "R", this.onFormEightSegmentOpCode);
            mIHost.RequestMnemonic("SEGPATTERN", 0x0ef001f0, "R", this.onFormEightSegmentOpCode);

        }//onLoad

        /// <summary>
        /// Called when our requested opcode(s) have been fetched and need executing
        /// Since we encoded the instruction in bit 8 of the opcode, we will test that bit to determine
        /// which of the 2 instructions we created needs executing.
        /// Note that the register paramater is encoded into the bottom 4 bits of the opcode.
        /// See onFormEightSegmentOpcode for details.
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public uint onExecuteEightSegmentOpcode(uint opCode)
        {
            //extract the register from the opcode
            uint reg = opCode & 0x0f;

            //now obtain the register contents from the simulator
            //not we are casting to an 8 bit value, we are not interested in the top 24 bits
            byte number = (byte)mIHost.getReg(reg);

            //determine which instruction is being executed and perform the operation on the
            // Eight Segment display
            switch (opCode & 0x00000100)
            {
                //SEGDIGIT reg
                case 0x0000:
                    mEightSegmentDisplayControl.SetNumberPattern(number, false);
                    return 3;

                //SEGPATTERN reg
                case 0x0100:
                    mEightSegmentDisplayControl.Code = number;
                    return 3;
            }//switch
            return 0;
        }//onExecuteEightSegmentOpcode

        /// <summary>
        /// This callback is called by the parser when the requested mnemonic is parsed. It allows the plugin
        /// the oppurtunity to form the opcode from the operands. In this case we will have 1 operand which is the register
        /// number specified in the instruction. We will encode this register value into the bottom 4 bits of the opcode.
        /// </summary>
        /// <param name="baseCode"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public uint onFormEightSegmentOpCode(uint baseCode, params uint[] operands)
        {
            //make sure we have exactly 1 operand. Anything else is an error.
            if (operands.Length != 1)
                return baseCode;

            //encode the operand into the bottom 4 bits of the base opcode and return.
            return (baseCode | (operands[0] & 0x0f));
        }

        /// <summary>
        /// This function is called when the simulator is reloading the simulated program.
        /// The plugin should be restored to its intial state. In this case we will clear
        /// the Eight segment display
        /// </summary>
        public void onRestart(object sender, EventArgs e)
        {
            mEightSegmentDisplayControl.Code = 0;
        }


        /// <summary>
        /// This function is called when our requested opcode(s) have been fetched and need executing
        /// Input:
        ///  opcode:is the opcode fetched
        /// Output
        ///  returns number of clock cycles required to execute this instruction
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public uint onExecuteSWI(uint opCode)
        {
            uint number = mIHost.getReg(0);
            bool point = (mIHost.getReg(0) != 0);
            mEightSegmentDisplayControl.SetNumberPattern(number, point);
            return 3;
        }//onExecuteSWI

        /// <summary>
        /// This function is called whenever a write access is performed on a reserved block
        /// Input:
        ///  mwa:properties of the write opertaion
        /// </summary>
        /// <param name="mwa"></param>
        public void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            //ignore any non-word writes
            if (mwa.Size != MemorySize.Word)
                return;

            //set the bottom eight bits as the display byte for our control
            mEightSegmentDisplayControl.Code = (byte)mwa.Value;

        }//onMemoryAccessWrite

        /// <summary>
        /// This function is called whenever a read access is performed on a reserved block
        /// Input:
        ///   mra:properties of the read opertaion
        /// </summary>
        /// <param name="mra"></param>
        /// <returns></returns>
        public uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            //ignore any non-word reads
            if (mra.Size != MemorySize.Word)
                return 0;

            //get the current byte of the control and return it as a word
            return mEightSegmentDisplayControl.Code;
        }//onMemoryAccessRead

        /// <summary>
        /// This property is the Name of the plugin. Plugins names must be unique in the host assembly.
        /// </summary>
        public string PluginName { get { return "Eight Segment Display"; } }

        /// <summary>
        /// This property is the Description string of the plugin. This can be any text that describes
        /// the plugin.
        /// </summary>
        public string PluginDescription { get { return "Simulates an eight segment display"; } }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mEightSegmentDisplayControl.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }//class SamplePlugin
}
