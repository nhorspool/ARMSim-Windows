using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    public class ButtonTimer : IARMPlugin
    {
        /// <summary>
        /// Reference to the ARMSim host interface. Aquired in the Init method.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// These controls hold the logic of the simulated hardware
        /// </summary>
        private TimerControl mTimerControl;                             // the 2 timers
        private LedsControl mLedsControl;                               // the 2 leds
        private BlackButtonsControl mBlackButtonsControl;               // the 2 black buttons
        private EightSegmentControl mEightSegmentControl;               // the eight segment display
        private TwoLineLCDControl mTwoLineLCDControl;                   // the 2 line lcd display
        private TrackBarControl mTrackBarControl;

        /// <summary>
        /// This user control houses the plugin user interface
        /// </summary>
        private ButtonTimerDisplay mButtonTimerDisplay;

        /// <summary>
        /// The Init method of the plugin. This method is called when the plugin has been created by ARMSim.
        /// ARMSim will pass an instance of IARMHost to the plugin so that the plugin can make requests back to ARMSim.
        /// </summary>
        /// <param name="ihost"></param>
        public void Init(IARMHost ihost)
        {
            //cache the interface to ARMSim
            mHost = ihost;

            //set the load handler so we get called when all the plugins are loaded
            mHost.Load += onLoad;

        }//Init

        /// <summary>
        /// The onLoad is called after all plugins have had their init called and are all loaded.
        /// </summary>
        private void onLoad(object sender, EventArgs args)
        {
            //request a panel to display our user interface in from the simulator
            Panel panel = mHost.RequestPanel(this.PluginName);

            //create the plugin user control and add it to the requested panel
            mButtonTimerDisplay = new ButtonTimerDisplay();
            panel.Controls.Add(mButtonTimerDisplay);

            //Create the timer control and initialize it
            mTimerControl = new TimerControl(mHost, mButtonTimerDisplay);

            //Create the leds control and initialize it
            mLedsControl = new LedsControl(mHost, mButtonTimerDisplay.LedsDisplay);

            //Create the black buttons control and initialize it
            mBlackButtonsControl = new BlackButtonsControl(mHost, mButtonTimerDisplay.BlackButtons);

            //Create the eight segment control and initialize it
            mEightSegmentControl = new EightSegmentControl(mHost, mButtonTimerDisplay.EightSegmentDisplay);

            mTwoLineLCDControl = new TwoLineLCDControl(mHost, mButtonTimerDisplay.TwoLineLCDDisplay);

            mTrackBarControl = new TrackBarControl(mHost, mButtonTimerDisplay.TrackBarControl, mButtonTimerDisplay.TrackBarLabel);

        }//onLoad

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin name
        /// </summary>
        public string PluginName { get { return "ButtonTimer"; } }

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin description
        /// </summary>
        public string PluginDescription { get { return "Simulates a push button and timer"; } }

    }//class ButtonTimer
}
