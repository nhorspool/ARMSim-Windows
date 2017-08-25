using System;
using System.Collections.Generic;
using System.Text;

using ARMSimWindowManager;
using System.Windows.Forms;
using System.Drawing;

using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;

namespace DockingWindows
{
    public class ContentControl : IContent
    {
        public bool Enabled { get; set; }


        public bool IsShowing
        {
            get
            {
                if (mTag == null)
                    return false;

                if (!(mTag is Content))
                    return false;

                Content c = mTag as Content;

                return c.Visible;
            }
            set
            {
                if (mTag == null)
                    return;

                if (!(mTag is Content))
                    return;

                Content c = mTag as Content;
                if(value)
                    mDockingManager.ShowContent(c);
                else
                    mDockingManager.HideContent(c);

            }
        }



        public ComputeWidthBasedOnFontEventHandler ComputeWidthBasedOnFont { get; set; }


        //public event GetWidth GetWidth;

        private object mTag;
        public object Tag
        {
            get
            {
                return mTag;
            }
            set
            {
                mTag = value;
            }
        }

        private Control mControl;
        private string mTitle;
        //private Content mContent;
        private DockingManager mDockingManager;
        public ContentControl(Control control, string title, DockingManager dockingManager)
        {
            mControl = control;
            mTitle = title;
            mDockingManager = dockingManager;
            this.IsShowing = true;
        }

        public string Title { get { return mTitle; } }

        //public bool Visible
        //{
        //    get { return mDockingManager.Contents[mTitle].Visible; }
        //    set {  }
        //}
        //public bool CloseOnHide { get; set; }

        //public void BringToFront() { }

        public Control Control { get { return mControl; } }

        //public void Show()
        //{
        //    if (this.Visible)
        //        return;

        //    Content content = mDockingManager.Contents[mTitle];
        //    mDockingManager.ShowContent(content);
        //    //this.Visible = true;

        //}

        //public void Hide()
        //{
        //    if (!this.Visible)
        //        return;

        //    Content content = mDockingManager.Contents[mTitle];
        //    mDockingManager.HideContent(content);
        //    //this.Visible = false;

        //}

    }
}
