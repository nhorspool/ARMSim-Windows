using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ARMSim.Simulator;

namespace ARMSim.GUI.Views
{
    public partial class NText : TextBox
    {
        private ResolveSymbolDelegate mResolveSymbolHandler;

        public NText()
        {
            InitializeComponent();

            this.Text = "0";
        }//NText ctor

        public ResolveSymbolDelegate ResolveSymbolHandler
        {
            get { return mResolveSymbolHandler; }
            set { mResolveSymbolHandler += value; }
        }

        public override string Text
        {
            get { return base.Text; }
            //set { }
        }

        public uint Value
        {
            get
            {
                try
                {
                    string str = base.Text.Trim();
                    if (str.Length == 0) return 0;
                    return Convert.ToUInt32(str, 16);
                }
                catch (Exception)
                {
                    base.Text = "";
                    return 0;
                }
            }

            set { base.Text = value.ToString("X8"); }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (mResolveSymbolHandler != null)
                {
                    uint address = 0;
                    if (mResolveSymbolHandler(this.Text, ref address))
                    {
                        base.Text = address.ToString("X8");
                    }
                }
            }
        }//OnKeyDown

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
            }
        }//OnKeyPress


    }//class NText
}
