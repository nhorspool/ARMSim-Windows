using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class Leds
    {
        private IARMHost mIhost;
        private IOPorts mIOPorts;
        private ARMSim.Plugins.UIControls.Leds mLedsControl;
        public Leds(IARMHost ihost, IOPorts ioPorts, ARMSim.Plugins.UIControls.Leds ledsControl)
        {
            mIhost = ihost;
            mIOPorts = ioPorts;
            mLedsControl = ledsControl;

            mIOPorts.RequestPortNotify(IOPorts.PortNumbers.PDATB, ioPortNotify);
            this.Restart();
        }

        public void Restart()
        {
            mLedsControl.LeftLed = false;
            mLedsControl.RightLed = false;
        }

        private void ioPortNotify(IOPorts.PortNumbers portNumber, uint value)
        {
            mLedsControl.LeftLed = (((value >> 9) & 0x01) != 0);
            mLedsControl.RightLed = (((value >> 10) & 0x01) != 0);
        }

    }//class Leds
}
