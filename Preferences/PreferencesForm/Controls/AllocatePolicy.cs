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
    public partial class AllocatePolicy : UserControl
    {
        public AllocatePolicy()
        {
            InitializeComponent();
        }

        public AllocatePolicyEnum AllocatePolicyType
        {
            get
            {
                if (rbReadAllocate.Checked)
                {
                    return AllocatePolicyEnum.Read;
                }
                else if (rbWriteAllocate.Checked)
                {
                    return AllocatePolicyEnum.Write;
                }
                else
                {
                    return AllocatePolicyEnum.Both;
                }//else
            }//get
            set
            {
                if (value == AllocatePolicyEnum.Read)
                {
                    rbReadAllocate.Checked = true;
                }
                else if (value == AllocatePolicyEnum.Write)
                {
                    rbWriteAllocate.Checked = true;
                }
                else
                {
                    rbBothAllocate.Checked = true;
                }
            }//set
        }//ReplaceStrategyType

    }
}
