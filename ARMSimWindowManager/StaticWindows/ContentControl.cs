using System;
using System.Collections.Generic;
using System.Text;

using ARMSimWindowManager;
using System.Windows.Forms;
using System.Drawing;

namespace StaticWindows
{
    public class ContentControl : IContent
    {
        //private bool mVisible;

        public ComputeWidthBasedOnFontEventHandler ComputeWidthBasedOnFont { get; set; }

        public object Tag { get; set; }

        private Control mControl;
        private string mTitle;
        //private StaticWindowsControl mSwc;
        public ContentControl(Control control, string title)
        {
            mControl = control;
            mTitle = title;
            //mSwc = swc;
            this.IsShowing = true;
            //this.Enabled = true;
        }

        public string Title { get { return mTitle; } }
        public bool IsShowing { get; set; }
        //public bool Enabled { get; set; }
        //public bool Visible { get { return mVisible; } }

        public Control Control { get { return mControl; } }

        //public void Show()
        //{
        //    if (mVisible)
        //        return;
        //    mVisible = true;
        //    mControl.Parent.Parent.PerformLayout(mControl, string.Empty);
        //    //mSwc.RecalcLayout(this.Control);
        //}
        //public void Hide()
        //{
        //    if (!mVisible)
        //        return;
        //    mVisible = false;
        //    mControl.Parent.Parent.PerformLayout(mControl, string.Empty);
        //    //mSwc.RecalcLayout(this.Control);
        //}

    }//class ContentControl
}
