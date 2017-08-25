using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class BlueButtons : UserControl
    {
        private bool[,] mClicked = new bool[4, 4];

        public event BlueButtonPressNotify Notify;

        public BlueButtons()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string text = button.Text;

            //3,4
            int xpos = int.Parse(text.Substring(0, 1));
            int ypos = int.Parse(text.Substring(2, 1));
            mClicked[xpos, ypos] = true;

            if (Notify != null)
                Notify(this, new BlueButtonEventArgs(xpos, ypos));

        }//button_Click

        //checks if any of the blue buttons has been pressed.
        //returns the result in a single 32bit integer
        //any button presses are cleared
        public uint CheckPressed()
        {
            uint mask = 0x01;
            uint ret = 0x00;

            for (int ii = 0; ii < 4; ii++)
            {
                for (int jj = 0; jj < 4; jj++)
                {
                    if (mClicked[ii, jj])
                    {
                        mClicked[ii, jj] = false;
                        ret |= mask;
                    }
                    mask <<= 1;
                }//for jj
            }//for ii
            return ret;
        }
    }

    public class BlueButtonEventArgs : EventArgs
    {
        private readonly int mXpos;
        private readonly int mYpos;
        public BlueButtonEventArgs(int xpos, int ypos)
        {
            mXpos = xpos;
            mYpos = ypos;
        }
        public int XPos { get { return mXpos; } }
        public int YPos { get { return mYpos; } }
    }
    public delegate void BlueButtonPressNotify(object sender, BlueButtonEventArgs args);

}
