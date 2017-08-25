using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.Controls
{
    public class PowerOf2 : System.Windows.Forms.NumericUpDown
    {
        public enum UpDownDirection
        {
            Up,
            Down
        }

        public delegate void UpDownDelegate(PowerOf2 sender, UpDownDirection direction);
        public event UpDownDelegate UpDownHandler;
        //private UpDownDelegate _UpDownDelegate;

        public PowerOf2()
        {
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.ReadOnly = true;

            this.Maximum = decimal.MaxValue;
            this.Minimum = 0;
        }

        //public UpDownDelegate UpDownHandler
        //{
        //    get { return _UpDownDelegate; }
        //    set { _UpDownDelegate += value; }
        //}

        public override void DownButton()
        {
            if (UpDownHandler != null)
            {
                UpDownHandler(this, UpDownDirection.Down);
            }
        }

        public override void UpButton()
        {
            if (UpDownHandler != null)
            {
                UpDownHandler(this, UpDownDirection.Up);
            }
        }

    }
}
