using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public partial class InterruptControllerDisplay : UserControl
    {
        private static string[] mChannelNames = new string[]
                    {"EINT0", "EINT1", "EINT2", "EINT3", "EINT4567",  "TICK", "INT_ZDMA0", "INT_ZDMA1",
                     "INT_BDMA0", "INT_BDMA1", "INT_WDT", "INT_UERR01", "INT_TIMER0",  "INT_TIMER1",
                     "INT_TIMER2", "INT_TIMER3", "INT_TIMER4", "INT_TIMER5", "INT_URXD0",  "INT_URXD1",
                     "INT_IIC", "INT_SIO", "INT_UTXD0", "INT_UTXD1", "INT_RTC",  "INT_ADC"};


        public InteruptController InteruptController { get; set; }

        public InterruptControllerDisplay(InteruptController interuptController)
        {
            this.InteruptController = interuptController;
            InitializeComponent();
            this.init();
        }

        private void init()
        {
            for (int ii = 0; ii < 26; ii++)
            {
                this.dataGridView1.Rows.Add(new DataGridViewRow());
                DataGridViewRow dr = this.dataGridView1.Rows[ii];
                dr.Cells[0].Value = mChannelNames[ii];
            }//for ii
            this.UpdateInterruptControllerDisplay();
        }

        public InterruptControllerDisplay()
        {
            InitializeComponent();
            this.init();
        }

        public void UpdateInterruptControllerDisplay()
        {
            if (this.InteruptController == null)
                return;

            uint current = this.InteruptController.Current;
            uint pending = this.InteruptController.Pending;
            uint mask = this.InteruptController.Mask;
            uint mode = this.InteruptController.Mode;
            for (int ii = 25; ii >= 0; ii--)
            {
                DataGridViewRow dr = this.dataGridView1.Rows[ii];
                dr.Cells[5].Value = ((current & 0x01) == 0) ? "0" : "1";
                dr.Cells[4].Value = ((mask & 0x01) == 0) ? "0" : "1";
                dr.Cells[2].Value = ((pending & 0x01) == 0) ? "No" : "Yes";
                dr.Cells[3].Value = ((mode & 0x01) == 0) ? "IRQ" : "FIQ";
                dr.Cells[1].Value = (25-ii).ToString();

                current >>= 1;
                pending >>= 1;
                mask >>= 1;
                mode >>= 1;
            }//for ii

            lblVectorMode.Text = this.InteruptController.VectorMode ? "Yes" : "No";
            lblIRQEnabled.Text = this.InteruptController.IRQEnabled ? "Yes" : "No";
            lblFIQEnabled.Text = this.InteruptController.FIQEnabled ? "Yes" : "No";

        }

    }//class InterruptControllerDisplay
}
