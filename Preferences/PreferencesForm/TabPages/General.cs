using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    public partial class General : UserControl
    {
        private GeneralPreferences mGeneralPreferences;
        public General()
        {
            InitializeComponent();
        }
        public General(GeneralPreferences generalPreferences) : this()
        {
            mGeneralPreferences = generalPreferences;
        }

        private void General_Load(object sender, EventArgs e)
        {
            try
            {
                cbSync.Checked = mGeneralPreferences.SyncCacheOnExit;
                cbCloseFiles.Checked = mGeneralPreferences.CloseFilesOnExit;
                tbStdinFilename.Text = mGeneralPreferences.StdinFileName;
                tbStdoutFilename.Text = mGeneralPreferences.StdoutFileName;
                tbStderrFilename.Text = mGeneralPreferences.StderrFileName;
                rbStdoutOverwrite.Checked = mGeneralPreferences.StdoutOverwrite;
                rbStdoutAppend.Checked = !rbStdoutOverwrite.Checked;
                rbStderrOverwrite.Checked = mGeneralPreferences.StderrOverwrite;
                rbStderrAppend.Checked = !rbStderrOverwrite.Checked;
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception in preferences load General:" + ex.Message);
                mGeneralPreferences.defaultSettings();
            }

        }

        public GeneralPreferences GeneralPreferences
        {
            get
            {
                GeneralPreferences generalPreferences = new GeneralPreferences();
                generalPreferences.SyncCacheOnExit = cbSync.Checked;
                generalPreferences.CloseFilesOnExit = cbCloseFiles.Checked;
                generalPreferences.StdinFileName = tbStdinFilename.Text;
                generalPreferences.StdoutFileName = tbStdoutFilename.Text;
                generalPreferences.StderrFileName = tbStderrFilename.Text;
                generalPreferences.StdoutOverwrite = rbStdoutOverwrite.Checked;
                generalPreferences.StderrOverwrite = rbStderrOverwrite.Checked;
                return generalPreferences;
            }
        }

        private void btnBrowseStdin_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbStdinFilename.Text = openFileDialog1.FileName;
            }
        }

        private void btnBrowseStdout_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbStdoutFilename.Text = openFileDialog1.FileName;
            }
        }

        private void btnBrowseStderr_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbStderrFilename.Text = openFileDialog1.FileName;
            }
        }

        //public void SaveSettings()
        //{
        //    mGeneralPreferences.SyncCacheOnExit = cbSync.Checked;
        //    mGeneralPreferences.CloseFilesOnExit = cbCloseFiles.Checked;
        //    mGeneralPreferences.StdinFilename = tbStdinFilename.Text;
        //    mGeneralPreferences.StdoutFilename = tbStdoutFilename.Text;
        //    mGeneralPreferences.StderrFilename = tbStderrFilename.Text;
        //    mGeneralPreferences.StdoutOverwrite = rbStdoutOverwrite.Checked;
        //    mGeneralPreferences.StderrOverwrite = rbStderrOverwrite.Checked;
        //}

    }//class General
}