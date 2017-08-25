using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ARMSim.Preferences;

namespace ARMSim.Preferences.PreferencesForm.Wizard
{
    public partial class CacheWizard : Form
    {
        private CachePreferences mCachePreferences;
        public CacheWizard(CachePreferences cachePreferences)
        {
            InitializeComponent();

            mCachePreferences = cachePreferences;

            rbUnifiedYes.Checked = mCachePreferences.UnifiedCache;
            rbUnifiedNo.Checked = !mCachePreferences.UnifiedCache;

            {
                InstructionCachePreferences icp = mCachePreferences.InstructionCachePreferences;
                rbICacheEnableYes.Checked = icp.Enabled;
                rbICacheEnableNo.Checked = !icp.Enabled;

                instructionCacheSize.Set(icp.BlockSize, icp.NumberBlocks);
                instructionAssociativity.Set(icp.NumberBlocks, icp.BlocksPerSet);
                instructionReplacementStrategy.ReplaceStrategyType = icp.ReplaceStrategy;
            }

            {
                DataCachePreferences dcp = mCachePreferences.DataCachePreferences;
                rbDCacheEnableYes.Checked = dcp.Enabled;
                rbDCacheEnableNo.Checked = !dcp.Enabled;

                dataCacheSize.Set(dcp.BlockSize, dcp.NumberBlocks);
                dataAssociativity.Set(dcp.NumberBlocks, dcp.BlocksPerSet);
                dataReplacementStrategy.ReplaceStrategyType = dcp.ReplaceStrategy;

                writePolicy.WritePolicyType = dcp.WritePolicy;
                allocatePolicy.AllocatePolicyType = dcp.AllocatePolicy;
            }

        }


        public CachePreferences CachePreferences
        {
            get
            {
                CachePreferences cachePreferences = new CachePreferences();
                cachePreferences.UnifiedCache = rbUnifiedYes.Checked;

                {
                    InstructionCachePreferences icp = cachePreferences.InstructionCachePreferences;
                    icp.Enabled = rbICacheEnableYes.Checked;

                    if (icp.Enabled)
                    {
                        icp.BlockSize = instructionCacheSize.BlockSize;
                        icp.NumberBlocks = instructionCacheSize.NumberBlocks;
                        icp.BlocksPerSet = instructionAssociativity.Blocks;
                        icp.ReplaceStrategy = instructionReplacementStrategy.ReplaceStrategyType;
                    }
                }
                {
                    DataCachePreferences dcp = cachePreferences.DataCachePreferences;
                    dcp.Enabled = rbDCacheEnableYes.Checked;
                    if (dcp.Enabled)
                    {
                        dcp.BlockSize = dataCacheSize.BlockSize;
                        dcp.NumberBlocks = dataCacheSize.NumberBlocks;
                        dcp.BlocksPerSet = dataAssociativity.Blocks;
                        dcp.ReplaceStrategy = dataReplacementStrategy.ReplaceStrategyType;

                        dcp.WritePolicy = writePolicy.WritePolicyType;
                        dcp.AllocatePolicy = allocatePolicy.AllocatePolicyType;
                    }
                }

                return cachePreferences;
            }
        }


        private void Summary()
        {
            CachePreferences cachePreferences = this.CachePreferences;
            this.cacheSummary1.SetSettings(cachePreferences);
            this.cacheSummary2.SetSettings(cachePreferences);
        }

        private void wizard1_BeforeSwitchPages(object sender, Wizard.BeforeSwitchPagesEventArgs e)
        {
            bool forward = e.NewIndex == (e.OldIndex + 1);

            switch (e.OldIndex)
            {
                case 1://Unified
                    if (forward && rbUnifiedYes.Checked)
                    {
                        e.NewIndex = 6;//DCache enabled
                    }
                    break;
                case 2://ICache Enable
                    if (forward && rbICacheEnableNo.Checked)
                    {
                        e.NewIndex = 6;//DCache enabled
                    }
                    break;

                case 6://DCache Enable
                    if (forward && rbDCacheEnableNo.Checked)
                    {
                        e.NewIndex = 12;//Summary
                    }
                    else if (!forward && rbUnifiedYes.Checked)
                    {
                        e.NewIndex = 1;//Unified
                    }
                    else if (!forward && rbUnifiedNo.Checked && rbICacheEnableNo.Checked)
                    {
                        e.NewIndex = 2;//ICache enabled
                    }
                    break;

                case 12://Summary
                    if (!forward && rbDCacheEnableNo.Checked)
                    {
                        e.NewIndex = 6;
                    }
                    break;
            }

            if (e.NewIndex == 12)
            {
                Summary();
            }//if

        }//wizard1_BeforeSwitchPages

    }//class CacheWizard
}