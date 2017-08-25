using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class BlackButtons
    {
        private IOPorts mIOPorts;
        private InteruptController mInteruptController;
        private ARMSim.Plugins.UIControls.BlackButtons mBlackButtons;
        private readonly InteruptController.InterruptTokens mToken=InteruptController.InterruptTokens.BIT_EINT4567;
        public BlackButtons(IOPorts ioPorts, InteruptController interuptController, ARMSim.Plugins.UIControls.BlackButtons blackButtons)
        {
            mIOPorts = ioPorts;
            mInteruptController = interuptController;
            mBlackButtons = blackButtons;
            mBlackButtons.Notify += ButtonPressPortNotify;
        }

        private void ButtonPressPortNotify(object sender, ARMSim.Plugins.UIControls.BlackButtonEventArgs args)
        {
            //0x04 left, 0x08 right
            uint value = mIOPorts.RequestRead(IOPorts.PortNumbers.EXTPND);
            if (args.Button == ARMSim.Plugins.UIControls.BlackButtons.BlackButtonEnum.Left)
            {
                value |= 0x0004;
                mIOPorts.RequestWrite(IOPorts.PortNumbers.EXTPND, value);
            }
            else
            {
                value |= 0x0008;
                mIOPorts.RequestWrite(IOPorts.PortNumbers.EXTPND, value);
            }
            mInteruptController.InteruptNotify(mToken);
        }
    }//class BlackButtons
}
