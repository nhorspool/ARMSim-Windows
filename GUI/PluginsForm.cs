using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//using ARMSim.Simulator;

namespace ARMSim.GUI
{
    public partial class PluginsForm : Form
    {
        private ICollection<ARMSim.Simulator.PluginManager.PluginManager.PluginItem> mAvailablePlugins;

        public ICollection<ARMSim.Simulator.PluginManager.PluginManager.PluginItem> SelectedPlugins
        {
            get
            {
                List<ARMSim.Simulator.PluginManager.PluginManager.PluginItem> selectedPlugins = new List<ARMSim.Simulator.PluginManager.PluginManager.PluginItem>();

                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.Checked)
                    {
                        selectedPlugins.Add(lvi.Tag as ARMSim.Simulator.PluginManager.PluginManager.PluginItem);
                    }
                }
                return selectedPlugins;
            }
        }

        public PluginsForm(ICollection<ARMSim.Simulator.PluginManager.PluginManager.PluginItem> availablePlugins)
        {
            mAvailablePlugins = availablePlugins;
            InitializeComponent();
        }

        private void PluginsForm_Load(object sender, EventArgs e)
        {
            foreach(ARMSim.Simulator.PluginManager.PluginManager.PluginItem item in mAvailablePlugins)
            {
                ListViewItem lvi = new ListViewItem(item.armPlugin.Name);
                lvi.SubItems.Add(item.Assembly);
                lvi.SubItems.Add(item.armPlugin.Description);
                lvi.Checked = item.Activated;
                lvi.Tag = item;
                listView1.Items.Add(lvi);
            }
        }

    }//class PluginsForm
}
