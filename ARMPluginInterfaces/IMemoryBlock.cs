using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    //Exception used to report invalid memory accesses
    [Serializable]
    public class MemoryAccessException : Exception
    {
        private readonly uint _address;
        private readonly string _reason;
        public MemoryAccessException(uint address, string reason)
        {
            _address = address;
            _reason = reason;
        }
        public uint Address { get { return _address; } }
        public string Reason { get { return _reason; } }
    }

    public interface IMemoryBlock
    {
        uint Start { get; }
        uint Size { get; }
        bool ProtectTextArea { get; set; }
        uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms);
        void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data);
        bool InRange(uint address, ARMPluginInterfaces.MemorySize ms);
        bool InDataRange(uint address, ARMPluginInterfaces.MemorySize ms);
        void HeapClear();
        void Reset(uint fillPattern, bool stackGrowsDown);
        uint DataStart { get; set; }
        uint DataEnd { get; set; }
        uint StackStart { get; set; }
        uint StackEnd { get; set; }
        uint HeapStart { get; }
        uint HeapEnd { get; }
        uint malloc(uint size);
    }
}
