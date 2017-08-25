using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.IntegerDivisionPlugin
{
    /// <summary>
    /// This Plugin demonstrates ARMSim# ability to allow the creation of new ARM instructions.
    /// In this example we will create 2 instructions to perform integer division. An unsigned and
    /// signed version of the instruction will be created and will work as follows:
    /// 1) UDIV Rd, Rm, Rn
    ///       Computes  Rd = Rm / Rn using unsigned integers
    ///       If (Rn) == 0 the result will be 0.
    /// 2) SDIV Rd, Rm, Rn
    ///       Computes  Rd = Rm / Rn using signed integers, result is signed
    ///       If (Rn) == 0 the result will be 0.
    ///       
    /// Instruction mnemonics are inserted into the parsing tables so the above syntax will work in source code.
    /// See TestFiles\testIntegerDivisionPlugin.s in this project for an example of how to use these instructions
    /// in ARM source.
    /// </summary>
    public class IntegerDivision : IARMPlugin
    {
        /// <summary>
        /// Reference to the ARMSim host interface. Aquired in the Init method.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// The Init method of the plugin. This method is called when the plugin has been created by ARMSim.
        /// ARMSim will pass an instance of IARMHost to the plugin so that the plugin can make requests back to ARMSim.
        /// </summary>
        /// <param name="ihost"></param>
        public void Init(IARMHost ihost)
        {
            //cache the interface to ARMSim
            mHost = ihost;

            //set the load handler so we get called when all the plugins are loaded
            mHost.Load += onLoad;

        }//Init

        /// <summary>
        /// The onLoad is called after all plugins have had their init called and are all loaded.
        /// </summary>
        public void onLoad(object sender, EventArgs e)
        {
            //request the opcodes from the undefined opcode range of the ARM processor. Specify the execute method
            //that will be called when this opcode is encountered by the simulation engine.
            mHost.RequestOpCodeRange(0x0ef000f0, 0x0ff000f0, this.onExecute);

            //Here we will insert the mnemonics of the new instructions into the parsing tables. The method specified
            //is called whwn the instruction is parsed. It allows us to form the final opcode with the operands
            //encoded into the opcode.
            //
            // However RequestMnemonic is no longer supported in ARMSim#2.0.
            // Thse requests will throw an exception.
            mHost.RequestMnemonic("UDIV", 0x0ef000f0, "RRR", this.onFormInstruction);
            mHost.RequestMnemonic("SDIV", 0x0ef001f0, "RRR", this.onFormInstruction);

        }//onLoad

        /// <summary>
        /// This method is called when the parsing of the source code has encountered an mnemonic that was added earlier.
        /// For our purposes all we need to do is encode the source/destination register into the least significant
        /// 4 bits of the opcode.
        /// The operands array contain the parsed operands. We do a sanity check to make sure there is only 1 entry in the array
        /// </summary>
        /// <param name="baseCode"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public uint onFormInstruction(uint baseCode, params uint[] operands)
        {
            //if not exactly 1 operand, do nothing
            if (operands.Length != 3)
                return baseCode;

            //encode register into opcode and return the formed opcode
            baseCode |= (operands[0] & 0x0f) << 16;
            baseCode |= (operands[1] & 0x0f) << 12;
            return baseCode | (operands[2] & 0x0f);
        }//onFormInstruction64

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin name
        /// </summary>
        public string PluginName { get { return "IntegerDivision"; } }

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin description
        /// </summary>
        public string PluginDescription { get { return "Integer Division instructions"; } }

        /// <summary>
        /// This method is called when our requested opcode(s) have been fetched and need executing.
        /// We will inspect the opcode and perform the operation specified in the instruction bits
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public uint onExecute(uint opcode)
        {
            //extract the register numbers from the opcode. See onFormInstruction for bit locations
            //of the 3 registers required
            uint Rd = ((opcode >> 16) & 0x0f);
            uint Rm = ((opcode >> 12) & 0x0f);
            uint Rn = (opcode & 0x0f);

            //assume a result of 0 for now
            uint result = 0;

            //extract the numerator and denomenator from the CPU registers
            uint denomenator = mHost.getReg(Rn);
            uint numerator = mHost.getReg(Rm);

            //if denomenator is 0, use the result of 0, otherwise execute the proper instruction
            if (denomenator != 0)
            {
                switch (opcode & 0x00000f00)
                {
                    //DIV rd,rm,rn
                    //compute    rd = rm / rn
                    //unsigned integers
                    case 0x0000:
                        result = numerator / denomenator;
                        break;

                    //DIVS rd,rm,rn
                    //compute    rd = rm / rn
                    //signed integers
                    case 0x0100:
                        result = (uint)((int)numerator / (int)denomenator);
                        break;

                    default: break;
                }//switch
            }//if

            //write result into destination CPU register
            mHost.setReg(Rd, result);

            //return number of clock cycles required to execute this opcode
            return 3;
        }//onExecute

    }//class IntegerDivision
}
