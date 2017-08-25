#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;
using ARMSim.Simulator;
using ARMSim.Preferences;

using ARMPluginInterfaces.Preferences;

namespace ARMSim.Simulator.Cache
{
	/// <summary>
	/// This class will implement an L1Cache. It can be thought of as a read-only version of the Data Cache.
	/// This class only allows the reading of the cache, essentially what an InstructionCache will do. Only reading
	/// can occur. The DatCache will derive from this class and add the write functionality.
	/// An L1Cache can be considered as a collection of CacheSets. There will always be at least one CacheSet
	/// in a cache.
	/// </summary>
	public class L1Cache
	{
        ///<summary>A reference to the main memory block of the simulator</summary>
		protected MemoryBlock		memBlock;

        ///<summary>The replacement strategy for this cache.</summary>
        private readonly ReplaceStrategiesEnum replaceStrategy;

        ///<summary>The number of words in one cache block for this configuration.</summary>
        protected readonly uint wordsPerBlock;

        /// <summary>
        /// Construct an L1Cache given a cache settings
        /// </summary>
        /// <param name="mb">reference to main memory block</param>
        /// <param name="cs">instruction cache preferences</param>
        public L1Cache(MemoryBlock mb, InstructionCachePreferences cs)
        {
            memBlock = mb;

            //Only if this cache is enabled do we create all these items
            if (cs.Enabled)
            {
                replaceStrategy = cs.ReplaceStrategy;
                wordsPerBlock = cs.BlockSize / 4;
                uint numberBlocks = cs.NumberBlocks;
                uint blocksPerSet = cs.BlocksPerSet;

                //determine the number of sets in the cache
                uint numSets = numberBlocks / blocksPerSet;

                //and allocate the collection of sets
                Sets = new CacheSet[numSets];

                //create and init each set
                uint blockNumber = 0;
                for (uint ii = 0; ii < numSets; ii++)
                {
                    Sets[ii] = new CacheSet(mb, wordsPerBlock, blocksPerSet, blockNumber, replaceStrategy);
                    blockNumber += blocksPerSet;
                }//for ii
            }//if

            ResetStats();
        }//L1Cache ctor

        /// <summary>
        /// Reset the statistics foreach cache set
        /// </summary>
		public void ResetStats()
		{
			if(!this.Enabled)return;
            foreach (CacheSet cs in Sets)
			{
				cs.ResetStats();
			}//foreach
		}//ResetStats

        /// <summary>
        /// Access to the cache sets
        /// </summary>
        public CacheSet[] Sets { get; private set; }

        /// <summary>
        /// Gather the read hits from the cache sets
        /// </summary>
		public uint ReadHits
		{
			get
			{
				uint sum=0;
				if(!this.Enabled)return 0;
                foreach (CacheSet cs in Sets)
				{
					sum+=cs.ReadHits;
				}//foreach
				return sum;
			}
		}//ReadHits

        /// <summary>
        /// Gather the read misses from the cache sets
        /// </summary>
		public uint ReadMisses
		{
			get
			{
				uint sum=0;
				if(!this.Enabled)return 0;
                foreach (CacheSet cs in Sets)
				{
					sum+=cs.ReadMisses;
				}//foreach
				return sum;
			}
		}//ReadMisses

        /// <summary>
        /// Return a falg indicating if the cache is enabled
        /// </summary>
        public bool Enabled { get { return Sets != null; } }

        /// <summary>
        /// Compute a set number based on the address.
        /// </summary>
        /// <param name="address">address of set</param>
        /// <returns>computed set number</returns>
		protected uint computeSetNumber( uint address )
		{
			//first determine the cache block numer
			uint blockNumber = ( address >> 2 ) / wordsPerBlock;

			//and the set is the block number mod number of sets
            return (uint)(blockNumber % Sets.Length);
		}//computeSetNumber

        /// <summary>
		/// Get memory from the cache. If the cache is not enabled, pass the request to main memory.
		/// Otherwise pass request to computed cache set.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <returns>value read</returns>
        public virtual uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms)
		{
			if(!this.Enabled)
                return memBlock.GetMemory(address, ms);

			//force allocate policy to be read for an L1cache
            return Sets[computeSetNumber(address)].GetMemory(address, ms, ARMPluginInterfaces.Preferences.AllocatePolicyEnum.Read);
        }//GetMemory

        /// <summary>
        /// Set the cache changed handler in each of the CacheSets
        /// </summary>
        public CacheChangedDelegate CacheChangedHandler
		{
			set
			{
				if(!this.Enabled)return;
                foreach (CacheSet cs in Sets)
				{
					cs.CacheChangedHandler = value;
				}//foreach
			}//set
		}//CacheChangedHandler

        /// <summary>
        /// Purge all sets in the cache
        /// </summary>
		public void Purge()
		{
			if(!this.Enabled)return;
            foreach (CacheSet cs in Sets)
			{
				cs.Purge();
			}
		}//Purge

        /// <summary>
        /// Reset all sets in the cache
        /// </summary>
		public void Reset()
		{
			if(!this.Enabled)return;
            foreach (CacheSet cs in Sets)
			{
				cs.Reset();
			}
			this.ResetStats();
		}//Reset
	}//class L1Cache
}
