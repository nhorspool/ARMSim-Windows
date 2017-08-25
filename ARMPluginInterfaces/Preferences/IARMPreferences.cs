using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    public interface IARMPreferences
    {
        IGeneralPreferences IGeneralPreferences { get; }
        ICachePreferences ICachePreferences { get; }
        ISimulatorPreferences ISimulatorPreferences { get; }
        IPluginPreferences IPluginPreferences { get; }

        string LastTab { get; }

    }
}
