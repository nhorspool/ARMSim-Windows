using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    public partial class ButtonTimerDisplay : UserControl
    {
        public ButtonTimerDisplay()
        {
            InitializeComponent();
        }

        public ushort Timer1Value { set { timValue1.Value = value; } }
        public ushort Timer1Control { set { timControl1.Value = value; } }
        public ushort Timer1Count { set { timCount1.Value = value; } }

        public ushort Timer2Value { set { timValue2.Value = value; } }
        public ushort Timer2Control { set { timControl2.Value = value; } }
        public ushort Timer2Count { set { timCount2.Value = value; } }

        public LedsDisplay LedsDisplay { get { return leds1; } }
        public BlackButtonsDisplay BlackButtons { get { return blackButtons1; } }
        public EightSegmentDisplay EightSegmentDisplay { get { return eightSegmentDisplay1; } }
        public TwoLineLCDDisplay TwoLineLCDDisplay { get { return twoLineLCDDisplay1; } }
        public TrackBar TrackBarControl { get { return trackBar1; } }
        public Label TrackBarLabel { get { return lblTrackbar; } }
        
    }
}
