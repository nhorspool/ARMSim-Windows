using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    ///<summary>the write policies this cache can use</summary>
    public enum WritePolicyEnum
    {
        ///<summary>write data to cache then through to main memory</summary>
        WriteThrough,
        ///<summary>write data to cache only</summary>
        WriteBack
    };

    ///<summary>When to allocate a new cache block policy</summary>
    public enum AllocatePolicyEnum
    {
        ///<summary>When reading only</summary>
        Read,
        ///<summary>When writing only</summary>
        Write,
        ///<summary>When reading or writing</summary>
        Both
    };

    public interface IDataCachePreferences
    {
        WritePolicyEnum WritePolicy { get; }
        AllocatePolicyEnum AllocatePolicy { get; }

    }
}
