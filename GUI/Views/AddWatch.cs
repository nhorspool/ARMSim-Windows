using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ARMSim.Simulator;
using System.IO;

namespace ARMSim.GUI.Views
{
    /// <summary>
    /// This class implements the Add Watch entry for the WatchView. It displays a dialog
    /// to the user who fills out the information of the entry to add to the watch view.
    /// </summary>
    public partial class AddWatch : Form
    {
        private class watchEntry
        {
            private readonly string _longFilename;
            private readonly string _shortFilename;
            public watchEntry(string longFilename, string shortFilename)
            {
                _longFilename = longFilename;
                _shortFilename = shortFilename;
            }
            public string LongFilename { get { return _longFilename; } }
            //public string ShortFilename { get { return _shortFilename; } }

            public override string ToString() { return _shortFilename; }
        }

        private CodeLabels _codeLabels;

        /// <summary>
        /// AddWatch ctor
        /// </summary>
        public AddWatch()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Access to the CodeLabels of the loaded program
        /// </summary>
        public CodeLabels CodeLabels { get { return _codeLabels; } set { _codeLabels = value; } }

        /// <summary>
        /// Type of watch entry
        /// </summary>
        public WatchView.WatchType WatchType
        {
            get
            {
                if (rbByte.Checked)
                    return (WatchView.WatchType.Byte);
                else if (rbHalfword.Checked)
                    return (WatchView.WatchType.HalfWord);
                else if (rbWord.Checked)
                    return (WatchView.WatchType.Word);
                else if (rbCharacter.Checked)
                    return (WatchView.WatchType.Character);
                else
                    return (WatchView.WatchType.String);
            }
            set
            {
                switch (value)
                {
                    case WatchView.WatchType.Byte:
                        rbByte.Checked = true; break;
                    case WatchView.WatchType.HalfWord:
                        rbHalfword.Checked = true; break;
                    case WatchView.WatchType.Word:
                        rbWord.Checked = true; break;
                    case WatchView.WatchType.Character:
                        rbCharacter.Checked = true; break;
                    default:
                        rbString.Checked = true; break;
                }
            }//set
        }//WatchType

        ///<summary>Display the watch entry in hexadecimal</summary>
        public bool DisplayHex { get { return rbHexDecimal.Checked; } set { rbHexDecimal.Checked = value; } }
        ///<summary>Display the watch entry in signed decimal</summary>
        public bool Signed { get { return rbSigned.Checked; } set { rbSigned.Checked = value; } }

        ///<summary>The label address of the watch entry</summary>
        public string Label
        {
            get
            {
                if (lbLabels.SelectedIndex < 0)
                {
                    string hexnum = this.HexAddress.Text;
                    uint addr = 0;
                    bool hexnumGiven = UInt32.TryParse(hexnum, System.Globalization.NumberStyles.HexNumber, null, out addr);
                    this.HexAddress.Text = "";
                    if (hexnumGiven)
                        return String.Format("0x{0:X}", addr);
                    return string.Empty;
                }
                return lbLabels.Items[lbLabels.SelectedIndex] as string;
            }
        }

        /// <summary>
        /// Add a watch entry to the watch view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWatch_Load(object sender, EventArgs e)
        {
            foreach (string fileName in _codeLabels.FileSections.Keys)
            {
                lbFiles.Items.Add(new watchEntry(fileName, Path.GetFileName(fileName)));
            }//foreach
            if (lbFiles.Items.Count > 0)
            {
                lbFiles.SelectedIndex = 0;
            }

        }//AddWatch_Load

        private void rbDisplayAs_CheckedChanged(object sender, EventArgs e)
        {
            bool signedEnabled = (!rbString.Checked && !rbCharacter.Checked);
            groupBox2.Enabled = signedEnabled;
            rbSigned.Enabled = signedEnabled;
            rbUnsigned.Enabled = signedEnabled;
            rbHexDecimal.Enabled = signedEnabled;
            rbDecimal.Enabled = signedEnabled;
            groupBox3.Enabled = signedEnabled;
        }

        private void rbIntegerFormat_CheckedChanged(object sender, EventArgs e)
        {
            bool decimalDisplay = rbDecimal.Checked;
            rbSigned.Enabled = decimalDisplay;
            rbUnsigned.Enabled = decimalDisplay;
            groupBox2.Enabled = decimalDisplay;
        }

        private void lbLabels_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbFiles.SelectedIndex;
            if (index >= 0)
            {
                lbLabels.Items.Clear();
                string fileName = (lbFiles.SelectedItem as watchEntry).LongFilename;
                foreach (string label in _codeLabels.DataSectionLabels(fileName).Keys)
                {
                    lbLabels.Items.Add(label);
                }//foreach
            }//if
        }


    }//class AddWatch
}
