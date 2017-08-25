using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    /// <summary>
    /// Represents the 2 LEDs being simulated.
    /// Access to the LEDs is provided through a single word of memory.
    /// The 2 bottom(LSB) bits control the 2 LEDs.
    /// </summary>
    public class LedsControl
    {
        /// <summary>
        /// The current LED states
        /// </summary>
        private uint mLedsState;

        /// <summary>
        /// Reference to the ARMSim host interface. Passed in through ctor
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// A reference to the LED display control that actual displays the LEDs in the UI. Passed in through ctor
        /// </summary>
        private LedsDisplay mLedsDisplay;

        /// <summary>
        /// Control ctor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="ledsDisplay"></param>
        public LedsControl(IARMHost host, LedsDisplay ledsDisplay)
        {
            //cache the simulator host interface and the LEDs user interface
            mHost = host;
            mLedsDisplay = ledsDisplay;

            //request to be notified when our area of the memory map is written or read
            mHost.RequestMemoryBlock(0x01d20000, 0xffffffff, onMemoryAccessRead, onMemoryAccessWrite);

            //request simulator restart notification so we can reset the LEDs to their default state
            mHost.Restart += mHost_Restart;

            //init LEDs to default state
            init();
        }

        /// <summary>
        /// Resets LEDs to their default state(both off)
        /// </summary>
        private void init()
        {
            mLedsState = 0;
            UpdateUserInterface();
        }

        /// <summary>
        /// This function is called when the simulator has been restarted by the user.
        /// Reset the LEDs to their default state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Restart(object sender, EventArgs args)
        {
            init();
        }//mHost_Restart

        /// <summary>
        /// This function is called when the memory locations declared by the LEDs have been read.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mra"></param>
        /// <returns></returns>
        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            // only word access allowed
            if (mra.Size == MemorySize.Word)
            {
                return mLedsState;
            }
            return 0;
        }//onMemoryAccessRead

        /// <summary>
        /// This function is called when the memory locations declared by the LEDs have been written.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mwa"></param>
        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            // only word access allowed
            if (mwa.Size == MemorySize.Word)
            {
                //obtain the bottom 2 bits of the value written
                uint newValue = (uint)(mwa.Value & 0x03);

                //Note:this test is to avoid excessive updating of the leds. If the state of the
                //LEDs does not change, dont update UI
                //if the value has changed, notify UI to update itself.
                if (mLedsState != newValue)
                {
                    //save new value
                    mLedsState = newValue;
                    UpdateUserInterface();
                }
            }//if
        }//onMemoryAccessWrite

        /// <summary>
        /// Helper funtion to update UI for new LED state
        /// </summary>
        private void UpdateUserInterface()
        {
            mLedsDisplay.LeftLed = ((mLedsState & 0x02) != 0);
            mLedsDisplay.RightLed = ((mLedsState & 0x01) != 0);
        }

    }//class LedsControl
}