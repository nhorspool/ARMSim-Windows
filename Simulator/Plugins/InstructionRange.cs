using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Simulator.Plugins
{
    /// <summary>
    /// This class represents a requested instruction range to override.
    /// The range is represented as a base bit pattern and a mask.
    /// When an opcode is tested to be in this range, the opcode is AND'ed with the mask
    /// and if it is == to the base code we have a match.
    /// </summary>
    public class InstructionRange
    {
        //base bit pattern of opcode, mask to use in test and delegate to invoke for a match
        private readonly uint _baseOpCode;
        private readonly uint _mask;
        private readonly pluginInstructionExecuteEventHandler _executeDelegate;

        /// <summary>
        /// InstructionRange ctor
        /// </summary>
        /// <param name="baseOpCode">base opcode to reserve</param>
        /// <param name="mask">opcode mask to apply</param>
        /// <param name="executeDelegate">delegate to call on execute opcode</param>
        public InstructionRange(uint baseOpCode, uint mask, pluginInstructionExecuteEventHandler executeDelegate)
        {
            _baseOpCode = baseOpCode;
            _mask = mask;
            _executeDelegate = executeDelegate;
        }

        /// <summary>
        /// Checks for a match
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public bool hitTest(uint opCode)
        {
            //perform the and and equality check
            return ((opCode & _mask) == _baseOpCode);
        }

        /// <summary>
        /// Execute the delegate and return the result
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public uint onExecute(uint opCode)
        {
            return _executeDelegate(opCode);
        }//onExecute

    }//class InstructionRange
}
