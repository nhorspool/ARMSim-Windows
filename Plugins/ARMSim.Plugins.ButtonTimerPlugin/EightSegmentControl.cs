using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    /// <summary>
    /// Represents the 8 segment display being simulated.
    /// Access to the 8 segment display is provided through a single word of memory.
    /// The 8 bottom(LSB) bits control the 8 segment display pattern.
    /// </summary>
    public class EightSegmentControl
    {
        /// <summary>
        /// The current 8 segment display state
        /// </summary>
        private uint mEightSegmentValue;

        /// <summary>
        /// Reference to the ARMSim host interface. Passed in through ctor
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// A reference to the 8 segment display control that actual displays the Segments in the UI. Passed in through ctor
        /// </summary>
        private EightSegmentDisplay mEightSegmentDisplay;

        /// <summary>
        /// Control ctor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="eightSegmentDisplay"></param>
        public EightSegmentControl(IARMHost host, EightSegmentDisplay eightSegmentDisplay)
        {
            //cache the simulator host interface and the LEDs user interface
            mHost = host;
            mEightSegmentDisplay = eightSegmentDisplay;

            //request to be notified when our area of the memory map is written or read
            mHost.RequestMemoryBlock(0x01d10000, 0xffffffff, onMemoryAccessRead, onMemoryAccessWrite);

            //request simulator restart notification so we can reset the 8 segment display to its default state
            mHost.Restart += mHost_Restart;

            //Force 8 segment display to a default state.
            init();
        }

        /// <summary>
        /// Resets 8 segment display to its default value (blank)
        /// </summary>
        private void init()
        {
            mEightSegmentValue = 0;
            UpdateUserInterface();
        }

        /// <summary>
        /// This function is called when the simulator has been restarted by the user.
        /// Reset the 8 segment display to its default state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Restart(object sender, EventArgs args)
        {
            init();
        }

        /// <summary>
        /// This function is called when the memory locations declared by the 8 segment display has been read.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mra"></param>
        /// <returns></returns>
        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            // only word access allowed
            if (mra.Size == MemorySize.Word)
            {
                return mEightSegmentValue;
            }
            return 0;
        }//onMemoryAccessRead

        /// <summary>
        /// This function is called when the memory locations declared by the 8 segment display has been written.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mwa"></param>
        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            // only word access allowed
            if (mwa.Size == MemorySize.Word)
            {
                //obtain the bottom 8 bits of the value written
                uint newValue = (uint)(mwa.Value & 0xff);

                //Note:this test is to avoid excessive updating of the 8 segment display. If the state of the
                //8 segment display does not change, dont update UI
                //if the value has changed, notify UI to update itself.
                if (mEightSegmentValue != newValue)
                {
                    //save new value
                    mEightSegmentValue = newValue;
                    UpdateUserInterface();
                }//if

            }//if
        }//onMemoryAccessWrite

        /// <summary>
        /// Helper funtion to update UI for new 8 segment display state
        /// </summary>
        private void UpdateUserInterface()
        {
            mEightSegmentDisplay.Code = (byte)mEightSegmentValue;
        }

    }//class EightSegmentControl
}
