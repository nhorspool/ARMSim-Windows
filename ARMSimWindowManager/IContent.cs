using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

namespace ARMSimWindowManager
{
    public delegate int ComputeWidthBasedOnFontEventHandler();

    public interface IContent
    {
		//The title of the content item (shown in places like the docking window title)
        string Title { get; }
		//Whether or not the content item is displayed somewhere (i.e. is currently visible or
		//underneath another window)
        bool IsShowing { get; set; }
        //bool Enabled { get; set; }

        object Tag { get; set; }

        //bool CloseOnHide { get; set; }
        //void BringToFront();
        Control Control { get; }

        ComputeWidthBasedOnFontEventHandler ComputeWidthBasedOnFont { get; set; }

        //void Show();
        //void Hide();

    }
}
