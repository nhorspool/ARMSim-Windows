#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;
using System.Text;

namespace ARMSim.Simulator
{
    /// <summary>
    /// Delegate defines a function to read memory with. Some cases will use GetMemory, some
    /// may need the version without side effects.
    /// </summary>
    /// <param name="address">address to read</param>
    /// <param name="ms">size of memory to read</param>
    /// <returns></returns>
    public delegate uint ReadMemoryDelegate(uint address, ARMPluginInterfaces.MemorySize ms);

	/// <summary>
	/// This class provides some useful utility routines.
	/// </summary>
	public static class Utils
	{
        /// <summary>
        /// Constant with msb set
        /// </summary>
		public const uint bit_31		=0x80000000;

        /// <summary>
        /// Constant with 2nd msb set.
        /// </summary>
		private const uint bit_30		=0x40000000;

        /// <summary>
        /// Constant with 2nd lsb set
        /// </summary>
		private const uint bit_1		=0x00000002;

        /// <summary>
        /// Constant with lsb set
        /// </summary>
		private const uint bit_0		=0x00000001;

        /// <summary>
        /// Utils ctor.
        /// </summary>
    	//public Utils() { }

        /// <summary>
        /// Load a string from memory. Load bytes until a 0 encountered or max bytes reached.
        /// </summary>
        /// <param name="rm">function to use to read a byte</param>
        /// <param name="address">address to start reading</param>
        /// <param name="maxSize">max bytes to read</param>
        /// <returns></returns>
        public static string loadStringFromMemory(ReadMemoryDelegate readMemoryDelegate, uint address, uint maxSize)
        {
            StringBuilder str = new StringBuilder();
            uint bytesLeft = maxSize;

            try
            {
                uint data;
                do
                {
                    data = readMemoryDelegate(address++, ARMPluginInterfaces.MemorySize.Byte);
                    if (data == 0)
                        break;
                    str.Append((char)data);
                } while (--bytesLeft > 0);
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Error while reading string from memory:" + ex.Message);
                str.Length = 0;
            }//catch
            return str.ToString();
        }//loadStringFromMemory

        /// <summary>
        /// Sign extend an integer
        /// </summary>
        /// <param name="val">value to extend</param>
        /// <returns>value sign extended</returns>
		static public uint signExtend(uint value)
		{
            if (value == 0)
                return 0;

            uint mask = value;
			while(!Utils.msb(mask))
			{
				mask = (mask << 1) | value;
			}
			return mask;
        }//signExtend

        /// <summary>
        /// Raise an integer to the power of 2
        /// </summary>
        /// <param name="exp">number to raise</param>
        /// <returns>result</returns>
		static public uint powerOf2( uint exp )
		{
			return (uint)( 1 << (int)exp );
        }//powerOf2

        /// <summary>
        /// Compute the log base 2 of an integer
        /// Assumes that only 1 bit is set in the integer
        /// </summary>
        /// <param name="val">number to compute log base 2</param>
        /// <returns>result</returns>
		static public uint logBase2(uint value)
		{
            if (value == 0)
                return 0;

			uint count = 0;
            while ((value & 0x0001) == 0)
			{
                value = value >> 1;
				count++;
			}
			return count;
        }//logBase2

        /// <summary>
        /// Determine if an address is word aligned.
        /// Check if bottom 2 bits are 0.
        /// </summary>
        /// <param name="address">address to check</param>
        /// <returns>true if aligned</returns>
        static public bool isValidWordAlignedAddress(uint address)
        {
            return ((address & (bit_0|bit_1)) == 0);
        }//isValidWordAlignedAddress

        /// <summary>
        /// Determine if an address is hlaf-word aligned.
        /// Check if bottom bit is 0.
        /// </summary>
        /// <param name="address">address to check</param>
        /// <returns>true if aligned</returns>
        static public bool isValidHalfWordAlignedAddress(uint address)
        {
            return ((address & bit_0) == 0);
        }

        /// <summary>
        /// Check that an address is word aligned.
        /// Check if bottom 2 bits are 0.
        /// </summary>
        /// <param name="address">address to check</param>
        /// <returns>true if aligned</returns>
		static public uint valid_address(uint address)
		{
			return(address & 0xfffffffc);
		}

        /// <summary>
        /// Check if the msb of an integer is set.
        /// </summary>
        /// <param name="val">value to check</param>
        /// <returns>true it is set</returns>
		static public bool msb(uint value)
		{
            return ((value & bit_31) != 0);
		}

        /// <summary>
        /// Check if the lsb of an integer is set.
        /// </summary>
        /// <param name="val">value to check</param>
        /// <returns>true it is set</returns>
        static public bool lsb(uint value)
		{
            return ((value & bit_0) != 0);
		}

        /// <summary>
        /// Check if the 2nd msb of an integer is set.
        /// </summary>
        /// <param name="val">value to check</param>
        /// <returns>true it is set</returns>
        static public bool mmsb(uint value)
		{
            return ((value & bit_30) != 0);
		}

        /// <summary>
        /// Check if the 2nd lsb of an integer is set.
        /// </summary>
        /// <param name="val">value to check</param>
        /// <returns>true it is set</returns>
        static public bool llsb(uint value)
		{
            return ((value & bit_1) != 0);
		}

    }//class Utils
}
