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
    public partial class About : Form
    {
        /// <summary>
        /// This class implements the About box dialog.
        /// Displays among other things the credits.
        /// </summary>
        public About()
        {
            InitializeComponent();


            Assembly ThisAssembly = Assembly.GetExecutingAssembly();
            AssemblyName ThisAssemblyName = ThisAssembly.GetName();

            string FriendlyVersion = string.Format("{0}.{1}.{2}", ThisAssemblyName.Version.Major, ThisAssemblyName.Version.Minor, ThisAssemblyName.Version.Build);

            Array Attributes = ThisAssembly.GetCustomAttributes(false);

            string Title = "Unknown Application";
            string Copyright = "Unknown Copyright";

            foreach (object o in Attributes)
            {
                AssemblyTitleAttribute o1 = o as AssemblyTitleAttribute;
                if (o1 != null) Title = o1.Title;
                AssemblyCopyrightAttribute o2 = o as AssemblyCopyrightAttribute;
                if (o2 != null) Copyright = o2.Copyright;
            }

            label4.Text = ARMSim.GUI.SplashScreen.Credits;

            label5.Text = "Simulating ARMv5 instruction architecture with Vector Floating Point support and a Data/Instruction Cache simulation.";

            this.Text = "About " + Title;
            StringBuilder sb = new StringBuilder("");
            sb.Append(Title);
            sb.Append(" Version ");
            sb.Append(FriendlyVersion);
            sb.Append(" (");
            sb.Append(ThisAssemblyName.Version.Revision.ToString());
            sb.Append(")\n\n");
            sb.Append(Copyright);
            label3.Text = sb.ToString();

        }
		/*
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.Links[0].Visited = true;
            this.linkLabel1.LinkVisited = true;
            string target = e.Link.LinkData as string;
            System.Diagnostics.Process.Start(target);
        }
		*/

    }//class About
}
