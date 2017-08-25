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
    public partial class InstructionCacheView : ARMSim.GUI.Views.CacheView
    {
        public InstructionCacheView(ApplicationJimulator jm)
            : base(jm)
        {
            this.Text = InstructionCacheView.ViewName;
            InitializeComponent();
        }

        public static string ViewName { get { return "InstructionCacheView"; } }

        public override ARMSim.Simulator.Cache.L1Cache CacheMemory
        {
            get { return _JM.InstructionCacheMemory; }
        }

    }
}
