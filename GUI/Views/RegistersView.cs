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
    public partial class RegistersView : UserControl, IView, IViewXMLSettings
    {
        private enum RegisterDisplayBase
        {
            Hexadecimal,
            Unsigned,
            Signed
        };
        private enum FloatingPointType
        {
            Single,
            Double
        };

        private bool[] _gpRegistersChanged;
        private bool[] _fpRegistersChanged;
        private bool[] _cpsrChanged;
        private bool[] _fpcpsrChanged;

        private GraphicElements _graphicElements;
        private Color mHighlightColor;
        private ApplicationJimulator mJM;
        //private SolidBrush _textBrush = new SolidBrush(Color.Black);
        //private SolidBrush _highlightBrush = new SolidBrush(Color.Red);

        private const string _displayBaseElement = "CurrentDisplayBase";
        private const string _displayRegisterTab = "CurrentRegisterTab";
        private const string _displayFPBaseElement = "CurrentFPDisplayBase";

        //public event ARMSimWindowManager.OnRecalLayout OnRecalLayout;

        /// <summary>
        /// Registers view ctor.
        /// </summary>
        /// <param name="jm"></param>
        public RegistersView(ApplicationJimulator jm)
        {
            mJM = jm;

            _cpsrChanged = new bool[7];
            _gpRegistersChanged = new bool[16];
            _fpRegistersChanged = new bool[32];
            _fpcpsrChanged = new bool[4];
            clearRegistersChanged();

            mJM.GPR.RegisterChangedHandler = delegate(uint reg) { _gpRegistersChanged[reg] = true; };
            mJM.FPP.FPR.RegisterChangedHandler = delegate(uint reg) { _fpRegistersChanged[reg] = true; };
            mJM.CPSR.CPSRChangedHandler = delegate(CPSR.CPUFlag flag) { _cpsrChanged[flag.Index] = true; };
            mJM.FPP.FPSCR.FPSRChangedHandler = delegate(Simulator.VFP.FPSCR.CPUFlag flag) { _fpcpsrChanged[flag.Index] = true; };

            this.Text = RegistersView.ViewName;

            InitializeComponent();

            this.defaultSettings();
            _graphicElements = new GraphicElements(this);

        }

        private void clearRegistersChanged()
        {
            Array.Clear(_gpRegistersChanged, 0, _gpRegistersChanged.Length);
            Array.Clear(_fpRegistersChanged, 0, _fpRegistersChanged.Length);
            Array.Clear(_cpsrChanged, 0, _cpsrChanged.Length);
            Array.Clear(_fpcpsrChanged, 0, _fpcpsrChanged.Length);
        }//clearRegistersChanged

        public void stepStart()
        {
            clearRegistersChanged();
        }

        public void stepEnd() { }
        public void resetView()
        {
            clearRegistersChanged();
        }

        private FloatingPointType CurrentFloatingPointType
        {
            get
            {
                if (btnSingle.Checked)
                    return FloatingPointType.Single;
                else
                    return FloatingPointType.Double;
            }
        }//CurrentFloatingPointType

        private RegisterDisplayBase CurrentDisplayBase
        {
            get
            {
                if (btnHex.Checked)
                    return RegisterDisplayBase.Hexadecimal;
                else if (btnUsigned.Checked)
                    return RegisterDisplayBase.Unsigned;
                else
                    return RegisterDisplayBase.Signed;
            }
        }

        public void updateView()
        {
            int requiredHeight = (panel1.Font.Height * 28) + 1;
            //this.vScrollBar1.Visible = panel1.Height < requiredHeight;
            panel1.AutoScrollMinSize = new Size(panel1.Width / 2, requiredHeight);

            int numRegisters = this.CurrentFloatingPointType == FloatingPointType.Single ? 32 : 16;
            requiredHeight = (panel2.Font.Height * (numRegisters + 10)) + 1;
            panel2.AutoScrollMinSize = new Size(panel2.Width / 2, requiredHeight);

            panel1.Invalidate();
            panel2.Invalidate();
        }

        public Font CurrentFont
        {
            get { return panel1.Font; }
            set
            {
                panel1.Font = value;
                panel2.Font = value;
                this.Parent.Parent.PerformLayout();
            }
        }

        public Color CurrentTextColour
        {
            get { return panel1.ForeColor; }
            set
            {
                panel1.ForeColor = value;
                panel2.ForeColor = value;
                panel1.Invalidate();
                panel2.Invalidate();
            }//set
        }//CurrentTextColour

        public Color CurrentBackgroundColour
        {
            get { return tabControl1.BackColor; }
            set
            {
                panel1.BackColor = value;
                panel2.BackColor = value;
            }
        }

        public Color CurrentHighlightColour
        {
            get { return this.mHighlightColor; }
            set
            {
                this.mHighlightColor = value;
                panel1.Invalidate();
                panel2.Invalidate();
            }
        }//CurrentHighlightColour

        public static string ViewName { get { return "RegistersView"; } }

        public Control ViewControl { get { return this; } }

        public void defaultSettings()
        {
            mHighlightColor = Color.Red;
        }

        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(RegistersView.ViewName);
            xmlOut.WriteAttributeString(_displayBaseElement, this.CurrentDisplayBase.ToString());
            xmlOut.WriteAttributeString(_displayRegisterTab, tabControl1.SelectedIndex.ToString());
            xmlOut.WriteAttributeString(_displayFPBaseElement, this.CurrentFloatingPointType.ToString());
            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();
        }//saveState

        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                _graphicElements.loadFromXML(xmlIn);
                string displayBase = xmlIn.GetAttribute(_displayBaseElement);
                if (displayBase != null)
                {
                    if (displayBase == RegisterDisplayBase.Hexadecimal.ToString())
                    {
                        btnHex.Checked = true;
                    }//if
                    else if (displayBase == RegisterDisplayBase.Unsigned.ToString())
                    {
                        btnUsigned.Checked = true;
                    }//else
                    else
                    {
                        btnSigned.Checked = true;
                    }//else
                }//if

                string displayFPBase = xmlIn.GetAttribute(_displayFPBaseElement);
                if (displayFPBase != null)
                {
                    if (displayFPBase == FloatingPointType.Single.ToString())
                    {
                        this.btnSingle.Checked = true;
                    }//if
                    else
                    {
                        this.btnDouble.Checked = true;
                    }//else
                }//if

                string currentTab = xmlIn.GetAttribute(_displayRegisterTab);
                if (currentTab != null)
                {
                    tabControl1.SelectedIndex = int.Parse(currentTab);
                }
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.defaultSettings();
            }
        }//LoadFromXML

        private void btn_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (!rb.Checked)
                return;

            this.updateView();
        }

        private void tabPage2_Resize(object sender, EventArgs e)
        {
            int formHeight = tabPage2.Height;
            int buttonHeight = btnSingle.Height + btnDouble.Height;
            panel2.Height = formHeight - buttonHeight - 5;
            //fpRegisterLB.ItemHeight = fpRegisterLB.Font.Height + 1;

            //int numRegisters = this.CurrentFloatingPointType == FloatingPointType.Single ? 32 : 16;
            //int requiredHeight = (panel2.Font.Height * (numRegisters + 10)) + 1;
            //panel2.AutoScrollMinSize = new Size(panel2.Width / 2, requiredHeight);

        }//tabPage2_Resize

        private void tabPage1_Resize(object sender, EventArgs e)
        {
            int formHeight = tabPage1.Height;
            int buttonHeight = btnHex.Height + btnUsigned.Height + btnSigned.Height;
            panel1.Height = formHeight - buttonHeight - 5;
            //int requiredHeight = (panel1.Font.Height * 28) + 1;
            ////this.vScrollBar1.Visible = panel1.Height < requiredHeight;
            //panel1.AutoScrollMinSize = new Size(panel1.Width/2, requiredHeight);

        }//tabPage1_Resize

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();
            _graphicElements.Popup(cms, false);
        }

        public int ComputeWidthBasedOnFont()
        {
            Graphics g = panel1.CreateGraphics();
            SizeF size = g.MeasureString("CPU Mode   :SystemF", panel1.Font);

            //SizeF size = g.MeasureString("FFFFFFFFF:FFFFFFFFF", panel1.Font);
            int extra = this.tabControl1.Width - panel1.ClientRectangle.Width;

            return (int)((size.Width) + 1 + extra);
        }

        private void RegistersView_Load(object sender, EventArgs e)
        {
            //this.Width = ComputeWidthBasedOnFont();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (mJM == null || !mJM.ValidLoadedProgram)
                return;

            Graphics g = e.Graphics;

            System.Drawing.Drawing2D.Matrix mx = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1,
                panel1.AutoScrollPosition.X, panel1.AutoScrollPosition.Y);
            e.Graphics.Transform = mx;

            using(SolidBrush backBrush = new SolidBrush(panel1.BackColor))
            using(SolidBrush textBrush = new SolidBrush(panel1.ForeColor))
            using(SolidBrush highlightBrush = new SolidBrush(mHighlightColor))
            {

                for (uint ii = 0; ii < 16; ii++)
                {
                    int ypos = (int)(ii * panel1.Font.Height);
                    Rectangle bounds = new Rectangle(0, ypos, panel1.ClientRectangle.Width, panel1.Font.Height);

                    string str = GeneralPurposeRegisters.registerToString(ii);

                    uint regValue = mJM.GPR[ii];
                    switch (this.CurrentDisplayBase)
                    {
                        case RegisterDisplayBase.Hexadecimal:
                            str += regValue.ToString("x8"); break;
                        case RegisterDisplayBase.Signed:
                            str += ((int)regValue).ToString(); break;
                        case RegisterDisplayBase.Unsigned:
                            str += regValue.ToString(); break;
                    }
                    g.FillRectangle(backBrush, bounds);
                    Brush brush = _gpRegistersChanged[ii] ? highlightBrush : textBrush;

                    // Draw the current item text based on the current Font and the custom brush settings.
                    e.Graphics.DrawString(str, panel1.Font, brush, bounds, StringFormat.GenericDefault);

                }//for ii

                RegistersView.DrawLine(g, panel1, 16, textBrush, "------------------");
                RegistersView.DrawLine(g, panel1, 17, textBrush, "CPSR Register");
                RegistersView.DrawLine(g, panel1, 18, _cpsrChanged[0] ? highlightBrush : textBrush, "Negative(N):" + (mJM.CPSR.nf ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 19, _cpsrChanged[1] ? highlightBrush : textBrush, "Zero(Z)    :" + (mJM.CPSR.zf ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 20, _cpsrChanged[2] ? highlightBrush : textBrush, "Carry(C)   :" + (mJM.CPSR.cf ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 21, _cpsrChanged[3] ? highlightBrush : textBrush, "Overflow(V):" + (mJM.CPSR.vf ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 22, textBrush, "IRQ Disable:" + (mJM.CPSR.IRQDisable ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 23, textBrush, "FIQ Disable:" + (mJM.CPSR.FIQDisable ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 24, textBrush, "Thumb(T)   :" + (mJM.CPSR.tf ? "1" : "0"));
                RegistersView.DrawLine(g, panel1, 25, textBrush, "CPU Mode   :" + System.Enum.GetName(typeof(CPSR.CPUModeEnum), mJM.CPSR.Mode));
                RegistersView.DrawLine(g, panel1, 26, textBrush, "------------------");
                RegistersView.DrawLine(g, panel1, 27, textBrush, "0x" + mJM.CPSR.Flags.ToString("x8"));
            }
        }

        public static void DrawLine(Graphics g, Control panel, int line, Brush textBrush, string str)
        {
            int ypos = (int)(line * panel.Font.Height);
            Rectangle bounds = new Rectangle(0, ypos, panel.ClientRectangle.Width, panel.Font.Height);
            using (SolidBrush backBrush = new SolidBrush(panel.BackColor))
            {
                g.FillRectangle(backBrush, bounds);
                g.DrawString(str, panel.Font, textBrush, bounds, StringFormat.GenericDefault);
            }
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (mJM == null || !mJM.ValidLoadedProgram)
                return;

            Graphics g = e.Graphics;

            System.Drawing.Drawing2D.Matrix mx = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, panel2.AutoScrollPosition.X, panel2.AutoScrollPosition.Y);
            e.Graphics.Transform = mx;

            using(SolidBrush backBrush = new SolidBrush(panel2.BackColor))
            using(SolidBrush textBrush = new SolidBrush(panel2.ForeColor))
            using (SolidBrush highlightBrush = new SolidBrush(mHighlightColor))
            {

                int line = 0;
                switch (this.CurrentFloatingPointType)
                {
                    case FloatingPointType.Single:
                        for (uint ii = 0; ii < 32; ii++, line++)
                        {
                            int ypos = (int)(ii * panel2.Font.Height);
                            Rectangle bounds = new Rectangle(0, ypos, panel2.ClientRectangle.Width, panel2.Font.Height);

                            string str = "s" + ii.ToString() + ((ii < 10) ? " " : "") + ":";
                            float regValue = mJM.FPP.FPR.ReadS(ii);
                            str += ARMSim.Simulator.VFP.FloatingPointProcessor.FloatToString(regValue);
                            //str += float.IsNaN(regValue) ? "NaN" : regValue.ToString("0.###E+0");

                            g.FillRectangle(backBrush, bounds);
                            Brush brush = _fpRegistersChanged[ii] ? highlightBrush : textBrush;
                            e.Graphics.DrawString(str, panel2.Font, brush, bounds, StringFormat.GenericDefault);

                        }
                        break;
                    case FloatingPointType.Double:
                        for (uint ii = 0; ii < 16; ii++, line++)
                        {
                            int ypos = (int)(ii * panel2.Font.Height);
                            Rectangle bounds = new Rectangle(0, ypos, panel2.ClientRectangle.Width, panel1.Font.Height);

                            string str = "d" + ii.ToString() + ((ii < 10) ? " " : "") + ":";
                            double regValue = mJM.FPP.FPR.ReadD(ii);
                            str += ARMSim.Simulator.VFP.FloatingPointProcessor.DoubleToString(regValue);
                            //str += double.IsNaN(regValue) ? "NaN" : regValue.ToString("0.###E+0");

                            g.FillRectangle(backBrush, bounds);
                            Brush brush = _fpRegistersChanged[ii] ? highlightBrush : textBrush;
                            e.Graphics.DrawString(str, panel1.Font, brush, bounds, StringFormat.GenericDefault);
                        }
                        break;
                }//switch
                RegistersView.DrawLine(g, panel2, line++, textBrush, "------------------");
                RegistersView.DrawLine(g, panel2, line++, textBrush, "FCPSR Register");
                RegistersView.DrawLine(g, panel2, line++, textBrush, "Negative(N):" + (mJM.FPP.FPSCR.nf ? "1" : "0"));
                RegistersView.DrawLine(g, panel2, line++, textBrush, "Zero(Z)    :" + (mJM.FPP.FPSCR.zf ? "1" : "0"));
                RegistersView.DrawLine(g, panel2, line++, textBrush, "Carry(C)   :" + (mJM.FPP.FPSCR.cf ? "1" : "0"));
                RegistersView.DrawLine(g, panel2, line++, textBrush, "Overflow(V):" + (mJM.FPP.FPSCR.vf ? "1" : "0"));
                RegistersView.DrawLine(g, panel2, line++, textBrush, "Stride     :" + (mJM.FPP.FPSCR.Stride.ToString()));
                RegistersView.DrawLine(g, panel2, line++, textBrush, "Length     :" + (mJM.FPP.FPSCR.Length.ToString()));
                RegistersView.DrawLine(g, panel2, line++, textBrush, "-------------");
                RegistersView.DrawLine(g, panel2, line++, textBrush, "0x" + mJM.FPP.FPSCR.Flags.ToString("x8"));
            }
        }

        public void TerminateInput() { }

    }//class RegistersView
}
