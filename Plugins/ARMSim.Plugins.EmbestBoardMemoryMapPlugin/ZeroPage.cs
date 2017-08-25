using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class ZeroPage
    {
        private IARMHost mIhost;
        private uint[] mMemory = new uint[256 / 4];

        public ZeroPage(IARMHost ihost)
        {
            mIhost = ihost;
        }

        public void onLoad()
        {
            mIhost.RequestMemoryBlock(0x00000000, 0xffffff00, onMemoryAccessRead, onMemoryAccessWrite);

        }

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            if (mra.Size != MemorySize.Word)
                return 0;

            return mMemory[mra.Address / 4];
        }

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            if (mwa.Size != MemorySize.Word)
                return;

            return;
        }


    }
}
