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
    public abstract partial class CacheView : UserControl, IView, IViewXMLSettings
    {
        private Color mHighlightColor;
        private Size mCharSize;
        private int mSetOutlineHeight;
        private int mStepSize;
        private int mIndentSize;

        private bool[] mChangedCacheLines;

        private int mVScrollPosition;
        private int mHScrollPosition;

        private GraphicElements _graphicElements;

        protected ApplicationJimulator _JM;

        public bool CacheEnabled { get; set; }

        /// <summary>
        /// Default constructor required for VS designer
        /// </summary>
        protected CacheView()
        {
        }

        /// <summary>
        /// CacheView ctor
        /// </summary>
        protected CacheView(ApplicationJimulator jm)
            : this()
        {
            _JM = jm;
            InitializeComponent();

            this.defaultSettings();
            _graphicElements = new GraphicElements(this);
        }

        public abstract ARMSim.Simulator.Cache.L1Cache CacheMemory { get; }

        public void LoadFromXML(XmlReader xmlIn)
        {
            _graphicElements.loadFromXML(xmlIn);
        }

        public void defaultSettings()
        {
            mHighlightColor = Color.Red;
        }

        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(this.Text);
            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();
        }

        public Font CurrentFont
        {
            get { return panel1.Font; }
            set { panel1.Font = value; panel1.Invalidate(); }
        }

        public Color CurrentHighlightColour
        {
            get { return this.mHighlightColor; }
            set { this.mHighlightColor = value; panel1.Invalidate(); }
        }

        public Color CurrentTextColour
        {
            get { return panel1.ForeColor; }
            set { panel1.ForeColor = value; panel1.Invalidate(); }
        }

        public Color CurrentBackgroundColour
        {
            get { return panel1.BackColor; }
            set { panel1.BackColor = value; panel1.Invalidate(); }
        }
        public Control ViewControl { get { return this; } }

        public void resetView()
        {
            if (!this.CacheEnabled)
                return;

            mVScrollPosition = 0;
            mHScrollPosition = 0;
            vScrollBar1.Value = 0;
            hScrollBar1.Value = 0;
            mChangedCacheLines = null;
            panel1.Invalidate();
        }
        public void updateView()
        {
            if (!this.CacheEnabled)
                return;

            panel1.Invalidate();
        }

        //Called when a step operation is about to start.
        //Gives us a chance to setup to record changes to the cache
        //1) Allocate a bool array, one for every cache line
        //2) Set the cache changed callback to our handler
        // The stepEnd function will deallocate this stuff
        public void stepStart()
        {
            if (!this.CacheEnabled)
                return;

            int numLines = (this.CacheMemory.Sets.Length * this.CacheMemory.Sets[0].Blocks.Length);
            mChangedCacheLines = new bool[numLines];
            this.CacheMemory.CacheChangedHandler = CodeCacheChangedHandler;

        }//stepStart

        //Called when a step operation has ended
        //This gives us a chance to reset the changed handler to null
        public void stepEnd()
        {
            if (!this.CacheEnabled)
                return;

            this.CacheMemory.CacheChangedHandler = null;
        }

        //Called when the cache has changed
        //Simply record the changed cache line in the bool array
        //When time comes to display the cache, this array is consulted
        public void CodeCacheChangedHandler(uint line, uint word)
        {
            if (!this.CacheEnabled)
                return;

            mChangedCacheLines[line] = true;
        }

        private int VY(int y) { return y - mVScrollPosition; }
        private int VX(int x) { return x - mHScrollPosition; }

        private void recomputeSizes(Graphics g)
        {
            if (!_JM.ValidLoadedProgram)
                return;

            Size fontSize = (g.MeasureString("0123456789abcdef", panel1.Font)).ToSize();
            mCharSize = new Size(fontSize.Width / 16, fontSize.Height);

            mStepSize = mCharSize.Width / 2;
            mIndentSize = mCharSize.Width;

            Simulator.Cache.CacheSet[] sets = this.CacheMemory.Sets;
            int numSets = sets.Length;
            int linesPerSet = sets[0].Blocks.Length;

            mSetOutlineHeight = (linesPerSet * mCharSize.Height) + ((linesPerSet + 1) * mStepSize);

            //			int totalHeight = numSets * (mSetOutlineHeight + (mStepSize*2) );
            int totalHeight = numSets * mSetOutlineHeight;
            bool showVScroll = totalHeight >= (panel1.Height - hScrollBar1.Height);
            vScrollBar1.Visible = showVScroll;

            vScrollBar1.SmallChange = mCharSize.Height;
            vScrollBar1.LargeChange = mSetOutlineHeight;
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = totalHeight;

            int numWordsPerLine = sets[0].Blocks[0].Data.Length;
            int totalWidth = (mStepSize * 3) + (mCharSize.Width * 12) + ((mCharSize.Width * 10) * numWordsPerLine);
            bool showHScroll = totalWidth >= (panel1.Width - vScrollBar1.Width);
            hScrollBar1.Visible = showHScroll;

            hScrollBar1.SmallChange = (mCharSize.Width * 10);
            hScrollBar1.LargeChange = (mCharSize.Width * 10) * 2;
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = totalWidth;


        }//recomputeSizes

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!_JM.ValidLoadedProgram)
                return;

            if (this.CacheMemory.Sets == null)
                return;

            if (!this.CacheEnabled)
                return;

            Graphics g = e.Graphics;
            recomputeSizes(g);

            Simulator.Cache.CacheSet[] sets = this.CacheMemory.Sets;
            int numSets = sets.Length;
            int linesPerSet = sets[0].Blocks.Length;

            using (Pen pen = new Pen(Brushes.Blue, 4)) {
                int thisLine = 0;
                int sy = 1;
                for (int ii = 0; ii < numSets; ii++)
                {
                    Simulator.Cache.CacheBlock[] blocks = sets[ii].Blocks;

                    int ly = sy + mStepSize;
                    for (int jj = 0; jj < linesPerSet; jj++, thisLine++)
                    {
                        if (blocks[jj].Dirty)
                        {
                            Rectangle rect = new Rectangle(VX(mStepSize), VY(ly + mStepSize), mStepSize * 2, mStepSize * 2);
                            using (SolidBrush b = new SolidBrush(mHighlightColor))
                            {
                                g.FillEllipse(b, rect);
                            }
                        }

                        Color color = (mChangedCacheLines != null && mChangedCacheLines[thisLine]) ? mHighlightColor : panel1.ForeColor;
                        using (SolidBrush drawBrush = new SolidBrush(color))
                        {

                            bool valid = blocks[jj].Valid;

                            uint[] data = blocks[jj].Data;
                            uint address = blocks[jj].Tag;

                            int wx = mStepSize * 3;
                            string str = valid ? address.ToString("x8") : new string('?', 8);
                            str += ":";
                            g.DrawString(str, panel1.Font, drawBrush, VX(wx), VY(ly));
                            wx = wx + (mCharSize.Width * 12);

                            for (int kk = 0; kk < data.Length; kk++)
                            {
                                str = valid ? data[kk].ToString("x8") : new string('?', 8);
                                g.DrawString(str, panel1.Font, drawBrush, VX(wx), VY(ly));
                                wx = wx + (mCharSize.Width * 10);
                            }
                            ly = ly + mCharSize.Height + mStepSize;
                        }
                    }

                    g.DrawLine(pen, new Point(VX(mIndentSize), VY(sy)), new Point(VX(0), VY(sy)));
                    g.DrawLine(pen, new Point(VX(0), VY(sy)), new Point(VX(0), VY(sy + mSetOutlineHeight)));
                    g.DrawLine(pen, new Point(VX(0), VY(sy + mSetOutlineHeight)), new Point(VX(mIndentSize), VY(sy + mSetOutlineHeight)));

                    sy = sy + mSetOutlineHeight + (mStepSize * 2);
                }//for ii
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (!_JM.ValidLoadedProgram)
                return;

            if (!this.CacheEnabled)
                return;

            Graphics g = panel1.CreateGraphics();
            recomputeSizes(g);
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            mVScrollPosition = e.NewValue;
            panel1.Invalidate();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            mHScrollPosition = e.NewValue;
            panel1.Invalidate();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();
            _graphicElements.Popup(cms, false);
        }

        public void TerminateInput() { }

    }//class CacheView
}
