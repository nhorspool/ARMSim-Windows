#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;
using ARMSim.Preferences;

using ARMPluginInterfaces.Preferences;

namespace ARMSim.Simulator.Cache
{
	/// <summary>
	/// This class implements the DataCache. It derives from L1Cache where most of the functionality
	/// of the cache exists. This class addds write support to the cache.
	/// </summary>
	public class DataCache : L1Cache
	{
		//the write policy. Write-through ir write-back
        private WritePolicyEnum _writePolicy;

		//allocate policy. read,write or both
        private readonly AllocatePolicyEnum _allocatePolicy;

		private readonly bool _writeThru;

        /// <summary>
        /// Construct a DataCache given data cache preferences.
        /// </summary>
        /// <param name="memoryBlock">reference to main memory block</param>
        /// <param name="dataCachePreferences">the data cache preferences</param>
        public DataCache(MemoryBlock memoryBlock, DataCachePreferences dataCachePreferences)
            : base(memoryBlock, dataCachePreferences)
        {
            _writePolicy = dataCachePreferences.WritePolicy;
            _allocatePolicy = dataCachePreferences.AllocatePolicy;
            _writeThru = (_writePolicy == WritePolicyEnum.WriteThrough);
        }

        /// <summary>
        /// get the Writehits count from each cache set
        /// </summary>
		public uint WriteHits
		{
			get
			{
				uint sum=0;
				if (!this.Enabled) return 0;
				foreach(CacheSet cs in Sets)
				{
					sum += cs.WriteHits;
				}//foreach
				return sum;
			}
		}//WriteHits

        /// <summary>
        /// get the Write misses count from each cache set
        /// </summary>
		public uint WriteMisses
		{
			get
			{
				uint sum=0;
				if (!this.Enabled) return 0;
				foreach(CacheSet cs in Sets)
				{
					sum += cs.WriteMisses;
				}//foreach
				return sum;
			}
		}//WriteMisses

        /// <summary>
        /// Override the getmem here to use the true allocate policy. The L1Cache version forces
        /// this parameter to be read allocate.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <returns></returns>
        public override uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms)
		{
            uint result;
			if(!this.Enabled)
                result = memBlock.GetMemory(address, ms);
			else
                result = Sets[computeSetNumber(address)].GetMemory(address, ms, _allocatePolicy);
            if ((address & 0x03) != 0)  // check for unaligned access
            {
                if (ms == ARMPluginInterfaces.MemorySize.Word)
                    // implement rotations of the memory word as specified in
                    // the architectural specification of the LDR instruction
                    switch(address & 0x03) {
                        case 0x01:
                            result = ((result >>  8) & 0x00FFFFFF) | (result << 24);
                            break;
                        case 0x02:
                            result = ((result >> 16) & 0x0000FFFF) | (result << 16);
                            break;
                        case 0x03:
                            result = ((result >> 24) & 0x000000FF) | (result <<  8);
                            break;
                    }
                // Note: for an unaligned halfword access, the result is unpredictable
                // so we do nothing.
            }
            return result;
        }//GetMemory

        /// <summary>
        /// Write memory to data cache
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <param name="data">data to write</param>
        public void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data)
		{
			if (!this.Enabled)
            {
                memBlock.SetMemory(address,ms,data);
                return;
            }//if
            Sets[computeSetNumber(address)].SetMemory(address, ms, data, _writeThru, _allocatePolicy);
        }//SetMemory
	}//class DataCache
}
