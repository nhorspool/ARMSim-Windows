using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class EightSegmentDisplay
    {
        private IARMHost mIHost;
        private ARMSim.Plugins.UIControls.EightSegmentDisplay mEightSegmentDisplay;
        public EightSegmentDisplay(IARMHost ihost, ARMSim.Plugins.UIControls.EightSegmentDisplay eightSegmentDisplay)
        {
            mIHost = ihost;
            mEightSegmentDisplay = eightSegmentDisplay;

            //request reads/writes to the memory address of the display
            mIHost.RequestMemoryBlock(0x02140000, 0xffffffff, this.onMemoryAccessRead, this.onMemoryAccessWrite);
            this.Restart();
        }

        public void Restart()
        {
            mEightSegmentDisplay.Code = 0;
        }

        /// <summary>
        /// This function is called whenever a write access is performed on a reserved block
        /// Input:
        ///  mwa:properties of the write opertaion
        /// </summary>
        /// <param name="mwa"></param>
        public void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            //ignore any non-word writes
            if (mwa.Size != MemorySize.Word)
                return;

            //set the bottom eight bits as the display byte for our control
            mEightSegmentDisplay.Code = (byte)mwa.Value;

        }//onMemoryAccessWrite

        /// <summary>
        /// This function is called whenever a read access is performed on a reserved block
        /// Input:
        ///   mra:properties of the read opertaion
        /// </summary>
        /// <param name="mra"></param>
        /// <returns></returns>
        public uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            //ignore any non-word reads
            if (mra.Size != MemorySize.Word)
                return 0;

            //get the current byte of the control and return it as a word
            return mEightSegmentDisplay.Code;
        }//onMemoryAccessRead

    }//class EightSegmentDisplay
}
