using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.GUI
{
    public partial class MultipleFileOpen : Form
    {
        private string _mruDir;

        public MultipleFileOpen(IList<string> loadedFiles)
        {
            InitializeComponent();

            //only interested in non null case and more than 0 files
            if (loadedFiles != null && loadedFiles.Count > 0)
            {
                //load each file into list box
                foreach (string str in loadedFiles)
                {
                    listBox1.Items.Add(str);
                }//foreach
                //get the current directory from the last file in list
                _mruDir = loadedFiles[loadedFiles.Count - 1];

            }//if
            updateRemoveButton();
        }//MultipleFileOpen ctor

        /// <summary>
        /// Return the list of files on the files listbox
        /// </summary>
        public IList<string> OpenFiles
        {
            get
            {
                IList<string> strs = new List<string>();
                for (int ii = 0; ii < listBox1.Items.Count; ii++)
                {
                    strs.Add(listBox1.Items[ii] as string);
                }
                return strs;
            }
        }//OpenFiles

        /// <summary>
        /// Enable or Disable the remove button.
        /// </summary>
        private void updateRemoveButton()
        {
            btnRemove.Enabled = (listBox1.SelectedIndex != -1);
            btnOK.Enabled = (listBox1.Items.Count > 0);
        }//updateRemoveButton

        /// <summary>
        /// The selected file from the files list box has changed. Update the remove button
        /// (Disable it of no file selected)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateRemoveButton();
        }

        /// <summary>
        /// Open the add files dialog to let the user select file(s) to add to the files listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _mruDir;
            openFileDialog.Filter   // added all files option - nigel
                = "All Recognized Formats (*.s, *.o, *.a)|*.s; *.o; *.a|Source files (*.s)|*.s|Object files (*.o)|*.o|Libraries (*.a)|*.a|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            //added multi file select - dale
            openFileDialog.Multiselect = true;

            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //set current directory to first file - dale
                foreach (string fileName in openFileDialog.FileNames)
                {   // we allow libraries to appear multiple times in the list
                    if (!listBox1.Items.Contains(fileName) || fileName.EndsWith(".a") || fileName.EndsWith(".A"))
                        listBox1.Items.Add(fileName);
                    _mruDir = fileName;
                }
            }//if
            updateRemoveButton();
        }//btnAdd_Click

        /// <summary>
        /// Allow the user to reorder the files -- it matters when there are libraries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			int location = listBox1.SelectedIndex;
			if (location > 0)
			{
			    object rememberMe = listBox1.SelectedItem;
			    listBox1.Items.RemoveAt(location);
			    listBox1.Items.Insert(location - 1, rememberMe);
			    listBox1.SelectedIndex = location - 1;
			}
            updateRemoveButton();
        }

        /// <summary>
        /// Allow the user to reorder the files -- it matters when there are libraries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			int location = listBox1.SelectedIndex;
			if (location >= 0 && location < listBox1.Items.Count-1)
			{
			    object rememberMe = listBox1.SelectedItem;
			    listBox1.Items.RemoveAt(location);
			    listBox1.Items.Insert(location + 1, rememberMe);
			    listBox1.SelectedIndex = location + 1;
			}
            updateRemoveButton();
        }

        /// <summary>
        /// Remove the selected file from the files list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index >= 0)
            {
                listBox1.Items.RemoveAt(index);
            }
            updateRemoveButton();
        }//btnRemove_Click

        /// <summary>
        /// Clear files from the open files list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            updateRemoveButton();
        }

    }//class MultipleFileOpen
}
