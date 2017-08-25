using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    public partial class MainMemory : UserControl
    {
        private SimulatorPreferences mSimulatorPreferences;
        public MainMemory()
        {
            InitializeComponent();
        }
        public MainMemory(SimulatorPreferences simulatorPreferences) : this()
        {
            mSimulatorPreferences = simulatorPreferences;
        }

        private void MainMemory_Load(object sender, EventArgs e)
        {
            try
            {
                nudStartAddress.Value = mSimulatorPreferences.MemoryStart;
                nudStackArea.Value = mSimulatorPreferences.StackAreaSize;
                nudHeapArea.Value = mSimulatorPreferences.HeapAreaSize;
                this.FillValue = mSimulatorPreferences.FillPattern;
                cbMisalignedStop.Checked = mSimulatorPreferences.StopOnMisaligned;

                if (mSimulatorPreferences.StackGrowsDown)
                    rbStackGrowsDown.Checked = true;
                else
                    rbStackGrowsUp.Checked = true;

                cbProtectTextArea.Checked = mSimulatorPreferences.ProtectTextArea;

            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception in preferences load MainMemory:" + ex.Message);
                mSimulatorPreferences.defaultSettings();
            }
        }

        private void hexFillPattern_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            char ch = (char)e.KeyChar;
            if (!char.IsDigit(ch))
            {
                char upper = char.ToUpper(ch);
                if (upper < 'A' || upper > 'F')
                {
                    if (upper != '\b')
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        public uint FillValue
        {
            get
            {
                string str = this.hexFillPattern.Text.Trim();
                if (string.IsNullOrEmpty(str))
                    return 0;

                return Convert.ToUInt32(this.hexFillPattern.Text, 16);
            }
            set { this.hexFillPattern.Text = value.ToString("x8"); }
        }

        public SimulatorPreferences SimulatorPreferences
        {
            get
            {
                SimulatorPreferences simulatorPreferences = new SimulatorPreferences();
                simulatorPreferences.MemoryStart = (uint)nudStartAddress.Value;
                simulatorPreferences.StackAreaSize = (uint)nudStackArea.Value;
                simulatorPreferences.HeapAreaSize = (uint)nudHeapArea.Value;
                simulatorPreferences.FillPattern = this.FillValue;
                simulatorPreferences.StopOnMisaligned = cbMisalignedStop.Checked;
                simulatorPreferences.StackGrowsDown = rbStackGrowsDown.Checked;
                simulatorPreferences.ProtectTextArea = cbProtectTextArea.Checked;
                return simulatorPreferences;
            }
        }

    }//class MainMemory
}