using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    /// <summary>
    /// Represents a text box that contains a 16 bit hexadecimal number.
    /// </summary>
    public partial class TimerDisplay : UserControl
    {
        /// <summary>
        /// Current number being displayed
        /// </summary>
        private ushort mValue;

        public TimerDisplay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Access to the current number.
        /// </summary>
        public ushort Value
        {
            get
            {
                return mValue;
            }//get
            set
            {
                mValue = value;
                lblTimerValue.Text = mValue.ToString("X4");
            }//set
        }//Value

    }//class TimerDisplay
}
