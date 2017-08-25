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
    public partial class ReplacementStrategy : UserControl
    {
        public ReplacementStrategy()
        {
            InitializeComponent();
        }

        public ReplaceStrategiesEnum ReplaceStrategyType
        {
            get
            {
                return rbRandom.Checked ? ReplaceStrategiesEnum.Random : ReplaceStrategiesEnum.RoundRobin;
            }//get
            set
            {
                if (value == ReplaceStrategiesEnum.Random)
                {
                    rbRandom.Checked = true;
                }//if
                else
                {
                    rbRoundRobin.Checked = true;
                }//else
            }//set
        }//ReplaceStrategyType

    }
}
