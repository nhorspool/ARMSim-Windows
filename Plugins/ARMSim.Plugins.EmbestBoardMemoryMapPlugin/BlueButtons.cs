using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class BlueButtons
    {
        private readonly uint keyboard_base = 0x06000000;

        private byte[] mLocs = new byte[4];

        private IARMHost mIhost;
        private IOPorts mIOPorts;
        private InteruptController mInteruptController;
        private ARMSim.Plugins.UIControls.BlueButtons mBlueButtons;
        private readonly InteruptController.InterruptTokens mToken=InteruptController.InterruptTokens.BIT_EINT1;
        public BlueButtons(IARMHost ihost, IOPorts ioPorts, InteruptController interuptController, ARMSim.Plugins.UIControls.BlueButtons blueButtons)
        {
            mIhost = ihost;
            mIOPorts = ioPorts;
            mInteruptController = interuptController;
            mBlueButtons = blueButtons;
            mBlueButtons.Notify += ButtonPressPortNotify;

            //request to be notified when our area of the memory map is written or read
            mIhost.RequestMemoryBlock(keyboard_base, 0xffffff00, onMemoryAccessRead, onMemoryAccessWrite);

        }

        private void ButtonPressPortNotify(object sender, ARMSim.Plugins.UIControls.BlueButtonEventArgs args)
        {
            Array.Clear(mLocs, 0, mLocs.Length);
            byte bits = (byte)(0x01 << (3 - args.XPos));
            mLocs[args.YPos] = (byte)(~bits & 0x0f);
            mInteruptController.InteruptNotify(mToken);
        }

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            switch (mra.Address - keyboard_base)
            {
                case 0xfd:
                    return mLocs[0];
                case 0xfb:
                    return mLocs[1];
                case 0xf7:
                    return mLocs[2];
                case 0xef:
                    return mLocs[3];
                default:
                    return 0xf;

            }
        }

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
        }

    }//class BlueButtons
}
