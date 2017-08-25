/* DockContentWrapper.cs
 * 
 * A wrapper for the contents of docked windows (e.g. registers, code, etc.)
 * 
 * B. Bird - 08/09/2014
*/


using System;
using System.Collections.Generic;
using System.Text;

using ARMSimWindowManager;
using System.Windows.Forms;
using System.Drawing;


using WeifenLuo.WinFormsUI.Docking;

namespace DockingWindows2
{
    public class DockContentWrapper : DockContent,IContent
    {
		
     	public DockContentWrapper(Control contentControl, string title)
		{
			
			Title = title;
			IsShowing = false;
			this.Control = contentControl;
			

			this.SuspendLayout();



			this.ClientSize = contentControl.Size;

			this.Control.Dock = DockStyle.Fill;
			this.Controls.Add(this.Control);

			//this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

			this.TabText = this.Name = this.Text = Title;


			this.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);

			this.ResumeLayout(false);
			this.PerformLayout();

			this.Size = this.SizeFromClientSize(contentControl.Size);
			this.PersistString = this.Name;

			//When the close button of a docked or floating window is pressed, we do not want to kill the window,
			//since the main ARMSim window keeps a reference to it and expects it to respawn when IsShowing is set
			//to true. The HideOnClose property causes the close button to hide the window instead of killing it.
			//Note that this behavior does not extend to MemoryView windows (see MemoryViewWrapper below)
			this.HideOnClose = true;

		}
		public string PersistString { get; set; }
		protected override string GetPersistString()
		{
			return PersistString;
		}
		

        public ComputeWidthBasedOnFontEventHandler ComputeWidthBasedOnFont { get; set; }


		public virtual bool IsShowing
		{
			get { return !this.IsHidden; }
			set
			{
				if (value)
				{
					Show();
				}
				else
				{
					Hide();
				}
			}
		}
	
        public string Title { get; private set; }

        public Control Control { get; private set; }


    }

	public class MemoryViewWrapper : DockContentWrapper
	{
		public int Index { get; private set; }
		public MemoryViewWrapper(Control contentControl, string title, int index)
			: base(contentControl, title)
		{
			Index = index;

			//When the close button is pressed, the window should be killed.
			//The DockingWindows2Manager class adds a handler to the FormClosed event to catch this.
			this.HideOnClose = false;

		}
	}
}
