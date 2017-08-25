using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ARMSim.Simulator;

namespace ARMSim.GUI.Views
{
    public partial class UnifiedCacheView : ARMSim.GUI.Views.DataCacheView
    {
        public UnifiedCacheView(ApplicationJimulator jm)
            : base(jm)
        {
            this.Text = UnifiedCacheView.ViewName;
            InitializeComponent();
        }
        public new static string ViewName { get { return "UnifiedCacheView"; } }

    }
}
