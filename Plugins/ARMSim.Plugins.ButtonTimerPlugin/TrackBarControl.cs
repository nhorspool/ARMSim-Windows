using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    public class TrackBarControl
    {
        /// <summary>
        /// This is the base memory mapped address of the trackbar.
        /// When read, this register provides the current value of the bar.
        /// It is an 8 bit value 0 -255
        /// </summary>
        private uint mBaseAddress = 0x01a00000;

        /// <summary>
        /// A reference to the simulation host that we can request services from.
        /// </summary>
        private IARMHost mHost;

        private TrackBar mTrackBar;
        private Label mTrackBarLabel;

        public TrackBarControl(IARMHost host, TrackBar trackBar, Label trackBarLabel)
        {
            mHost = host;
            mTrackBar = trackBar;
            mTrackBarLabel = trackBarLabel;

            //request to be notified when our area of the memory map is read. We don't care about writes so use null.
            mHost.RequestMemoryBlock(mBaseAddress, 0xffffffff, onMemoryAccessRead, null);

            //get notified when the simulation is restarted so we can put buttons back to its initial state
            mHost.Restart += mHost_Restart;

            //set notification for scroll events on trackbar
            mTrackBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);

            //and make sure display is init
            init();
        }

        /// <summary>
        /// Called when the simulation is restarted
        /// We need to make sure buttons are in the initial state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Restart(object sender, EventArgs args)
        {
            init();
        }//mHost_Restart

        /// <summary>
        /// Set buttons to intitial power-on state
        /// </summary>
        private void init()
        {
            mTrackBar.Value = 0;
            mTrackBarLabel.Text = "0";
        }//init

        /// <summary>
        /// This function is called when a memory read has occurred on the button id register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mra"></param>
        /// <returns></returns>
        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            //we only allow word access
            if (mra.Size == MemorySize.Word)
            {
                return (uint)mTrackBar.Value;
            }
            return 0;
        }//onMemoryAccessRead

        /// <summary>
        /// This function is called when the trackbar is scrolled. Update the label that
        /// shows its current value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            mTrackBarLabel.Text = mTrackBar.Value.ToString();
        }

    }//class TrackBarControl
}
