using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

namespace ARMSim.Plugins.UIControls
{
    public partial class WalkSignal : UserControl
    {
        public enum XWalkStates
        {
            DONTWALK = 0,
            WALK = 1,
            HURRY = 2
        }

        private Bitmap mWalkImage;
        private Bitmap mDontWalkImage;
        private Bitmap mHurryImage;
        private XWalkStates mState;

        public WalkSignal()
        {
            InitializeComponent();

            // Get the assembly that contains the bitmap resource
            Assembly assembly = Assembly.GetAssembly(this.GetType());

            mDontWalkImage = (Bitmap.FromStream(assembly.GetManifestResourceStream("ARMSim.Plugins.UIControls.Resources.BIGdontwalk.gif")) as Bitmap);
            mWalkImage = (Bitmap.FromStream(assembly.GetManifestResourceStream("ARMSim.Plugins.UIControls.Resources.BIGwalk.gif")) as Bitmap);
            mHurryImage = (Bitmap.FromStream(assembly.GetManifestResourceStream("ARMSim.Plugins.UIControls.Resources.Hurry.bmp")) as Bitmap);

            this.pictureBox1.BackgroundImage = mDontWalkImage;

        }

        public XWalkStates State
        {
            get { return mState; }
            set
            {
                mState = value;
                switch (mState)
                {
                    case XWalkStates.DONTWALK:
                        this.pictureBox1.BackgroundImage = mDontWalkImage;
                        break;
                    case XWalkStates.WALK:
                        this.pictureBox1.BackgroundImage = mWalkImage;
                        break;
                    case XWalkStates.HURRY:
                        this.pictureBox1.BackgroundImage = mHurryImage;
                        break;
                    default: break;
                }
            }
        }

    }
}
