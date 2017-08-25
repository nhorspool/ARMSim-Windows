using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class MatrixStackDisplay : UserControl
    {
        private int mVScrollPosition;
        private int mHScrollPosition;

        private Size mCharSize;
        private int mStepSize;
        private int mIndentSize;
        private int mMatrixIndex;

        private MatrixStackDisplay.MatrixDef[] mMatrices;

        public MatrixStackDisplay()
        {
            InitializeComponent();
        }

        public void UpdateMatrixDisplay(MatrixStackDisplay.MatrixDef[] matrices)
        {
            mMatrices = matrices;
            panel1.Invalidate();
        }

        public struct MatrixDef
        {
            public int rows;
            public int cols;
            public double[] elements;
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

        public void panel1_Resize(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            recomputeSizes(g);
            panel1.Invalidate(true);
        }

        private void recomputeSizes(Graphics g)
        {
            if (mMatrices == null || mMatrices.Length == 0)
                return;

            Size fontSize = (g.MeasureString("0123456789abcdef", panel1.Font)).ToSize();
            mCharSize = new Size(fontSize.Width / 16, fontSize.Height);

            mStepSize = mCharSize.Width / 2;
            mIndentSize = mCharSize.Width;
            mMatrixIndex = mCharSize.Width * 2;

            int totalHeightRows = 0;
            int maxWidthCols = 0;
            for (int ii = 0; ii < mMatrices.Length; ii++)
            {
                totalHeightRows += mMatrices[ii].rows;
                if (mMatrices[ii].cols > maxWidthCols)
                    maxWidthCols = mMatrices[ii].cols;
            }

            int totalHeightPixels = (totalHeightRows * mCharSize.Height) + ((mMatrices.Length * 2) * mStepSize);
            bool showVScroll = totalHeightPixels >= (panel1.Height - hScrollBar1.Height);
            vScrollBar1.Visible = showVScroll;

            vScrollBar1.SmallChange = mCharSize.Height;
            vScrollBar1.LargeChange = mCharSize.Height * 5;
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = totalHeightPixels;

            int totalWidthPixels = ((maxWidthCols * 5) * mCharSize.Width) + (2 * mIndentSize);
            bool showHScroll = totalWidthPixels >= (panel1.Width - vScrollBar1.Width);
            hScrollBar1.Visible = showHScroll;

            hScrollBar1.SmallChange = (mCharSize.Width * 10);
            hScrollBar1.LargeChange = (mCharSize.Width * 10) * 2;
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = totalWidthPixels;

        }//recomputeSizes


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (mMatrices == null)
                return;

            Graphics g = e.Graphics;
            recomputeSizes(g);

            int ypos = 3;
            for (int matrixNum = 0; matrixNum < mMatrices.Length; matrixNum++)
            {
                MatrixDef md = mMatrices[matrixNum];
                ypos += DrawMatrix(matrixNum, ypos, md, g);
            }//for matrixNum
        }//panel1_Paint

        private int VY(int y) { return y - mVScrollPosition; }
        private int VX(int x) { return x - mHScrollPosition; }

        private int DrawMatrix(int matrixNum, int ypos, MatrixDef md, Graphics g)
        {
            Font font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            SolidBrush greenBrush = new SolidBrush(Color.Green);
            int outlineHeight = (md.rows * mCharSize.Height) + (2 * mStepSize);
            g.DrawString(matrixNum.ToString(), font, greenBrush, VX(0), VY(ypos + (outlineHeight / 2) - (mCharSize.Height / 2)));

            Pen pen = new Pen(Brushes.Blue, 4);
            g.DrawLine(pen, new Point(VX(mMatrixIndex + mIndentSize), VY(ypos)), new Point(VX(mMatrixIndex + 3), VY(ypos)));
            g.DrawLine(pen, new Point(VX(mMatrixIndex + 3), VY(ypos)), new Point(VX(mMatrixIndex + 3), VY(ypos + outlineHeight)));
            g.DrawLine(pen, new Point(VX(mMatrixIndex + 3), VY(ypos + outlineHeight)), new Point(VX(mMatrixIndex + mIndentSize), VY(ypos + outlineHeight)));

            SolidBrush drawBrush = new SolidBrush(Color.Black);


            int[] columnWidths = new int[md.cols];
            int totalWidthPixels = 0;
            for (int col = 0; col < md.cols; col++)
            {
                int maxWidth = -1;
                for (int row = 0; row < md.rows; row++)
                {
                    double value = md.elements[(row * md.cols) + col];
                    //string str = string.Format("{0,0:F2}", value);
                    string str = string.Format("{0:0.00}", value);
                    int strWidth = (int)((g.MeasureString(str, font).Width) + 1);
                    if (strWidth > maxWidth)
                        maxWidth = strWidth;

                }
                columnWidths[col] = maxWidth;
                totalWidthPixels += maxWidth;
            }
            totalWidthPixels += ((md.cols - 1) * mCharSize.Width);

            for (int row = 0; row < md.rows; row++)
            {
                int ly = ypos + (row * mCharSize.Height) + (mCharSize.Height / 2);
                int wx = mCharSize.Width + mMatrixIndex;
                for (int col = 0; col < md.cols; col++)
                {
                    double value = md.elements[(row * md.cols) + col];
                    string str = string.Format("{0:0.00}", value);

                    Rectangle rect = new Rectangle(VX(wx), VY(ly), columnWidths[col], mCharSize.Height);

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Far;
                    g.DrawString(str, font, drawBrush, rect, sf);
                    //g.DrawString(str, font, drawBrush, VX(wx), VY(ly));

                    wx += (columnWidths[col]) + mCharSize.Width;
                }
            }

            //int totalWidthPixels = ((md.cols * 5) * mCharSize.Width) + (2 * mIndentSize);
            totalWidthPixels += (2 * mIndentSize);
            g.DrawLine(pen, new Point(VX(mMatrixIndex + totalWidthPixels - mIndentSize), VY(ypos)), new Point(VX(mMatrixIndex + totalWidthPixels), VY(ypos)));
            g.DrawLine(pen, new Point(VX(mMatrixIndex + totalWidthPixels), VY(ypos)), new Point(VX(mMatrixIndex + totalWidthPixels), VY(ypos + outlineHeight)));
            g.DrawLine(pen, new Point(VX(mMatrixIndex + totalWidthPixels), VY(ypos + outlineHeight)), new Point(VX(mMatrixIndex + totalWidthPixels - mIndentSize), VY(ypos + outlineHeight)));

            return outlineHeight + (2 * mStepSize);
        }

    }
}
