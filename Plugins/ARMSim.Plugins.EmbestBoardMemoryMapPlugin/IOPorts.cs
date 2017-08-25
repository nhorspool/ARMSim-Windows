using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public delegate void ioPortNotify(IOPorts.PortNumbers portNumber, uint value);

    public class IOPorts
    {
        [Flags]
        public enum PortNumbers
        {
            PCONA = 0x00,
            PDATA = 0x04,
            PCONB = 0x08,
            PDATB = 0x0c,
            PCONC = 0x10,
            PDATC = 0x14,
            PUPC  = 0x18,
            PCOND = 0x1c,
            PDATD = 0x20,
            PUPD  = 0x24,
            PCONE = 0x28,
            PDATE = 0x2c,
            PUPE  = 0x30,
            PCONF = 0x34,
            PDATF = 0x38,
            PUPF  = 0x3c,
            PCONG = 0x40,
            PDATG = 0x44,
            PUPG  = 0x48,
            SPUCR = 0x4c,
            EXTINT = 0x50,
            EXTPND = 0x54
        }

        public class PortDefinition
        {
            public uint mData;
            public event ioPortNotify Notify;

            public bool mInvertedWrite = false;

            //private readonly string mName;
            //private readonly PortDirections mPortDirections;
            private readonly PortNumbers mPortNumber;
            private readonly int mBits;
            public PortDefinition(PortNumbers portNumber, int bits)
            {
                mPortNumber = portNumber;
                mBits = bits;
            }
            public PortDefinition(PortNumbers portNumber, int bits, bool invertedWrite)
                : this(portNumber, bits)
            {
                mInvertedWrite = invertedWrite;
            }

            public string Name { get { return mPortNumber.ToString(); } }

            public uint Read()
            {
                return mData;
            }

            public void Write(uint data)
            {
                if(mInvertedWrite)
                    mData = mData & ~data;
                else
                    mData = data;

                if (Notify != null)
                    Notify(0, data);
            }

            public void WriteNoSideEffect(uint data)
            {
                mData = data;
            }

            public void Restart()
            {
                mData = 0;
            }

        }

        private const uint mLowerAddress = 0x01d20000;
        //private const uint mUpperAddress = 0x01d20054;

        private PortDefinition[] mPortDefinitions = new PortDefinition[22];

        private IARMHost mIhost;
        public IOPorts(IARMHost ihost)
        {
            mIhost = ihost;
            //request to be notified when our area of the memory map is written or read
            mIhost.RequestMemoryBlock(mLowerAddress, 0xffffff80, onMemoryAccessRead, onMemoryAccessWrite);

            mPortDefinitions[0] = new PortDefinition(PortNumbers.PCONA, 10);
            mPortDefinitions[1] = new PortDefinition(PortNumbers.PDATA, 10);
            mPortDefinitions[2] = new PortDefinition(PortNumbers.PCONB, 11);
            mPortDefinitions[3] = new PortDefinition(PortNumbers.PDATB, 11);

            mPortDefinitions[4] = new PortDefinition(PortNumbers.PCONC, 32);
            mPortDefinitions[5] = new PortDefinition(PortNumbers.PDATC, 16);
            mPortDefinitions[6] = new PortDefinition(PortNumbers.PUPC, 16);

            mPortDefinitions[7] = new PortDefinition(PortNumbers.PCOND, 16);
            mPortDefinitions[8] = new PortDefinition(PortNumbers.PDATD, 8);
            mPortDefinitions[9] = new PortDefinition(PortNumbers.PUPD, 8);

            mPortDefinitions[10] = new PortDefinition(PortNumbers.PCONE, 18);
            mPortDefinitions[11] = new PortDefinition(PortNumbers.PDATE, 9);
            mPortDefinitions[12] = new PortDefinition(PortNumbers.PUPE, 8);

            mPortDefinitions[13] = new PortDefinition(PortNumbers.PCONF, 22);
            mPortDefinitions[14] = new PortDefinition(PortNumbers.PDATF, 9);
            mPortDefinitions[15] = new PortDefinition(PortNumbers.PUPF, 9);

            mPortDefinitions[16] = new PortDefinition(PortNumbers.PCONG, 16);
            mPortDefinitions[17] = new PortDefinition(PortNumbers.PDATG, 8);
            mPortDefinitions[18] = new PortDefinition(PortNumbers.PUPG, 8);

            mPortDefinitions[19] = new PortDefinition(PortNumbers.SPUCR, 3);
            mPortDefinitions[20] = new PortDefinition(PortNumbers.EXTINT, 31);
            mPortDefinitions[21] = new PortDefinition(PortNumbers.EXTPND, 4, true);

            this.Restart();
        }//ctor

        public void Restart()
        {
            foreach (PortDefinition pd in mPortDefinitions)
            {
                pd.Restart();
            }
        }

        static private uint PortNumberToIndex(PortNumbers portNumber)
        {
            uint address = (uint)(mLowerAddress + (uint)portNumber);
            uint index = (address - mLowerAddress) >> 2;
            return index;
        }

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            if (mra.Size == MemorySize.Word && Utils.isValidAddress(mra.Address, MemorySize.Word))
            {
                uint index = (mra.Address - mLowerAddress) >> 2;
                if (index < mPortDefinitions.Length)
                    return mPortDefinitions[index].Read();
            }
            return 0;
        }//onMemoryAccessRead

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            if (mwa.Size == MemorySize.Word && Utils.isValidAddress(mwa.Address, MemorySize.Word))
            {
                uint index = (mwa.Address - mLowerAddress) >> 2;
                if (index < mPortDefinitions.Length)
                {
                    mPortDefinitions[index].Write(mwa.Value);
                }
            }
        }//onMemoryAccessWrite

        public void RequestPortNotify(PortNumbers portNumber, ioPortNotify notify)
        {
            mPortDefinitions[PortNumberToIndex(portNumber)].Notify += notify;

        }//RequestPortNotify

        public void RequestWrite(PortNumbers portNumber, uint value)
        {
            mPortDefinitions[PortNumberToIndex(portNumber)].WriteNoSideEffect(value);
        }
        public uint RequestRead(PortNumbers portNumber)
        {
            return mPortDefinitions[PortNumberToIndex(portNumber)].Read();
        }

    }//class IOPorts
}
