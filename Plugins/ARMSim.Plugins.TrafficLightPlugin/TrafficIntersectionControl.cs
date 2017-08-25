using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.TrafficLightPlugin
{
    public partial class TrafficIntersectionControl : UserControl
    {
        private bool mSideButtonPressed;
        private bool mMainButtonPressed;

        public TrafficIntersectionControl()
        {
            InitializeComponent();
        }

        public ARMSim.Plugins.UIControls.WalkSignal MainWalkSignal { get { return mainWalkSignal; } }
        public ARMSim.Plugins.UIControls.TrafficLight MainTrafficLight { get { return mainTrafficLight; } }

        public ARMSim.Plugins.UIControls.WalkSignal SideWalkSignal { get { return sideWalkSignal; } }
        public ARMSim.Plugins.UIControls.TrafficLight SideTrafficLight { get { return sideTrafficLight; } }

        public bool GetMainButtonPressed()
        {
            bool pressed = mMainButtonPressed;
            mMainButtonPressed = false;
            return pressed;
        }

        public bool GetSideButtonPressed()
        {
            bool pressed = mSideButtonPressed;
            mSideButtonPressed = false;
            return pressed;
        }

        public void Reset()
        {
            mainWalkSignal.State = ARMSim.Plugins.UIControls.WalkSignal.XWalkStates.DONTWALK;
            mainTrafficLight.State = ARMSim.Plugins.UIControls.TrafficLight.TrafficLightStates.BLACK;

            sideWalkSignal.State = ARMSim.Plugins.UIControls.WalkSignal.XWalkStates.DONTWALK;
            sideTrafficLight.State = ARMSim.Plugins.UIControls.TrafficLight.TrafficLightStates.BLACK;

            mMainButtonPressed = mSideButtonPressed = false;


        }

        private void mainButton_Click(object sender, EventArgs e)
        {
            mMainButtonPressed = true;
        }

        private void sideButton_Click(object sender, EventArgs e)
        {
            mSideButtonPressed = true;
        }

    }
}
