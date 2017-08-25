using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ARMSim.Simulator.Plugins;

namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    public partial class UCPlugins : UserControl
    {
        //private PluginPreferences mPluginPreferences;
        private ICollection<PluginManager.PluginItem> mAvailablePlugins;
        public UCPlugins()
        {
            InitializeComponent();
        }
        public UCPlugins(ICollection<PluginManager.PluginItem> availablePlugins)
            : this()
        {
            //mPluginPreferences = pluginPreferences;
            mAvailablePlugins = availablePlugins;
        }

        private void Plugins_Load(object sender, EventArgs e)
        {
            foreach (PluginManager.PluginItem item in mAvailablePlugins)
            {
                ListViewItem lvi = new ListViewItem(item.armPlugin.PluginName);
                lvi.SubItems.Add(item.Assembly);
                lvi.SubItems.Add(item.armPlugin.PluginDescription);
                lvi.Checked = item.Activated;
                lvi.Tag = item;
                listView1.Items.Add(lvi);
            }//foreach

        }

        public PluginPreferences PluginPreferences
        {
            get
            {
                PluginPreferences pluginPreferences = new PluginPreferences();
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.Checked)
                    {
                        PluginManager.PluginItem pluginItem = lvi.Tag as PluginManager.PluginItem;
                        ARMPluginInterfaces.Preferences.PluginSettingsItem item = new ARMPluginInterfaces.Preferences.PluginSettingsItem(pluginItem.armPlugin.PluginName, pluginItem.Assembly);
                        pluginPreferences.AddPlugin(item);
                    }//if
                }//foreach
                return pluginPreferences;
            }//get
        }

        //public void SaveSettings()
        //{
        //    mPluginPreferences.SettingsPlugins.Clear();
        //    foreach (ListViewItem lvi in listView1.Items)
        //    {
        //        if (lvi.Checked)
        //        {
        //            PluginManager.PluginItem pluginItem = lvi.Tag as PluginManager.PluginItem;
        //            PluginPreferences.PluginSettingsItem item = new PluginPreferences.PluginSettingsItem(pluginItem.armPlugin.PluginName, pluginItem.Assembly);
        //            mPluginPreferences.AddPlugin(item);
        //        }//if
        //    }//foreach
        //}

    }//class Plugins
}
