using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class EmbestBoardMemoryMapPlugin : IARMPlugin
    {
        private IARMHost mIHost;

        private InteruptController mInteruptController;
        private WatchDogTimer mWatchDogTimer;
        private PWMTimer mPWMTimer;
        private IOPorts mIOPorts;
        private LcdDisplay mLcdDisplay;
        private EightSegmentDisplay mEightSegmentDisplay;
        private Leds mLeds;
        private BlackButtons mBlackButtons;
        private BlueButtons mBlueButtons;
        private Uarts mUarts;

        /// <summary>
        /// The init function is called once the plugin has been loaded.
        /// From this function you can subscribe to the events the
        /// simulator supports.
        /// </summary>
        /// <param name="iHost"></param>
        public void Init(IARMHost iHost)
        {
            mIHost = iHost;
            mIHost.Load += onLoad;
        }//init

        /// <summary>
        /// The onLoad function is called after all the plugins have been loaded and their
        /// init methods called.
        /// </summary>
        private void onLoad(object sender, EventArgs e)
        {
            ARMSim.Plugins.UIControls.EmbestBoard embestBoardControl = new ARMSim.Plugins.UIControls.EmbestBoard();
            InterruptControllerDisplay interruptControllerDisplay = new InterruptControllerDisplay(mInteruptController);

            mInteruptController = new InteruptController(mIHost, interruptControllerDisplay);
            mWatchDogTimer = new WatchDogTimer(mIHost, mInteruptController);
            mPWMTimer = new PWMTimer(mIHost, mInteruptController);
            mIOPorts = new IOPorts(mIHost);
            mLeds = new Leds(mIHost, mIOPorts, embestBoardControl.Leds);
            mBlackButtons = new BlackButtons(mIOPorts, mInteruptController, embestBoardControl.BlackButtons);
            mBlueButtons = new BlueButtons(mIHost, mIOPorts, mInteruptController, embestBoardControl.BlueButtons);
            mLcdDisplay = new LcdDisplay(mIHost, embestBoardControl.Lcd);
            mEightSegmentDisplay = new EightSegmentDisplay(mIHost, embestBoardControl.EightSegmentDisplay);
            mUarts = new Uarts(mIHost, mInteruptController);

            mIHost.RequestPanel(this.PluginName).Controls.Add(embestBoardControl);
            mIHost.RequestPanel("InterruptControllerDisplay").Controls.Add(interruptControllerDisplay);

            mIHost.Restart += onRestart;

        }//onLoad

        private void onRestart(object sender, EventArgs args)
        {
            mInteruptController.Restart();
            mWatchDogTimer.Restart();
            mPWMTimer.Restart();
            mIOPorts.Restart();
            mLeds.Restart();
            mLcdDisplay.Restart();
            mEightSegmentDisplay.Restart();
        }//onRestart

        /// <summary>
        /// This property is the Name of the plugin. Plugins names must be unique in the host assembly.
        /// </summary>
        public string PluginName { get { return "EmbestBoardMemoryMapPlugin"; } }

        /// <summary>
        /// This property is the Description string of the plugin. This can be any text that describes
        /// the plugin.
        /// </summary>
        public string PluginDescription { get { return "Simulates the Embest S3CE40 development board through the Memory Map"; } }

    }//class EmbestBoardMemoryMapPlugin
}
