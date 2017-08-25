using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    public interface IGeneralPreferences
    {
        bool SyncCacheOnExit { get; }
        bool CloseFilesOnExit { get; }
        string StdinFileName { get; }
        string StdoutFileName { get; }
        string StderrFileName { get; }
        bool StdoutOverwrite { get; }
        bool StderrOverwrite { get; }
    }
}
