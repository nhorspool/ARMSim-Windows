using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    /// <summary>
    /// Replacement strategy used in this cache.
    /// </summary>
    public enum ReplaceStrategiesEnum
    {
        ///<summary>Blocks are replaced at random</summary>
        Random,
        ///<summary>Blocks are replaced round robin</summary>
        RoundRobin
    };

    public interface IInstructionCachePreferences
    {
        ReplaceStrategiesEnum ReplaceStrategy { get; }
        bool Enabled { get; }
        string TagName { get; }
        uint BlockSize { get; }
        uint NumberBlocks { get; }
        uint BlocksPerSet { get; }

    }
}
