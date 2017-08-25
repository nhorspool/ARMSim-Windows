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
    public partial class MemoryView : UserControl, IView, IViewXMLSettings
    {
        private ApplicationJimulator _JM;
        private GraphicElements _graphicElements;
        private Color _highlightColor = Color.Red;
        private string _title;

        private bool[] mChangedMemory;
        private uint mChangedMemoryStart;

        private Size mCharSize;
        private int mNumCellsPerLine;
        private int mNumLines;
        private int mBytesPerCell;

        //private TabPage mTabPage;

        //public event ARMSimWindowManager.OnRecalLayout OnRecalLayout;

        ////todo - delete me
        //private NText tbAddress = new NText();
        /// <summary>
        /// MemoryView ctor
        /// </summary>
        public MemoryView(ApplicationJimulator jm, int viewIndex)
        {
            _JM = jm;

            //mChangedMemory;
            //mChangedMemoryStart = 0;
            this.Index = viewIndex;

            InitializeComponent();

            _graphicElements = new GraphicElements(this);

        }

        //public TabPage TabPage { set { mTabPage = value; } }

        public MemoryView(ApplicationJimulator jm)
            : this(jm, 0)
        {
        }

		private int _index;
		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_title = MemoryView.Prefix + value;
				_index = value;
			}
		}

        public ResolveSymbolDelegate ResolveSymbolHandler
        {
            //get { return tbAddress.ResolveSymbolHandler; }
            set { tbAddress.ResolveSymbolHandler += value; }
        }

        public string ViewName { get { return _title; } }

        public Font CurrentFont
        {
            get { return panel1.Font; }
            set
            {
                panel1.Font = value;
                panel1.Invalidate();
            }
        }

        public Color CurrentHighlightColour
        {
            get { return this._highlightColor; }
            set { this._highlightColor = value; this.Invalidate(); }
        }

        public Color CurrentTextColour
        {
            get { return panel1.ForeColor; }
            set { panel1.ForeColor = value; this.Invalidate(); }
        }

        public Color CurrentBackgroundColour
        {
            get { return panel1.BackColor; }
            set { panel1.BackColor = value; }
        }
        public Control ViewControl { get { return this; } }

        public uint NumberBytesWindow
        {
            get { return (uint)(mNumCellsPerLine * (mNumLines - 1) * mBytesPerCell); }
        }

        public void resetView()
        {
            mChangedMemory = null;
            panel1.Invalidate();
        }
        public void updateView()
        {
            panel1.Invalidate();
        }

        public ARMPluginInterfaces.MemorySize CheckedMemorySize
        {
            get
            {
                if (rb8Bit.Checked)
                    return ARMPluginInterfaces.MemorySize.Byte;
                else if (rb16Bit.Checked)
                    return ARMPluginInterfaces.MemorySize.HalfWord;
                else
                    return ARMPluginInterfaces.MemorySize.Word;
            }
            set
            {
                if (value == ARMPluginInterfaces.MemorySize.Byte)
                {
                    rb8Bit.Checked = true;
                }
                else if (value == ARMPluginInterfaces.MemorySize.HalfWord)
                {
                    rb16Bit.Checked = true;
                }
                else
                {
                    rb32Bit.Checked = true;
                }
            }//set
        }//CheckedMemorySize

        public string CheckedMemorySizeString
        {
            get
            {
                switch (this.CheckedMemorySize)
                {
                    case ARMPluginInterfaces.MemorySize.Byte: return "8Bit";
                    case ARMPluginInterfaces.MemorySize.HalfWord: return "16Bit";
                    default: return "32Bit";
                }
            }
            set
            {
                if (value == "8Bit")
                    rb8Bit.Checked = true;
                else if (value == "16Bit")
                    rb16Bit.Checked = true;
                else
                    rb32Bit.Checked = true;
            }
        }

        public uint CurrentAddress
        {
            get
            {
                switch (this.CheckedMemorySize)
                {
                    case ARMPluginInterfaces.MemorySize.Byte: return tbAddress.Value;
                    case ARMPluginInterfaces.MemorySize.HalfWord: return tbAddress.Value & 0xfffffffe;
                    default: return Utils.valid_address(tbAddress.Value);
                }
            }//get
            set
            {
                switch (this.CheckedMemorySize)
                {
                    case ARMPluginInterfaces.MemorySize.Byte: tbAddress.Value = value; break;
                    case ARMPluginInterfaces.MemorySize.HalfWord: tbAddress.Value = value & 0xfffffffe; break;
                    default: tbAddress.Value = Utils.valid_address(value); break;
                }
            }//set
        }//CurrentAddress

        public string CurrentAddressString
        {
            get
            {
                return this.CurrentAddress.ToString("X8");
            }//get
            set
            {
                try
                {
                    uint address = Convert.ToUInt32(value, 16);
                    this.CurrentAddress = address;
                }
                catch (Exception)
                {
                }
            }//set
        }//CurrentAddressString

        public static string Prefix { get { return "MemoryView"; } }

        public void memoryChangedHandler(uint address, ARMPluginInterfaces.MemorySize ms, uint oldValue, uint newValue)
        {
            if (mChangedMemory == null || address < mChangedMemoryStart) return;

            uint memAddress = address - mChangedMemoryStart;
            for (int ii = 0; ii < (int)ms; ii++, memAddress++)
            {
                if (memAddress < mChangedMemory.Length)
                {
                    mChangedMemory[memAddress] = true;
                }//if
            }//for ii
        }//memoryChangedHandler

        private static string MemoryToString(uint data, ARMPluginInterfaces.MemorySize ms)
        {
            switch (ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte: return data.ToString("X2");
                case ARMPluginInterfaces.MemorySize.HalfWord: return data.ToString("X4");
                case ARMPluginInterfaces.MemorySize.Word: return data.ToString("X8");
                //                case ARMPluginInterfaces.MemorySize.HalfWord: return Utils.reverseBinary(data, ARMPluginInterfaces.MemorySize.HalfWord).ToString("X4");
                //                case ARMPluginInterfaces.MemorySize.Word: return Utils.reverseBinary(data, ARMPluginInterfaces.MemorySize.Word).ToString("X8");
                default: return "";
            }
        }

        private static string MemoryToUnknownString(ARMPluginInterfaces.MemorySize ms)
        {
            switch (ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte: return "??";
                case ARMPluginInterfaces.MemorySize.HalfWord: return "????";
                case ARMPluginInterfaces.MemorySize.Word: return "????????";
                default: return "";
            }
        }

        private int CellWidth(ARMPluginInterfaces.MemorySize ms)
        {
            switch (ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte: return mCharSize.Width * 3;
                case ARMPluginInterfaces.MemorySize.HalfWord: return mCharSize.Width * 5;
                case ARMPluginInterfaces.MemorySize.Word: return mCharSize.Width * 10;
                default: return 0;
            }
        }

        private uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            return _JM.GetMemoryNoSideEffect(address, ms);
        }

        private bool get_changed(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            if (mChangedMemory == null || address < mChangedMemoryStart) return false;

            bool changed = false;
            uint memAddress = address - mChangedMemoryStart;
            for (int ii = 0; ii < (int)ms; ii++, memAddress++)
            {
                if (memAddress < mChangedMemory.Length)
                {
                    if (mChangedMemory[memAddress])
                    {
                        changed = true;
                    }
                }//if
            }//for ii
            return changed;
        }

        private void drawNormalLineCell(Graphics formGraphics, uint address, Point startPoint, int cellNo, ARMPluginInterfaces.MemorySize ms, SolidBrush textBrush)
        {
            Brush drawBrush = get_changed(address, ms) ? new SolidBrush(_highlightColor) : textBrush;
            string dataChars;
            if (_JM.InRange(address, ARMPluginInterfaces.MemorySize.Byte))
            {
                uint data = this.GetMemory(address, ms);
                dataChars = MemoryToString(data, ms);
            }
            else
            {
                dataChars = MemoryToUnknownString(ms);
            }

            int xpos = startPoint.X + (cellNo * CellWidth(ms));
            for (int kk = 0; kk < dataChars.Length; kk++, xpos += mCharSize.Width)
            {
                formGraphics.DrawString(dataChars.Substring(kk, 1), CurrentFont, drawBrush, xpos, startPoint.Y);
            } //for kk
            if (drawBrush != textBrush) drawBrush.Dispose();
        }

        private uint drawLineWords(Graphics formGraphics, uint address, Point startPoint, SolidBrush textBrush)
        {
            for (int jj = 0; jj < mNumCellsPerLine; jj++, address += 4)
            {
                drawNormalLineCell(formGraphics, address, startPoint, jj, ARMPluginInterfaces.MemorySize.Word, textBrush);
            }//for jj
            return address;
        }//drawLineWords

        private uint drawLineHalfWords(Graphics formGraphics, uint address, Point startPoint, SolidBrush textBrush)
        {
            for (int jj = 0; jj < mNumCellsPerLine; jj++, address += 2)
            {
                drawNormalLineCell(formGraphics, address, startPoint, jj, ARMPluginInterfaces.MemorySize.HalfWord, textBrush);
            }//for jj
            return address;
        }//drawLineHalfWords

        private uint drawLineBytes(Graphics formGraphics, uint address, Point startPoint, SolidBrush textBrush)
        {
            for (int jj = 0; jj < mNumCellsPerLine; jj++, address++)
            {
                drawNormalLineCell(formGraphics, address, startPoint, jj, ARMPluginInterfaces.MemorySize.Byte, textBrush);

                char c = '.';
                if (_JM.InRange(address, ARMPluginInterfaces.MemorySize.Byte))
                {
                    char ch = (char)this.GetMemory(address, ARMPluginInterfaces.MemorySize.Byte);
                    if (Char.IsLetterOrDigit(ch) || Char.IsPunctuation(ch) || Char.IsWhiteSpace(ch)) c = ch;
                }

                int xpos = (((3 * mNumCellsPerLine) + jj) * mCharSize.Width) + startPoint.X;
                formGraphics.DrawString(new string(c, 1), panel1.Font, textBrush, xpos, startPoint.Y);
            }//for jj
            return address;
        }//drawLineBytes

        public void stepStart()
        {
            this.clearChangedMemory();
            _JM.SetMemoryChangedHandler(memoryChangedHandler);
        }

        public void stepEnd()
        {
            _JM.SetMemoryChangedHandler(null);
        }

        private void clearChangedMemory()
        {
            if (mChangedMemory != null)
            {
                for (int ii = 0; ii < mChangedMemory.Length; ii++)
                {
                    mChangedMemory[ii] = false;
                }
            }
        }

        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                _graphicElements.loadFromXML(xmlIn);

                //string viewIndex = xmlIn.GetAttribute(0);
                //string wordSize = xmlIn.GetAttribute(1);
                //string address = xmlIn.GetAttribute(2);
                string viewIndex = xmlIn["Index"];
                string wordSize = xmlIn["WordSize"];
                string address = xmlIn["Address"];

                this.CurrentAddressString = address;
                this.CheckedMemorySizeString = wordSize;
                this.Index = int.Parse(viewIndex);
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.defaultSettings();
            }
        }

        private void MemoryView_Resize(object sender, EventArgs e)
        {
            panel1.Height = this.Size.Height - groupBox1.Size.Height - 1;
            panel1.Width = this.Size.Width;
            groupBox1.Location = new Point(this.Width - groupBox1.Width, 0);
            this.resetView();
        }

        public void saveState(XmlWriter xmlOut)
        {
            //Memory0
            xmlOut.WriteStartElement(this.ViewName);
            xmlOut.WriteAttributeString("Index", this.Index.ToString());
            xmlOut.WriteAttributeString("WordSize", this.CheckedMemorySizeString);
            xmlOut.WriteAttributeString("Address", this.CurrentAddressString);
            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();
        }

        private void menuItem_CloseView(object sender, System.EventArgs e)
        {
            //this.Close();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();

            ToolStripMenuItem closeItem = new ToolStripMenuItem("&Close View", null, this.menuItem_CloseView);

            cms.Items.Add(closeItem);
            _graphicElements.Popup(cms, false);
        }




        private void tbAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                e.Handled = true;
                this.resetView();
            }
        }//tbAddress_KeyDown

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!_JM.ValidLoadedProgram)
                return;

            Graphics formGraphics = e.Graphics;
            Size fontSize = (formGraphics.MeasureString("0123456789abcdef", panel1.Font)).ToSize();
            mCharSize = new Size(fontSize.Width/16, fontSize.Height);

            Point startPoint = new Point(mCharSize.Width*10, 0);
            int spaceLeft = panel1.ClientSize.Width - startPoint.X;

            switch (this.CheckedMemorySize)
            {
                case ARMPluginInterfaces.MemorySize.Byte:
                    mNumCellsPerLine = (spaceLeft/(mCharSize.Width*4));
                    mBytesPerCell = 1;
                    break;
                case ARMPluginInterfaces.MemorySize.HalfWord:
                    mNumCellsPerLine = (spaceLeft/(mCharSize.Width*5));
                    mBytesPerCell = 2;
                    break;
                case ARMPluginInterfaces.MemorySize.Word:
                    mNumCellsPerLine = (spaceLeft/(mCharSize.Width*10));
                    mBytesPerCell = 4;
                    break;
                default:
                    return;
            }//switch

            uint address = this.CurrentAddress;
            mNumLines = panel1.Height/mCharSize.Height;

            if (mChangedMemory == null)
            {
                uint sizeBytes = (uint) (mNumLines*mNumCellsPerLine*mBytesPerCell);
                mChangedMemory = new bool[sizeBytes];
                mChangedMemoryStart = address;
                clearChangedMemory();
            } //if

            using (SolidBrush textBrush = new SolidBrush(this.CurrentTextColour))
            {
                for (int ii = 0; ii < mNumLines; ii++)
                {
                    string drawString = address.ToString("X8");
                    formGraphics.DrawString(drawString, panel1.Font, textBrush, 0, startPoint.Y);

                    try
                    {
                        switch (this.CheckedMemorySize)
                        {
                            case ARMPluginInterfaces.MemorySize.Byte:
                                address = drawLineBytes(formGraphics, address, startPoint, textBrush);
                                break;
                            case ARMPluginInterfaces.MemorySize.HalfWord:
                                address = drawLineHalfWords(formGraphics, address, startPoint, textBrush);
                                break;
                            case ARMPluginInterfaces.MemorySize.Word:
                                address = drawLineWords(formGraphics, address, startPoint, textBrush);
                                break;
                        } //switch
                    }
                    catch (ARMPluginInterfaces.MemoryAccessException ex)
                    {
                        ARMPluginInterfaces.Utils.OutputDebugString("bad memory access, reason:" + ex.Reason);
                        //System.Console.WriteLine("bad memory access, reason:" + ex.Reason);
                    }
                    startPoint.Y += mCharSize.Height;
                } //for ii
            }
        }//panel1_Paint

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollEventType scrollET = e.Type;
            uint address = tbAddress.Value;
            if (scrollET == ScrollEventType.SmallIncrement)
            {
                address += this.NumberBytesWindow;
            }//if
            else if (scrollET == ScrollEventType.SmallDecrement)
            {
                if (address <= this.NumberBytesWindow)
                    address = 0;
                else
                    address -= this.NumberBytesWindow;
            }//elseif
            else
                return;

            tbAddress.Value = address;
            this.resetView();

        }

        private void rbBit_CheckedChanged(object sender, EventArgs e)
        {
            this.resetView();
        }

        public void defaultSettings()
        {
            this.CurrentAddress = 0;
            this.CheckedMemorySize = ARMPluginInterfaces.MemorySize.Byte;
            this.Index = 0;
            this._highlightColor = Color.Red;
        }

        public void TerminateInput() { }

    }//class MemoryView
}
