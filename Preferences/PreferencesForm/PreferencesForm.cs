using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ARMSim.Simulator;

namespace ARMSim.Preferences.PreferencesForm
{
    public partial class PreferencesForm : Form
    {
        private TabPages.General mGeneral;
        private TabPages.MainMemory mMainMemory;
        private TabPages.ARMCache mCache;
        private TabPages.UCPlugins mPlugins;

        private ARMPreferences mPreferences;
        private ApplicationJimulator mApplicationJimulator;

        public PreferencesForm(ARMPreferences preferences, ApplicationJimulator applicationJimulator)
        {
            mPreferences = preferences;
            mApplicationJimulator = applicationJimulator;
            InitializeComponent();
        }

        public ARMPreferences Preferences { get { return mPreferences; } }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            TabPage tb = new TabPage();
            tb.Name = tb.Text = "General";
            mGeneral = new TabPages.General(mPreferences.GeneralPreferences);
            mGeneral.Dock = DockStyle.Fill;
            tb.Controls.Add(mGeneral);
            this.tabControl1.TabPages.Add(tb);

            tb = new TabPage();
            tb.Name = tb.Text = "Main Memory";
            mMainMemory = new TabPages.MainMemory(mPreferences.SimulatorPreferences);
            mMainMemory.Dock = DockStyle.Fill;
            tb.Controls.Add(mMainMemory);
            this.tabControl1.TabPages.Add(tb);

            tb = new TabPage();
            tb.Name = tb.Text = "Cache";
            mCache = new TabPages.ARMCache(mPreferences.CachePreferences);
            mCache.Dock = DockStyle.Fill;
            tb.Controls.Add(mCache);
            this.tabControl1.TabPages.Add(tb);

            tb = new TabPage();
            tb.Name = tb.Text = "Plugins";
            mPlugins = new TabPages.UCPlugins(mApplicationJimulator.AvailablePlugins);
            mPlugins.Dock = DockStyle.Fill;
            tb.Controls.Add(mPlugins);
            this.tabControl1.TabPages.Add(tb);

            TabPage tab = tabControl1.TabPages[mPreferences.LastTab];
            if (tab != null)
            {
                tabControl1.SelectTab(mPreferences.LastTab);
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            mPreferences.LastTab = tabControl1.SelectedTab.Text;

            mPreferences.GeneralPreferences = mGeneral.GeneralPreferences;
            mPreferences.SimulatorPreferences = mMainMemory.SimulatorPreferences;
            mPreferences.CachePreferences = mCache.CachePreferences;
            mPreferences.PluginPreferences = mPlugins.PluginPreferences;
            this.Close();
        }

    }//class PreferencesForm
}
