using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    public interface ICachePreferences
    {
        bool UnifiedCache { get; }
        IDataCachePreferences IDataCachePreferences { get; }
        IInstructionCachePreferences IInstructionCachePreferences { get; }
    }
}
