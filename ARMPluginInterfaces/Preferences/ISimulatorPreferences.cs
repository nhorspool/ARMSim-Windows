using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    public interface ISimulatorPreferences
    {
        uint MemoryStart { get; }
        uint StackAreaSize { get; }
        uint HeapAreaSize { get; }
        uint FillPattern { get; }
        bool StopOnMisaligned { get; }
        bool StackGrowsDown { get; }
        bool ProtectTextArea { get; }
    }
}
