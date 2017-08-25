using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;
using System.IO.Ports;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class Uarts
    {
        static uint mLowerAddress = 0x01d00000;
        //0x01d04028

        //private string[] mPortNames;
        private Uart mUart0 = new Uart();
        private Uart mUart1 = new Uart();

        private IARMHost mIhost;
        private InteruptController mInteruptController;
        public Uarts(IARMHost ihost, InteruptController interuptController)
        {
            mIhost = ihost;
            mInteruptController = interuptController;
            mIhost.RequestMemoryBlock(mLowerAddress, 0xffff0000, onMemoryAccessRead, onMemoryAccessWrite);

            mUart0.Create("COM1");
            mUart1.Create("COM2");

            this.Restart();
        }

        public void Restart()
        {
            //mPortNames = SerialPort.GetPortNames();
            mUart0.Restart();
            mUart1.Restart();
        }

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            if (mra.Size == MemorySize.Word && Utils.isValidAddress(mra.Address, MemorySize.Word))
            {
                Uart uart = ((mra.Address & 0x01d04000) == 0) ? mUart0 : mUart1;
                if (uart == null)
                    return 0;

            }
            return 0;
        }//onMemoryAccessRead

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            if (mwa.Size != MemorySize.Word || Utils.isValidAddress(mwa.Address, MemorySize.Word))
                return;

            Uart uart = ((mwa.Address & 0x01d04000) == 0) ? mUart0 : mUart1;
            switch (mwa.Address & 0x01d00000)
            {
                //ULCON:Uart Line Control Register
                case 0x01d00000:
                    uart.ULCON = mwa.Value;
                    break;

                //UCON:Uart Control Register
                case 0x01d00004:
                    uart.UCON = mwa.Value;
                    break;

                //UFCON:Uart FIFO Control Register
                case 0x01d00008:
                    uart.UFCON = mwa.Value;
                    break;

                //UMCON:Uart Modem Control Register
                case 0x01d0000c:
                    uart.UMCON = mwa.Value;
                    break;

                //UTRSTAT:Uart Tx/Rx Status Register (read-only)
                case 0x01d00010:
                    break;

                //UERSTAT:Uart Error Status Register
                case 0x01d00014:
                    uart.UERSTAT = mwa.Value;
                    break;

                //UFSTAT:Uart FIFO Status Register (read-only)
                case 0x01d00018:
                    //uart.UFSTAT = mwa.Value;
                    break;

                //UMSTAT:Uart Modem Status Register
                case 0x01d0001c:
                    //uart.UMSTAT = mwa.Value;
                    break;

                //UTXH:Uart Transmit Holding(Buffer) Register
                case 0x01d00020:
                    uart.UTXH = mwa.Value;
                    break;

                //URXH:Uart Receive Holding(Buffer) Register
                case 0x01d00024:
                    //uart.URXH = mwa.Value;
                    break;

                //UBRDIV:Uart Baud Rate Division Register
                case 0x01d00028:
                    uart.UBRDIV = mwa.Value;
                    break;


            }

        }//onMemoryAccessWrite

    }//class Uarts

    public class Uart
    {
        private bool mInfraRedMode;
        private bool mRxTimeout;
        private bool mRxErrorInt;
        private bool mLoopBackMode;
        private bool mTxInterrupt;
        private bool mRxInterrupt;

        private uint mULCON;
        private uint mUCON;
        private uint mUFCON;
        private uint mUMCON;
        private uint mUERSTAT;
        //private uint mUMSTAT;
        //private uint mUTXH;
        //private uint mURXH;
        private uint mUBRDIV;

        private bool mFIFOEnabled;
        private FIFO mTxFIFO = new FIFO();
        private FIFO mRxFIFO = new FIFO();
        private SerialPort mSerialPort;

        public Uart()
        {
            this.Restart();
        }

        public void Restart()
        {
            mULCON = 0;
            mUCON = 0;
            mUFCON = 0;
            mUMCON = 0;
            mUERSTAT = 0;
            //mUMSTAT = 0;
            //mUTXH = 0;
            //mURXH = 0;
            mUBRDIV = 0;

            mInfraRedMode = false;
            mRxTimeout = false;
            mRxErrorInt = false;
            mLoopBackMode = false;
            mTxInterrupt = false;
            mRxInterrupt = false;

            mTxFIFO.Reset();
            mRxFIFO.Reset();
            mFIFOEnabled = false;
        }

        public bool Create(string name)
        {
            try
            {
                mSerialPort = new SerialPort(name);
                return true;
            }
            catch (System.IO.IOException ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Could not intialize " + name + " Reason:" + ex.Message);
                mSerialPort = null;
                return false;
            }
        }

        public uint ULCON
        {
            get { return mULCON; }
            set
            {
                mULCON = value;

                if (mSerialPort == null)
                    return;

                mInfraRedMode = ((mULCON & 0x0400) != 0);

                switch (mULCON & 0x38)
                {
                    case 0x00:
                    case 0x01:
                    case 0x02:
                    case 0x03:
                        mSerialPort.Parity = Parity.None;
                        break;
                    case 0x04:
                        mSerialPort.Parity = Parity.Odd;
                        break;
                    case 0x05:
                        mSerialPort.Parity = Parity.Even;
                        break;
                    case 0x06:
                        mSerialPort.Parity = Parity.Mark;
                        break;
                    case 0x07:
                        mSerialPort.Parity = Parity.Space;
                        break;
                }//switch

                mSerialPort.StopBits = ((mULCON & 0x04) == 0) ? StopBits.One : StopBits.Two;
                mSerialPort.DataBits = (int)((mULCON & 0x03) + 5);

            }
        }//ULCON

        public uint UCON
        {
            get { return mUCON; }
            set
            {
                mUCON = value;

                if (mSerialPort == null)
                    return;

                mRxTimeout = ((mUCON & 0x80) != 0);
                mRxErrorInt = ((mUCON & 0x40) != 0);
                mLoopBackMode = ((mUCON & 0x20) != 0);

                mTxInterrupt = ((mUCON & 0xc0) == 0x04);
                mRxInterrupt = ((mUCON & 0x03) == 0x01);

            }
        }//UCON

        public uint UFCON
        {
            get { return mUFCON; }
            set
            {
                mUFCON = value;

                if (mSerialPort == null)
                    return;

                if ((mUFCON & 0x40) != 0)
                {//Tx FIFO reset
                    mTxFIFO.Reset();
                }
                if ((mUFCON & 0x20) != 0)
                {//Rx FIFO reset
                    mRxFIFO.Reset();
                }

                mTxFIFO.TriggerLevel = (FIFO.TriggerLevels)((mUFCON & 0xc0) >> 6);
                mRxFIFO.TriggerLevel = (FIFO.TriggerLevels)((mUFCON & 0x30) >> 4);

                mFIFOEnabled = ((mUFCON & 0x01) != 0);

            }
        }//UFCON

        public uint UMCON
        {
            get { return mUMCON; }
            set
            {
                mUMCON = value;
            }
        }//UMCON

        //read-only
        public uint UTRSTAT
        {
            get
            {
                if (mSerialPort == null)
                    return 0;

                uint result = 0;

                if (mTxFIFO.IsEmpty)
                    result |= 0x60;

                if (!mRxFIFO.IsEmpty)
                    result |= 0x01;

                return result;
            
            }
        }//UTRSTAT

        //read-only
        public uint UERSTAT
        {
            get
            {//These bits (UERSAT[3:0]) are automatically cleared to 0 when the UART error status register is read.
                uint result = mUERSTAT;
                mUERSTAT = 0;
                return result;
            }
            set
            {
                mUERSTAT = value;
            }
        }//UERSTAT

        //read-only
        public uint UFSTAT
        {
            get
            {
                uint result = 0;

                if (mTxFIFO.IsFull)
                    result |= 0x200;
                if (mRxFIFO.IsFull)
                    result |= 0x100;

                result |= ((uint)mTxFIFO.Count & 0x0f) << 4;
                result |= ((uint)mRxFIFO.Count & 0x0f);

                return result;
            }
        }//UFSTAT

        //read-only
        public uint UMSTAT
        {
            get { return 0; }
        }//UMSTAT

        //write-only
        public uint UTXH
        {
            set
            {
                if (!this.mFIFOEnabled)
                {
                    //if(mSerialPort

                }

                if (!mTxFIFO.IsFull)
                    mTxFIFO.Add(value);
                //mUTXH = value;
            }
        }//UTXH

        //read-only
        public uint URXH
        {
            get
            {
                uint dummy = this.UERSTAT;
                if (mRxFIFO.IsEmpty)
                    return 0;

                FIFOItem fifoItem = mRxFIFO.Read();

                uint result = 0;
                if (fifoItem.BreakError)
                    result |= 0x80;
                if (fifoItem.FrameError)
                    result |= 0x40;
                if (fifoItem.ParityError)
                    result |= 0x20;

                this.UERSTAT = result;

                return fifoItem.Data;
            }
        }//URXH

        public uint UBRDIV
        {
            get { return mUBRDIV; }
            set
            {
                mUBRDIV = value;
            }
        }//UBRDIV

    }//class Uart

    public class FIFOItem
    {
        public byte Data { get; set; }
        public bool BreakError { get; set; }
        public bool ParityError { get; set; }
        public bool FrameError { get; set; }
    }

    public class FIFO
    {
        private FIFOItem[] mData = new FIFOItem[16];
        private int mCurrentIndex;
        public TriggerLevels TriggerLevel { get; set; }

        public FIFO()
        {
            for (int ii = 0; ii < mData.Length; ii++)
            {
                mData[ii] = new FIFOItem();
            }
            this.TriggerLevel = TriggerLevels.Empty;
        }

        public int Count { get { return mCurrentIndex; } }

        public bool IsEmpty
        {
            get
            {
                return (mCurrentIndex == 0);
            }
        }

        public bool IsFull
        {
            get
            {
                return (mCurrentIndex == 16);
            }
        }

        public void Reset()
        {
            for (int ii = 0; ii < mData.Length; ii++)
            {
                mData[ii].Data = 0;
                mData[ii].BreakError = false;
                mData[ii].ParityError = false;
                mData[ii].FrameError = false;
            }
            mCurrentIndex = 0;
            this.TriggerLevel = TriggerLevels.Empty;
        }//Restart

        public FIFOItem Read()
        {
            if (this.IsEmpty)
                return null;

            mCurrentIndex--;
            return mData[mCurrentIndex];
        }

        public void Add(uint value)
        {
            if (this.IsFull)
                return;

            mData[mCurrentIndex].Data = (byte)value;
            mData[mCurrentIndex].BreakError = false;
            mData[mCurrentIndex].ParityError = false;
            mData[mCurrentIndex].FrameError = false;
            mCurrentIndex++;
        }

        public enum TriggerLevels
        {
            Empty = 0,
            FourByte = 1,
            EightByte = 2,
            TwelveByte = 3
        }

    }//class FIFO


}
