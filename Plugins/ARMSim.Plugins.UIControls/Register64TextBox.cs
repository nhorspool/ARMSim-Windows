using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class Register64TextBox : UserControl
    {
        private bool mBase10;
        private long mCurrentValue;

        public Register64TextBox()
        {
            InitializeComponent();
            this.Value = 0;
        }

        private void UpdateRegister()
        {
            string text;
            if (mBase10)
            {
                text = Value.ToString();
            }
            else
            {
                string str = Value.ToString("X16");
                text = str.Substring(0, 8) + " " + str.Substring(8);
            }
            textBox1.Text = text;
        }

        public long Value
        {
            get
            {
                return mCurrentValue;
            }
            set
            {
                mCurrentValue = value;
                UpdateRegister();
            }
        }

        private void base10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mBase10 = true;
            UpdateRegister();
        }

        private void hexideimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mBase10 = false;
            UpdateRegister();
        }
    }
}