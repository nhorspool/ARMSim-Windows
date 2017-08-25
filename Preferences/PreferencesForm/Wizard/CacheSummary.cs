using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.Wizard
{
    public partial class CacheSummary : UserControl
    {
        private bool mInstructionCacheSummary;

        public CacheSummary()
        {
            InitializeComponent();
        }

        public void SetSettings(CachePreferences cachePreferences)
        {
            InstructionCachePreferences cp = mInstructionCacheSummary ? cachePreferences.InstructionCachePreferences : cachePreferences.DataCachePreferences;

            if (!cp.Enabled)
            {
                gbSummary.Enabled = false;
                return;
            }
            lblCacheSize.Text = (cp.NumberBlocks * cp.BlockSize).ToString() + " Bytes";
            lblBlockSize.Text = cp.BlockSize.ToString() + " Bytes";
            lblNumBlocks.Text = cp.NumberBlocks.ToString();

            if (cp.NumberBlocks == 1)
                lblAssociativity.Text = "Fully Associative";
            else if (cp.NumberBlocks == cp.BlocksPerSet)
                lblAssociativity.Text = "Direct Mapped";
            else
                lblAssociativity.Text = string.Format("{0} way", cp.BlocksPerSet);

            lblReplacement.Text = cp.ReplaceStrategy.ToString();

            if (!mInstructionCacheSummary)
            {
                DataCachePreferences dcp = cachePreferences.DataCachePreferences;
                lblAllocate.Text = dcp.AllocatePolicy.ToString();
                lblWrite.Text = dcp.WritePolicy.ToString();
            }


        }//SetSettings

        [
        Category("Appearance"),
        Description("Sets the title of the control group box."),
        DefaultValue(false),
        ]
        public bool InstructionCache
        {
            get { return mInstructionCacheSummary; }
            set
            {
                mInstructionCacheSummary = value;

                label21.Visible = !mInstructionCacheSummary;
                label22.Visible = !mInstructionCacheSummary;
                lblAllocate.Visible = !mInstructionCacheSummary;
                lblWrite.Visible = !mInstructionCacheSummary;

                gbSummary.Text = mInstructionCacheSummary ? "Instruction Cache" : "Data Cache";

            }
        }//InstructionCache


    }
}
