using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using ARMSim.Preferences;
using ARMSim.Simulator;

namespace ARMSim.GUI.Views
{
    public partial class StackView : UserControl, IView, IViewXMLSettings
    {
        private uint mStackPointer;
        private uint mLowAddress;
        //private uint mHighAddress;

        private uint[] mStackWords;

        //public event ARMSimWindowManager.OnRecalLayout OnRecalLayout;

        private GraphicElements _graphicElements;
        private Color _highlightColor;

        private ApplicationJimulator mJM;

        /// <summary>
        /// StackView ctor
        /// </summary>
        public StackView(ApplicationJimulator jm)
        {
            mJM = jm;
            this.Text = StackView.ViewName;
            InitializeComponent();

            this.defaultSettings();
            _graphicElements = new GraphicElements(this);
        }

        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                _graphicElements.loadFromXML(xmlIn);
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.defaultSettings();
            }
        }//LoadFromXML

        public void resetView() { }

        public void updateView()
        {
            this.CalculateLayout();
            panel1.Invalidate();
        }

        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(StackView.ViewName);
            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();
        }//saveState

        public static string ViewName { get { return "StackView"; } }

        public Font CurrentFont
        {
            get { return panel1.Font; }
            set
            {
                panel1.Font = value;
                this.CalculateLayout();
                this.Parent.Parent.PerformLayout();
            }
        }

        public Color CurrentTextColour
        {
            get { return panel1.ForeColor; }
            set { panel1.ForeColor = value; }
        }

        public Color CurrentBackgroundColour
        {
            get { return panel1.BackColor; }
            set { panel1.BackColor = value; }
        }
        public Control ViewControl { get { return this; } }

        public Color CurrentHighlightColour
        {
            get { return this._highlightColor; }
            set { this._highlightColor = value; panel1.Invalidate(); }
        }

        public void stepStart() { }
        public void stepEnd() { }

        public void defaultSettings()
        {
            _highlightColor = Color.LightBlue;
        }

        private void CalculateLayout()
        {
            if (mJM == null || !mJM.ValidLoadedProgram)
                return;

            uint numRows = (uint)((panel1.ClientRectangle.Height + 1) / panel1.Font.Height);
            mStackWords = new uint[numRows];

            uint halfHeight = numRows >> 1;
            mStackPointer = mJM.GPR.SP >> 2;

            mLowAddress = mStackPointer - halfHeight;
            //mHighAddress = mStackPointer + halfHeight;
            for (uint ii = 0; ii < mStackWords.Length; ii++)
            {
                //want to fetch memory and bypass the cache logic
                uint address = (mLowAddress + ii) << 2;
                if(mJM.MainMemory.InRange(address, ARMPluginInterfaces.MemorySize.Word))
                {
                    mStackWords[ii] = mJM.MainMemory.GetMemory(address, ARMPluginInterfaces.MemorySize.Word);
                }
            }//for ii
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (mJM == null || !mJM.ValidLoadedProgram || mStackWords == null)
                return;

            Graphics g = e.Graphics;
            using (Brush myBrush = new SolidBrush(panel1.ForeColor))
            {

                for (int ii = 0; ii < mStackWords.Length; ii++)
                {
                    //uint word = mStackWords[ii];
                    int ypos = ii * panel1.Font.Height;
                    Rectangle bounds = new Rectangle(0, ypos, panel1.ClientRectangle.Width, panel1.Font.Height);
                    if ((mLowAddress + ii) == mStackPointer)
                    {
                        using (SolidBrush b = new SolidBrush(_highlightColor))
                            g.FillRectangle(b, bounds);
                    }
                    else
                    {
                        using(SolidBrush b = new SolidBrush(panel1.BackColor))
                            g.FillRectangle(b, bounds);
                    }   

                    uint address = (uint)((mLowAddress + ii) << 2);
                    string myString = address.ToString("X8") + ":";
                    if (mJM.InRange(address, ARMPluginInterfaces.MemorySize.Word))
                    {
                        uint opcode = mJM.GetMemoryNoSideEffect(address, ARMPluginInterfaces.MemorySize.Word);
                        myString += opcode.ToString("X8");
                    }
                    else
                    {
                        myString += new string('?', 8);
                    }
                    g.DrawString(myString, panel1.Font, myBrush, bounds);

                }//for ii
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (mJM == null || !mJM.ValidLoadedProgram)
                return;

            this.CalculateLayout();
            panel1.Invalidate();
        }

        public int ComputeWidthBasedOnFont()
        {
            Graphics g = panel1.CreateGraphics();
            SizeF size = g.MeasureString("FFFFFFFFF:FFFFFFFF", panel1.Font);
            return (int)((size.Width) + 1);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();
            _graphicElements.Popup(cms, false);
        }

        public void TerminateInput() { }

    }
}
