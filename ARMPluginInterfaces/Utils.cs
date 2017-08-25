using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    /// <summary>
    /// This is a collection of useful utilities for both the ARMSim simulator and the plugins.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Load a string from memory. Read bytes from memory until a 0 is found.
        /// </summary>
        /// <param name="ihost">Simulator reference</param>
        /// <param name="address">Address to load from</param>
        /// <param name="maxSize">max number of bytes to load</param>
        /// <returns>string read from memory</returns>
        public static string loadStringFromMemory(IARMHost ihost, uint address, uint maxSize)
        {
            StringBuilder str = new StringBuilder();
            uint bytesLeft = maxSize;

            try
            {
                uint data;
                do
                {
                    data = ihost.GetMemory(address++, MemorySize.Byte);
                    if (data == 0) break;
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
        /// Convert an array of bools to a bit pattern.
        /// </summary>
        /// <param name="bools">array of bools</param>
        /// <returns>bit pattern</returns>
        public static uint BoolsToBits(bool[] bools)
        {
            uint ret = 0;
            uint mask = 0x01;

            foreach (bool b in bools)
            {
                if (b)
                    ret |= mask;
                mask = mask << 1;
            }
            return ret;
        }

        /// <summary>
        /// Test if a given bit(s) are set in a word.
        /// </summary>
        /// <param name="address">word to test</param>
        /// <param name="mask">bits to test against</param>
        /// <returns></returns>
        public static bool bitsSet(uint address, uint mask)
        {
            return ((address & mask) != 0);
        }

        /// <summary>
        /// Test if the least significant bit is set.
        /// </summary>
        /// <param name="address">word to test</param>
        /// <returns>true if set</returns>
        public static bool lsb(uint address)
        {
            return bitsSet(address, 0x01);
        }

        /// <summary>
        /// Test if second to least significant bit is set
        /// </summary>
        /// <param name="address">word to test</param>
        /// <returns>true if set</returns>
        public static bool ls2b(uint address)
        {
            return bitsSet(address, 0x03);
        }

        /// <summary>
        /// Test if most significant bit is set.
        /// </summary>
        /// <param name="address">word to test</param>
        /// <returns>true if set</returns>
        public static bool msb(uint address)
        {
            return bitsSet(address, 0x80000000);
        }//msb

        /// <summary>
        /// Test if a given address is aligned on a given size
        /// </summary>
        /// <param name="address">address to test</param>
        /// <param name="ms">memory size to test</param>
        /// <returns>true if aligned</returns>
        public static bool isValidAddress(uint address, MemorySize ms)
        {
            if (ms == MemorySize.Byte || (ms == MemorySize.HalfWord && !lsb(address)))
                return true;
            return (!ls2b(address));
        }//isValidAddress


        /// <summary>
        /// A useful debugging function
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="parms"></param>
        public static void OutputDebugString(string fmt, params object[] parms)
        {
            string str = String.Format(fmt, parms);
            System.Diagnostics.Debug.WriteLine(str);
        }

    }//class Utils
}