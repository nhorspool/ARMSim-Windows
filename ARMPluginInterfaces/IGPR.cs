using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    public delegate void RegisterChangedDelegate(uint reg);

    public interface IGPR
    {
        void SetUserModeRegister(uint reg, uint newValue);
        void Reset();
        uint SP { get; set; }
        uint LR { get; set; }
        uint PC { get; set; }
        uint this[uint index] { get; set; }
        RegisterChangedDelegate RegisterChangedHandler { set; }
    }
}