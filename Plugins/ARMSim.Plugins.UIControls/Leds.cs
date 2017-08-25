using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class Leds : UserControl
    {
        private bool mLeftLedState;
        private bool mRightLedState;

        public Leds()
        {
            InitializeComponent();
            this.LeftLed = false;
            this.RightLed = false;
        }

        public bool LeftLed
        {
            get { return mLeftLedState; }
            set
            {
                mLeftLedState = value;
                pictureBox1.Image = mLeftLedState ? this.imageList1.Images[1] : this.imageList1.Images[0];
            }
        }

        public bool RightLed
        {
            get { return mRightLedState; }
            set
            {
                mRightLedState = value;
                pictureBox2.Image = mRightLedState ? this.imageList1.Images[1] : this.imageList1.Images[0];
            }
        }

    }
}
