using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ARMPluginInterfaces.Preferences;

namespace ARMSim.Preferences.PreferencesForm.Controls
{
    public partial class WritePolicy : UserControl
    {
        public WritePolicy()
        {
            InitializeComponent();
        }

        public WritePolicyEnum WritePolicyType
        {
            get
            {
                return rbWriteThrough.Checked ? WritePolicyEnum.WriteThrough : WritePolicyEnum.WriteBack;
            }//get
            set
            {
                if (value == WritePolicyEnum.WriteThrough)
                {
                    rbWriteThrough.Checked = true;
                }//if
                else
                {
                    rbWriteBack.Checked = true;
                }//else
            }//set
        }//WritePolicyType

    }
}
