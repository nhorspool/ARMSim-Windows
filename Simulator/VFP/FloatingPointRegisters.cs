#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;

namespace ARMSim.Simulator.VFP
{
    /// <summary>
    /// delegate defines a callback when a CPU register is written to
    /// </summary>
    /// <param name="reg">register that changed</param>
    public delegate void FPRegisterChangedDelegate(uint reg);

	/// <summary>
	/// This class represent the floating point registers of the ARM VFP
    /// There are 16 double or 32 sinle precision registers that over overlapped
    /// with each other.
	/// </summary>
	public class FloatingPointRegisters
	{
        private FPRegisterChangedDelegate _registerChangedHandler;

		//the 16 double precision fp registers
        //or 32 single precision fp registers(overlayed)
		private byte [] _bytes = new byte[8*16];

        /// <summary>
        /// FloatingPointRegisters ctor
        /// construct the fp area, make all bytes 0
        /// </summary>
		public FloatingPointRegisters()
		{
            this.reset();
		}

        /// <summary>
        ///Determines if a particular register is a Nan or not
        ///Input:
        ///reg - register number to check
        ///singleType - true for single
        /// </summary>
        /// <param name="reg">register to test</param>
        /// <param name="singleType">true if single</param>
        /// <returns>true if NaN</returns>
        public bool isNaN(uint reg, bool singleType)
        {
            if (singleType)
                return float.IsNaN(this.ReadS(reg));
            else
                return double.IsNaN(this.ReadD(reg));
        }//isNaN

        /// <summary>
        /// Read a double precision register
        /// </summary>
        /// <param name="reg">register to read</param>
        /// <returns>double value read</returns>
        public double ReadD(uint reg)
        {
            return (reg > 15) ? 0 : BitConverter.ToDouble(_bytes, (int)(reg * 8));
        }//ReadD

        /// <summary>
        /// Write a double precision register
        /// </summary>
        /// <param name="reg">register to write</param>
        /// <param name="value">double value to write</param>
        public void WriteD(uint reg, double value)
        {
            if (reg <= 15)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Copy(bytes, 0, _bytes, (int)(reg * 8), bytes.Length);

                //notify and listeners that a fp register has changed
                //call twice, once for each single register
                fireFPRegisterChanged(reg*2);
                fireFPRegisterChanged((reg*2)+1);
            }//if
        }//WriteD

        /// <summary>
        /// Read a single precision register
        /// </summary>
        /// <param name="reg">register to read</param>
        /// <returns>float value read</returns>
        public float ReadS(uint reg)
        {
            return (reg > 31) ? 0 : BitConverter.ToSingle(_bytes, (int)(reg * 4));
        }//ReadS

        /// <summary>
        /// Write a single precision register
        /// </summary>
        /// <param name="reg">register to write</param>
        /// <param name="value">float value to write</param>
        public void WriteS(uint reg, float value)
        {
            if (reg <= 31)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Copy(bytes, 0, _bytes, (int)(reg * 4), bytes.Length);

                //notify and listeners that a fp register has changed
                fireFPRegisterChanged(reg);
            }//if
        }//WriteS

        /// <summary>
        /// Write a word as is into the fp register buffer. The number is not translated
        /// to a fp number, is copied in exactly as an integer.
        /// Input:
        /// reg - register number to write to(single precision)
        /// data - uint value to write
        /// </summary>
        /// <param name="reg">register to write</param>
        /// <param name="data">data to write</param>
        public void WriteRaw(uint reg, uint data)
        {
            byte[] bytes = BitConverter.GetBytes(data);                 //convert uint to byte stream
            Array.Copy(bytes, 0, _bytes, (int)(reg * 4), bytes.Length); //copy bytes into fp buffer

            //notify listeners that register has changed
            fireFPRegisterChanged(reg);
        }//WriteRaw

        /// <summary>
        /// Read a uint from the fp buffer. No tranlslation, take the bits out exactly as they are.
        /// </summary>
        /// <param name="reg">register to read</param>
        /// <returns>data read</returns>
        public uint ReadRaw(uint reg)
        {
            return BitConverter.ToUInt32(_bytes, (int)(reg * 4));
        }//ReadRaw

        /// <summary>
        /// Write a word to the low/high word of a double precision register. No translation is made,
        /// copy exactly as is.
        /// Input:
        /// reg - register number to write to(double precision)
        /// data - uint data to write
        /// lowWord - true if bits 0:31 are to be written to
        /// </summary>
        /// <param name="reg">register to write</param>
        /// <param name="data">data to write</param>
        /// <param name="lowWord">true if low word</param>
        public void WriteRaw(uint reg, uint data, bool lowWord)
        {
            byte[] bytes = BitConverter.GetBytes(data);             //get byte stream of data
            int offset = (int)(reg * 8);                            //compute offset
            if (!lowWord) offset += 4;                              //adust if high word
            Array.Copy(bytes, 0, _bytes, offset, bytes.Length);     //copy bytes

            //notify and listeners that a fp register has changed
            //call twice, once for each single register
            fireFPRegisterChanged(reg * 2);
            fireFPRegisterChanged((reg*2)+1);
        }//WriteRaw

        /// <summary>
        /// Read a word from the low/high word of a double precision register. No translation is made,
        /// copy exactly as is.
        /// Input:
        /// reg - register number to read from(double precision)
        /// data - uint data to read
        /// lowWord - true if bits 0:31 are to be read
        /// </summary>
        /// <param name="reg">register to read</param>
        /// <param name="lowWord">true if low word</param>
        /// <returns>data read</returns>
        public uint ReadRaw(uint reg, bool lowWord)
        {
            int offset = (int)(reg * 8);                    //compute offset into fp buffer
            if (!lowWord) offset += 4;                      //adjust if high word
            return BitConverter.ToUInt32(_bytes, offset);   //copy bytes
        }//ReadRaw

        /// <summary>
        /// reset floating point registers. clear array
        /// </summary>
        public void reset()
        {
            Array.Clear(_bytes, 0, _bytes.Length);
        }//reset

        /// <summary>
        /// call the register changed callback if it is set
        /// </summary>
        /// <param name="reg">register that changed</param>
        private void fireFPRegisterChanged(uint reg)
        {
            if (_registerChangedHandler != null)
                _registerChangedHandler(reg);
        }//fireFPRegisterChanged

        /// <summary>
        /// property to set the fp register changed callback
        /// </summary>
        public FPRegisterChangedDelegate RegisterChangedHandler
        {
            set { _registerChangedHandler = value; }
        }
    }//class FloatingPointRegisters
}
