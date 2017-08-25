using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class EmbestBoard : UserControl
    {
        public EmbestBoard()
        {
            InitializeComponent();
        }

        public ARMSim.Plugins.UIControls.EightSegmentDisplay EightSegmentDisplay { get { return eightSegmentDisplayControl1; } }
        public ARMSim.Plugins.UIControls.Leds Leds { get { return leds1; } }
        public ARMSim.Plugins.UIControls.BlackButtons BlackButtons { get { return blackButtons1; } }
        public ARMSim.Plugins.UIControls.BlueButtons BlueButtons { get { return blueButtons1; } }
        public ARMSim.Plugins.UIControls.Lcd Lcd { get { return lcd1; } }

    }
}
