using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Simulator.Plugins
{
    /// <summary>
    /// This class represents a requested memory range to override.
    /// The range is represented as a base bit pattern and a mask.
    /// When a memory access is tested to be in this range, the address is AND'ed with the mask
    /// and if it is == to the base code we have a match.
    /// </summary>
    public class MemoryRange
    {
        //base bit pattern of address, mask to use in test and delegate to invoke for a match
        //and the 2 delegates to call if match for read and for write.
        private readonly uint _baseAddress;
        private readonly uint _mask;
        private readonly pluginMemoryAccessReadEventHandler _readDelegate;
        private readonly pluginMemoryAccessWriteEventHandler _writeDelegate;

        /// <summary>
        /// MemoryRange ctor
        /// </summary>
        /// <param name="baseAddress">base address of memory range</param>
        /// <param name="mask">memory address mask</param>
        /// <param name="readDelegate">delegate to call on memory read</param>
        /// <param name="writeDelegate">delegate to call on memory write</param>
        public MemoryRange(uint baseAddress, uint mask, pluginMemoryAccessReadEventHandler readDelegate, pluginMemoryAccessWriteEventHandler writeDelegate)
        {
            _baseAddress = baseAddress;
            _mask = mask;
            _readDelegate = readDelegate;
            _writeDelegate = writeDelegate;
        }

        /// <summary>
        /// Test if a memory address is in range
        /// </summary>
        /// <param name="address">address to test</param>
        /// <returns>true if a hit</returns>
        public bool hitTest(uint address)
        {
            return ((address & _mask) == _baseAddress);
        }

        /// <summary>
        /// Execute the read delegate if set. Return the result
        /// </summary>
        /// <param name="address">address being read</param>
        /// <param name="ms">size of operation</param>
        /// <returns>value read from memory</returns>
        public uint onRead(object sender, uint address, MemorySize ms)
        {
            if (_readDelegate != null)
            {
                return _readDelegate(sender, new MemoryAccessReadEventArgs(address, ms));
            }
            return 0;
        }

        /// <summary>
        /// Execute the write delegate if set. Return the result
        /// </summary>
        /// <param name="address">address being written</param>
        /// <param name="ms">size of operation</param>
        /// <param name="data">data to write</param>
        public void onWrite(object sender, uint address, MemorySize ms, uint data)
        {
            if (_writeDelegate != null)
            {
                _writeDelegate(sender, new MemoryAccessWriteEventArgs(address, ms, data));
            }
        }//onWrite

    }//class MemoryRange
}
