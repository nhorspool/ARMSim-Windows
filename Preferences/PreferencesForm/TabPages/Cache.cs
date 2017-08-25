using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    public partial class ARMCache : UserControl
    {
        private CachePreferences mCachePreferences;

        public ARMCache()
        {
            InitializeComponent();
        }

        public ARMCache(CachePreferences cachePreferences) : this()
        {
            mCachePreferences = cachePreferences;
        }

        private void cbUnifiedCache_CheckedChanged(object sender, EventArgs e)
        {
            instructionCache.Enabled = !cbUnifiedCache.Checked;
            dataCache.Enabled = true;
            dataCache.Title = cbUnifiedCache.Checked ? "Cache" : "DataCache";
        }

        private void Cache_Load(object sender, EventArgs e)
        {
            instructionCache.Preferences = mCachePreferences.InstructionCachePreferences;
            dataCache.Preferences = mCachePreferences.DataCachePreferences;
            cbUnifiedCache.Checked = mCachePreferences.UnifiedCache;

        }

        //public void SaveSettings()
        //{
        //    mCachePreferences.UnifiedCache = cbUnifiedCache.Checked;
        //    mCachePreferences.InstructionCachePreferences = instructionCache.Preferences;
        //    mCachePreferences.DataCachePreferences = (DataCachePreferences)dataCache.Preferences;

        //}

        public CachePreferences CachePreferences
        {
            get
            {
                CachePreferences cachePreferences = new CachePreferences();
                cachePreferences.UnifiedCache = cbUnifiedCache.Checked;
                cachePreferences.InstructionCachePreferences = instructionCache.Preferences;
                cachePreferences.DataCachePreferences = (DataCachePreferences)dataCache.Preferences;
                return cachePreferences;
            }
        }

        /// <summary>
        /// Invoke the cache settings wizard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCacheWizard_Click(object sender, EventArgs e)
        {
            using (ARMSim.Preferences.PreferencesForm.Wizard.CacheWizard cacheWizard =
                new ARMSim.Preferences.PreferencesForm.Wizard.CacheWizard(mCachePreferences))
            {
                if (cacheWizard.ShowDialog() == DialogResult.OK)
                {
                    mCachePreferences = cacheWizard.CachePreferences;
                    this.Cache_Load(null, null);
                }
            }
        }

        /// <summary>
        /// Restore all cache settings to default values.
        /// These are basically all disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            mCachePreferences.defaultSettings();
            this.Cache_Load(null, null);
        }

    }//class Cache
}
