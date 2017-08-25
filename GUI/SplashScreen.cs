using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

namespace ARMSim.GUI
{
    /// <summary>
    /// Summary description for SplashScreen.
    /// This screen is displayed at application startup, but only in a release build.
    /// </summary>
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// flag indicating splash screen is done.
        /// </summary>
        public bool done;

        /// <summary>
        /// SplashScreen ctor
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();
            this.label2.Text = Credits;

            Assembly ThisAssembly = Assembly.GetExecutingAssembly();
            AssemblyName ThisAssemblyName = ThisAssembly.GetName();
            string FriendlyVersion = string.Format("Version:{0}.{1}.{2}", ThisAssemblyName.Version.Major, ThisAssemblyName.Version.Minor, ThisAssemblyName.Version.Build);

            this.lblVersion.Text = FriendlyVersion;


        }//SplashScreen ctor

        /// <summary>
        /// Creates a string with the ARM sim credits.
        /// </summary>
        public static string Credits
        {
            get
            {
                StringBuilder str = new StringBuilder();
                str.Append("University of Victoria\n");
                str.Append("Produced by:\nDr. Nigel Horspool\nDale Lyons\nDr. Micaela Serra\nBill Bird\n");
                str.Append("Department of Computer Science.\n\n");
                str.Append("Copyright 2006--2017 University of Victoria.\nAll rights reserved.");
                //str.Append("For use by CSC230 students and staff only");
                return str.ToString();
            }//get
        }//Credits

        private void timer1_Tick(object sender, EventArgs e)
        {
            done = true;
            this.Close();
        }

    }//class SplashScreen
}
