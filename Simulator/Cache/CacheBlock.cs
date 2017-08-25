#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;

namespace ARMSim.Simulator.Cache
{
	/// <summary>
	/// CacheBlock represents one line in the cache. When created, the size of the line
	/// is specified. Enough space is allocated for a line and this class will manage
	/// the cache line.
	/// This class is capable of loading itself from main memory, and also saving itself
	/// to main memory. A dirty flag is maintained indicating if any words have changed
	/// from being loaded.
	/// </summary>
	public class CacheBlock
	{
		private MemoryBlock	memBlock;				//reference to main memory

        /// <summary>
        /// CacheBlock ctor
        /// </summary>
        /// <param name="mb">reference to the main memory block</param>
        /// <param name="wordsPerBlock">number of words in one cache block</param>
        /// <param name="blockNumber">block number of this block</param>
		public CacheBlock(MemoryBlock mb, uint wordsPerBlock, uint blockNumber)
		{
			memBlock = mb;
			BlockNumber = blockNumber;
            Data = new uint[wordsPerBlock];
			Mask = ( Utils.signExtend(wordsPerBlock) ) << 2;
		}

        ///<summary>Return true if this block is valid.</summary>
        public bool Valid { get; private set; }
        ///<summary>Return the block tag</summary>
        public uint Tag { get; private set; }
        ///<summary>Return the block mask</summary>
        public uint Mask { get; private set; }
        ///<summary>Return the block dirty flag</summary>
        public bool Dirty { get; private set; }
        ///<summary>Return the block number</summary>
        public uint BlockNumber { get; private set; }

        /// <summary>
        /// Access to the block data
        /// </summary>
        public uint[] Data { get; private set; }

        /// <summary>
        /// Read the block of memory at the given address and size.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size of read</param>
        /// <returns>value read at that address</returns>
        public uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            System.Diagnostics.Debug.Assert(Valid && ((address & Mask) == Tag));

            uint index = (address & ~Mask) >> 2;
            uint data = Data[index];

            //fetch the value based on the size requested
            switch (ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte:
                    {
                        uint byteNumber = address & 0x0003;
                        data = (data >> (int)(byteNumber * 8)) & 0x00ff;
                    } break;

                case ARMPluginInterfaces.MemorySize.HalfWord:
                    {
                        if ((address & 0x0002) != 0)
                            data >>= 16;
                        data &= 0xFFFF;
                    } break;

                case ARMPluginInterfaces.MemorySize.Word:
                    break;

                default:
                    throw new Exception("Bad memory size specified");
            }//switch
            return data;
        }//GetMemory

        /// <summary>
        /// Write to the cache Line. If the write-thru policy is active, write to both
        /// the cache line and out to main memory.
        /// </summary>
        /// <param name="address">address to write to</param>
        /// <param name="ms">size of write</param>
        /// <param name="data">value to write</param>
        /// <param name="writeThru">write through to main memory</param>
        public void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data, bool writeThru)
		{
			System.Diagnostics.Debug.Assert(Valid && ((address & Mask) == Tag));

			//convert address to array index
			uint index = (address & ~Mask)>>2;

			switch(ms)
			{
				//case handles the writing of bytes
                case ARMPluginInterfaces.MemorySize.Byte:
				{
					uint byteNumber = address & 0x0003;
					uint mask = (uint)(0x00ff << (int)(byteNumber * 8));

                    uint newData = Data[index] & ~mask;
					uint thisData = data & 0x00ff;
					uint sData = thisData << (int)(byteNumber * 8);
                    Data[index] = newData | sData;

				}
				break;

				//case handles the writing of halfwords
            case ARMPluginInterfaces.MemorySize.HalfWord:
                {
                    uint d = Data[index];
                    if ((address & 0x0002) == 0)
                        Data[index] = (d & 0xFFFF0000) | (data & 0x0000FFFF);
                    else
                        Data[index] = (d & 0x0000FFFF) | (data << 16);
				}
				break;

				//case handles the writing of words
            case ARMPluginInterfaces.MemorySize.Word:
					Data[index] = data;
					break;
				default:
					throw new Exception("Bad memory size specified");
			}//switch

			if (writeThru)
            {//if write thru is active then write this to main memory as well
                memBlock.SetMemory(address, ms, data);
			}
			else
            {//else set the dirty bit
				Dirty = true;
			}
        }//SetMemory

        /// <summary>
        /// Load this line from main memory. Based on the line address, go to main memory and load
        /// this line with data.
        /// </summary>
        /// <param name="address">address to load from</param>
		public void LoadFromMainMemory(uint address)
		{
            //make sure any changed memory is purged back to main memory
			this.Purge();
			uint thisAddress = address & Mask;
			Tag = thisAddress;

            //load the cache block from main memory
            for (uint ii = 0; ii < Data.Length; ii++, thisAddress += 4)
			{
                Data[ii] = memBlock.GetMemory(thisAddress, ARMPluginInterfaces.MemorySize.Word);
			}
			Dirty = false;
			Valid = true;
        }//loadFromMainMemory

        /// <summary>
        /// Purge this line. If it is dirt, write back to main memory, reset dirty flag.
        /// </summary>
		public void Purge()
		{
            //if no dirty data, we are done
			if (!Valid || !Dirty)return;

			uint thisAddress = Tag;
            for (uint ii = 0; ii < Data.Length; ii++, thisAddress += 4)
			{
                memBlock.SetMemory(thisAddress, ARMPluginInterfaces.MemorySize.Word, Data[ii]);
			}
			Dirty = false;
		}//Purge

        /// <summary>
        /// The cache is being reset. Purge this line back to main memory and reset to an invalid state
        /// </summary>
		public void Reset()
		{
			this.Purge();
			Valid = false;
			Dirty = false;
			Tag = 0;
		}//Reset
    }//class CacheBlock
}
