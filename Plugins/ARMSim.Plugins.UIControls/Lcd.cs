using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class Lcd : UserControl
    {
        private const int mMaxWidth = 40;
        private const int mMaxHeight = 15;

        private int mCharWidth;
        private int mCharHeight;
        private char[,] _text = new char[mMaxWidth, mMaxHeight];
        private SolidBrush _textBrush = new SolidBrush(Color.Black);

        public Lcd()
        {
            InitializeComponent();

            Graphics g = this.CreateGraphics();
            SizeF size = g.MeasureString("ABCDEFG", this.Font);

            mCharWidth = (int)((int)size.Width / 7);
            mCharHeight = (int)size.Height;
            this.ClearScreen();

        }//Lcd ctor

        //public void Init()
        //{

        //    int displayWidth = mCharWidth * mMaxWidth;
        //    //displayWidth += displayWidth / 10;

        //    int displayHeight = mCharHeight * mMaxHeight;
        //    //displayHeight += displayHeight / 10;

        //    //this.Size = new Size(displayWidth, displayHeight);
        //}

        public void PrintString(uint xpos, uint ypos, string str)
        {
            if (string.IsNullOrEmpty(str))
                return;

            foreach (char ch in str)
            {
                PrintChar(xpos++, ypos, ch);
            }
        }//PrintString

        public void PrintChar(uint xpos, uint ypos, char ch)
        {
            if (xpos >= mMaxWidth || ypos >= mMaxHeight)
                return;

            _text[xpos, ypos] = ch;
        }

        public void ClearLine(uint ypos)
        {
            if (ypos >= mMaxHeight)
                return;

            for (uint xpos = 0; xpos < mMaxWidth; xpos++)
            {
                PrintChar(xpos, ypos, ' ');
            }//for
        }//ClearLine

        public void ClearScreen()
        {
            for (int ypos = 0; ypos < mMaxHeight; ypos++)
            {
                for (int xpos = 0; xpos < mMaxWidth; xpos++)
                {
                    _text[xpos, ypos] = ' ';
                }//for xpos
            }//for ypos
        }//ClearScreen

        private void Lcd_Paint(object sender, PaintEventArgs e)
        {
            for (int ypos = 0; ypos < mMaxHeight; ypos++)
            {
                for (int xpos = 0; xpos < mMaxWidth; xpos++)
                {
                    int x = xpos * mCharWidth;
                    int y = ypos * mCharHeight;

                    char ch = _text[xpos, ypos];
                    if (ch == 127)
                    {
                        e.Graphics.FillRectangle(_textBrush, x + 1, y + 1, mCharWidth - 2, mCharHeight - 2);
                    }//if
                    else
                    {
                        string str = new string(ch, 1);
                        e.Graphics.DrawString(str, this.Font, _textBrush, new PointF(x, y));
                    }//else
                }//for xpos
            }//for ypos
        }//Lcd_Paint

    }//class Lcd
}
