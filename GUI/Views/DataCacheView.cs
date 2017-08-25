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
    public partial class DataCacheView : ARMSim.GUI.Views.CacheView
    {
        public DataCacheView(){}
        public DataCacheView(ApplicationJimulator jm)
            : base(jm)
        {
            this.Text = DataCacheView.ViewName;
            InitializeComponent();
        }

        public static string ViewName { get { return "DataCacheView"; } }

        public override ARMSim.Simulator.Cache.L1Cache CacheMemory
        {
            get { return _JM.DataCacheMemory; }
        }

    }
}
