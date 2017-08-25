using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.Controls
{
    public partial class DataCache : UserControl
    {
        private bool mInstructionCache;

        public DataCache()
        {
            InitializeComponent();
            cacheSize.BlocksChanged += associativity.numberBlocksChangedHandler;
        }

        public string Title { get { return groupBox1.Text; } set { groupBox1.Text = value; } }

        [
        Category("Appearance"),
        Description("Sets the title of the control group box."),
        DefaultValue(false),
        ]
        public bool InstructionCache
        {
            get { return mInstructionCache; }
            set
            {
                mInstructionCache = value;
                writePolicy.Visible = !mInstructionCache;
                allocatePolicy.Visible = !mInstructionCache;

                Size dialogSize = this.Size;

                if (mInstructionCache)
                {
                    dialogSize.Height = allocatePolicy.Location.Y + 5;
                    this.Title = "Instruction Cache";
                }
                else
                {
                    dialogSize.Height = allocatePolicy.Location.Y + allocatePolicy.Size.Height + 5;
                    this.Title = "Data Cache";
                }
                this.Size = dialogSize;
            }
        }//InstructionCache

        public InstructionCachePreferences Preferences
        {
            get
            {
                if (mInstructionCache)
                {
                    InstructionCachePreferences icp = new InstructionCachePreferences();
                    icp.Enabled = this.cbCacheEnable.Checked;
                    icp.BlockSize = cacheSize.BlockSize;
                    icp.NumberBlocks = cacheSize.NumberBlocks;
                    icp.BlocksPerSet = associativity.Blocks;
                    icp.ReplaceStrategy = replacementStrategy.ReplaceStrategyType;
                    return icp;
                }
                else
                {
                    DataCachePreferences dcp = new DataCachePreferences();
                    dcp.Enabled = this.cbCacheEnable.Checked;
                    dcp.BlockSize = cacheSize.BlockSize;
                    dcp.NumberBlocks = cacheSize.NumberBlocks;
                    dcp.BlocksPerSet = associativity.Blocks;
                    dcp.ReplaceStrategy = replacementStrategy.ReplaceStrategyType;
                    dcp.WritePolicy = writePolicy.WritePolicyType;
                    dcp.AllocatePolicy = allocatePolicy.AllocatePolicyType;
                    return dcp;
                }
            }
            set
            {
                this.cbCacheEnable.Checked = value.Enabled;
                cacheSize.Set(value.BlockSize, value.NumberBlocks);
                associativity.Set(value.NumberBlocks, value.BlocksPerSet);
                replacementStrategy.ReplaceStrategyType = value.ReplaceStrategy;
                if (!mInstructionCache)
                {
                    DataCachePreferences dcp = (value as DataCachePreferences);
                    writePolicy.WritePolicyType = dcp.WritePolicy;
                    allocatePolicy.AllocatePolicyType = dcp.AllocatePolicy;
                }
            }
        }

        private void cbCacheEnable_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cbCacheEnable.Checked;
            cacheSize.Enabled = enable;
            associativity.SetInternalEnabled(enable);
            replacementStrategy.Enabled = enable;
            writePolicy.Enabled = enable;
            allocatePolicy.Enabled = enable;
        }

    }//class DataCache
}
