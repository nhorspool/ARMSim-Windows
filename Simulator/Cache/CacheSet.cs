#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;
using ARMSim.Preferences;

using ARMPluginInterfaces.Preferences;

namespace ARMSim.Simulator.Cache
{
	/// <summary>
	/// CacheSet represents one set of the cache. The cache will contain at least one set.
	/// This class will maintain a collection of CacheBlocks which comprise the set.
	/// When a read or write request comes in, this class will determine which cache line the
	/// request is for and forward the request. The cache statistics are maintained here.
	/// </summary>
	public class CacheSet
	{
		private MemoryBlock		memBlock;

		private uint		nextAllocate;
		private Random		rand;

        private readonly ReplaceStrategiesEnum _replaceStrategy;

        private CacheChangedDelegate _cacheChangedHandler;

        /// <summary>
        /// Create a CacheSet with blocksPerSet lines. Create each line in the set and reset the
        /// statisitcs.
        /// </summary>
        /// <param name="mb">reference to main memory block</param>
        /// <param name="wordsPerBlock">number of words per block</param>
        /// <param name="blocksPerSet">number of blocks per set</param>
        /// <param name="startBlock">starting block of this set</param>
        /// <param name="replaceStrategy">which replacement strategy to use</param>
        public CacheSet(MemoryBlock mb, uint wordsPerBlock, uint blocksPerSet, uint startBlock, ReplaceStrategiesEnum replaceStrategy)
		{
			memBlock = mb;
			_replaceStrategy = replaceStrategy;
			rand = new Random( 4 );

            Blocks = new CacheBlock[blocksPerSet];
			for(uint ii=0;ii<blocksPerSet;ii++)
			{
                Blocks[ii] = new CacheBlock(mb, wordsPerBlock, startBlock++);
			}//for ii
			ResetStats();
        }//CacheSet ctor

        /// <summary>
        /// set all counters to 0
        /// </summary>
		public void ResetStats()
		{
            WriteHits = WriteMisses = ReadHits = ReadMisses = 0;
		}

		//return various statistic values
        /// <summary>Return the current read hit counter</summary>
        public uint ReadHits { get; private set; }
        /// <summary>Return the current read miss counter</summary>
        public uint ReadMisses { get; private set; }
        /// <summary>Return the current write hit counter</summary>
        public uint WriteHits { get; private set; }
        /// <summary>Return the current write miss counter</summary>
        public uint WriteMisses { get; private set; }

        /// <summary>
        /// Access to the set blocks
        /// </summary>
        public CacheBlock[] Blocks { get; private set; }

        /// <summary>
        /// Allocate a line in the cache.
        /// First determine if any unallocated blocks exist in the the set. If so, allocate one of those.
        /// Otherwise action is determined by the replacement strategy. Either a line is selected at
        /// Random or is selected by a round-robin.
        /// </summary>
        /// <param name="address">address of allocated line</param>
        /// <returns></returns>
		private CacheBlock allocateLine(uint address)
		{
			//first check if any line is not allocated yet, if so pick that one
            foreach (CacheBlock cl in Blocks)
			{
				if(!cl.Valid)
				{
					//free line found, load it from mainmemory
					cl.LoadFromMainMemory(address);

					//if the change handler was installed, call it
                    if (_cacheChangedHandler != null)
					{
                        _cacheChangedHandler(cl.BlockNumber, address & cl.Mask);
					}
					return cl;
				}
			}//foreach

			//all lines are allocated, so we must replace one. use the replacement algorithm specified
			CacheBlock cacheBlock;
			switch(_replaceStrategy)
			{
				//round robin, get next line in order
                case ReplaceStrategiesEnum.RoundRobin:
                    cacheBlock = Blocks[nextAllocate++];
                    if (nextAllocate >= Blocks.Length) nextAllocate = 0;
					break;

				//random, generate a random number 0-lines-1 and use that line
                case ReplaceStrategiesEnum.Random:
                    int index = rand.Next(0, Blocks.Length - 1);
                    cacheBlock = Blocks[index];
					break;

				default:
					throw new Exception("bad replacement strategy");
			}//switch

			//the load will purge the cache line if it is dirty
			cacheBlock.LoadFromMainMemory(address);

			//notify any handlers that the cache has changed
            if (_cacheChangedHandler != null)
			{
                _cacheChangedHandler(cacheBlock.BlockNumber, address & cacheBlock.Mask);
			}
			return cacheBlock;
        }//allocateLine

        /// <summary>
        /// Read from the CacheSet.
        /// Check each block in the set and compare the line tag with the address. If the address tag matches the
        /// line tag, then we have a cache block hit. Otherwise we need to get the requested block into the
        /// cache and this will depend on the allocate policy.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <param name="allocatePolicy">allocation policy to use</param>
        /// <returns>value read</returns>
        public uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms, AllocatePolicyEnum allocatePolicy)
		{
			//check each line in the cache set
            foreach (CacheBlock cb in Blocks)
			{
				//only check valid lines and look for a tag match
				if(cb.Valid && ((address & cb.Mask) == cb.Tag))
				{
					//cache read hit
                    ReadHits++;
                    return cb.GetMemory(address, ms);
				}
			}
			//cache read miss
            ReadMisses++;

            if (allocatePolicy != AllocatePolicyEnum.Write)
			{
				//read or both policy
                return allocateLine(address).GetMemory(address, ms);
			}
			else
			{
				//write policy, do not allocate a new cache line
                return memBlock.GetMemory(address, ms);
			}
        }//GetMemory

        /// <summary>
        /// Reset the CacheSet, pass on reset request to all lines in cache set
        /// </summary>
		public void Reset()
		{
            //iterate over all the cache blocks and reset them
            foreach (CacheBlock cl in Blocks)
			{
				cl.Reset();
			}
		}//Reset

        /// <summary>
        /// Purge the CacheSet, pass on purge request to all lines in cache set
        /// </summary>
		public void Purge()
		{
            //iterate over all the cache blocks and purge them
            foreach (CacheBlock cl in Blocks)
			{
				cl.Purge();
			}
		}//Purge

        /// <summary>
        /// This method allows reading of the cache with no side effects. This is used ny the user interface
        /// so the cache contents can be displayed without changing the state of the cache.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <returns>valuee read</returns>
        public uint GetMemoryNoSideEffect(uint address, ARMPluginInterfaces.MemorySize ms)
		{
            //iterate over the cache blocks looking for the block that holds this address
            foreach (CacheBlock cl in Blocks)
			{
                //check if this block is valid and has this address
                if (cl.Valid && ((address & cl.Mask) == cl.Tag))
				{
					//cache read hit
                    return cl.GetMemory(address, ms);
				}//if
			}//foreach
            return memBlock.GetMemory(address, ms);
        }//GetMemoryNoSideEffect

        /// <summary>
        /// Property to set delegate for cache changed callback
        /// </summary>
        public CacheChangedDelegate CacheChangedHandler
		{
			set{_cacheChangedHandler=value;}
		}

        /// <summary>
        /// Write to the CacheSet.
        /// Check each block in the set and compare the block tag with the address. If the address tag matches the
        /// block tag, then we have a cache write block hit. Otherwise we need to get the requested block into the
        /// cache and this will depend on the allocate policy.
        /// </summary>
        /// <param name="address">address to write to</param>
        /// <param name="ms">size to write</param>
        /// <param name="data">data to write</param>
        /// <param name="writeThru">true then write through to main memory</param>
        /// <param name="allocatePolicy">allocate policy to use</param>
        public void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data, bool writeThru, AllocatePolicyEnum allocatePolicy)
		{
            //iterate over the cache blocks looking for the block that holds this address
            foreach (CacheBlock cb in Blocks)
			{
                //check if this block is valid and has this address
				if (cb.Valid && ((address & cb.Mask) == cb.Tag))
				{
					//cache write hit, set data into cache memory
                    WriteHits++;
                    cb.SetMemory(address, ms, data, writeThru);

                    //if the changed handler is set, call it
                    if (_cacheChangedHandler != null)
                        _cacheChangedHandler(cb.BlockNumber, address & cb.Mask);

                    //done
					return;
				}//if
			}//foreach

			//cache write miss
            WriteMisses++;

            //allocate a cache line based on the allocation policy
            if (allocatePolicy != AllocatePolicyEnum.Read)
			{
				//write or both policy
                allocateLine(address).SetMemory(address, ms, data, writeThru);
			}
			else
			{
				//read policy, do not allocate a new cache line
                memBlock.SetMemory(address, ms, data);
			}
        }//SetMemory
    }//class CacheSet
}
