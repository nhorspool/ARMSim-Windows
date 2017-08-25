using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    /// <summary>
    /// This class handles the graphical part of the lcd 2 line display simulation.
    /// </summary>
    public partial class TwoLineLCDDisplay : UserControl
    {
        /// <summary>
        /// Flag indicating if the cursor is currently on or off while blinking.
        /// </summary>
        private bool mCursorOn;

        /// <summary>
        /// The buffer that holds the contents of the 2 line 16 character display
        /// </summary>
        private byte[,] buffer = new byte[2, 16];

        /// <summary>
        /// When the display is enabled, use this back color
        /// </summary>
        private Color mBackgroundEnabled = Color.Lime;

        /// <summary>
        /// When the display is Disabled, use this back color
        /// </summary>
        private Color mBackgroundDisabled = Color.DarkGreen;

        /// <summary>
        /// The current cursor location
        /// </summary>
        private CursorLocation mCursorLocation = new CursorLocation();
        public CursorLocation CurrentCursorLocation { get { return mCursorLocation; } }

        /// <summary>
        /// The direction the cursor shifts when a new character is written
        /// </summary>
        public bool CursorDirectionRight { get; set; }

        /// <summary>
        /// Flag indicates if the display shifts when a character is written to the end of display????
        /// (NOT implemented)
        /// </summary>
        public bool DisplayShifted { get; set; }

        /// <summary>
        /// Flag indicates if the cursor is blinking.
        /// </summary>
        public bool CursorBlinking { get; set; }

        /// <summary>
        /// Flag indicates if the cursor is enabled.
        /// </summary>
        public bool CursorEnabled { get; set; }

        /// <summary>
        /// Flag and accessor for display enabled flag.
        /// When set we will set the background color.
        /// </summary>
        private bool mDisplayEnabled;
        public bool DisplayEnabled
        {
            get { return mDisplayEnabled; }
            set
            {
                mDisplayEnabled = value;
                this.BackColor = mDisplayEnabled ? mBackgroundEnabled : mBackgroundDisabled;
            }//set
        }//DisplayEnabled

        /// <summary>
        /// Construct an LCD display control.
        /// </summary>
        public TwoLineLCDDisplay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shift the display either left or right 1 position.
        /// A blank character is inserted into the start position, the
        /// last character is lost.
        /// </summary>
        /// <param name="shiftRight"></param>
        public void ShiftDisplay(bool shiftRight)
        {
            if (shiftRight)
            {
                for (int xpos = 15; xpos > 0; xpos--)
                {
                    buffer[0, xpos] = buffer[0, xpos - 1];
                    buffer[1, xpos] = buffer[1, xpos - 1];
                }
                //insert blanks into open position
                buffer[0, 0] = (byte)' ';
                buffer[1, 0] = (byte)' ';
            }
            else
            {
                for (int xpos = 0; xpos < 15; xpos++)
                {
                    buffer[0, xpos] = buffer[0, xpos + 1];
                    buffer[1, xpos] = buffer[1, xpos + 1];
                }
                //insert blanks into open position
                buffer[0, 15] = (byte)' ';
                buffer[1, 15] = (byte)' ';
            }
        }//ShiftDisplay

        /// <summary>
        /// Clear the LCD display. Set all characters to blank.
        /// Note:this does not change the cursor location
        /// </summary>
        public void ClearLCD()
        {
            for (int ii = 0; ii < 2; ii++)
            {
                for (int jj = 0; jj < 16; jj++)
                {
                    buffer[ii, jj] = (byte)' ';
                }
            }
        }//ClearLCD

        /// <summary>
        /// Put a character at the current cursor location.
        /// Cursor is advanced to its new location.
        /// </summary>
        /// <param name="c">character to write</param>
        public void PutChar(int c)
        {
            buffer[CurrentCursorLocation.YPos, CurrentCursorLocation.XPos] = (byte)(c & 0x7f);
            CurrentCursorLocation.Shift(this.CursorDirectionRight);
        }//PutChar

        /// <summary>
        /// Get the character at the current cursor location.
        /// </summary>
        /// <returns></returns>
        public byte GetChar()
        {
            return buffer[CurrentCursorLocation.YPos, CurrentCursorLocation.XPos];
        }//GetChar

        /// <summary>
        /// Helper function to convert a xpos(0-15) into a pixel offset into the display.
        /// Note:origin(0,0) is top left corner
        /// Each char is 30 pixels wide.
        /// </summary>
        /// <param name="xpos"></param>
        /// <returns></returns>
        private static int XPosToPixel(int xpos)
        {
            return (CursorLocation.ValidXPos(xpos) * 30) + 1;
        }//XPosToPixel

        /// <summary>
        /// Helper function to convert a ypos(0-1) into a pixel offset into the display.
        /// Note:origin(0,0) is top left corner
        /// Each char is 40 pixels high.
        /// </summary>
        /// <param name="ypos"></param>
        /// <returns></returns>
        private static int YPosToPixel(int ypos)
        {
            return (CursorLocation.ValidYPos(ypos) * 40) + 4;
        }//YPosToPixel

        /// <summary>
        /// Draw the character at the character coordinate.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="xpos">0-15</param>
        /// <param name="ypos">0-1</param>
        /// <param name="index">character index</param>
        private void DrawChar(Graphics g, int xpos, int ypos, int index)
        {
            int xx = XPosToPixel(xpos);
            int yy = YPosToPixel(ypos);
            int ii = (index & 0x7f);
            g.DrawImageUnscaled(mImages[ii], new Point(xx, yy));
        }//DrawChar

        /// <summary>
        /// Draw the cursor at the x,y location. Flag indicates if it is drawn or erased
        /// to simulate blinking.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="erase"></param>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        private void DrawCursor(Graphics g, bool erase, int xpos, int ypos)
        {
            int xx = XPosToPixel(xpos);
            int yy = YPosToPixel(ypos) + 36;
            using (Pen pen = erase ? new Pen(this.BackColor, 3.0F) : new Pen(Color.Black, 3.0F))
            {
                g.DrawLine(pen, xx, yy, xx + 26, yy);
            }
        }//DrawCursor

        /// <summary>
        /// Paint the LCD display.
        /// If the display is disabled then do nothing. Only the background color
        /// will show.
        /// Draw the contents of the character buffer, then draw the cursor(if enabled)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwoLineLCDDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (!this.DisplayEnabled)
                return;

            Graphics g = e.Graphics;
            for (int ii = 0; ii < 2; ii++)
            {
                for (int jj = 0; jj < 16; jj++)
                {
                    DrawChar(g, jj, ii, buffer[ii, jj]);
                }
            }

            if (this.CursorEnabled)
            {
                DrawCursor(g, mCursorOn, CurrentCursorLocation.XPos, CurrentCursorLocation.YPos);
            }

        }//TwoLineLCDDisplay_Paint

        /// <summary>
        /// If the size of the display changes we must force the display to be exactly 480x85 pixels.
        /// Windows forms may change the size to adjust for fonts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwoLineLCDDisplay_SizeChanged(object sender, EventArgs e)
        {
            if (this.Width != 480)
                this.Width = 480;

            if (this.Height != 85)
                this.Height = 85;

        }//TwoLineLCDDisplay_SizeChanged

        /// <summary>
        /// Cursor timer tick. Either draw or erase the cursor(if enabled)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if the cursor is either not enabled or not blinking, do nothing
            if (!this.CursorEnabled || !this.CursorBlinking)
                return;

            //set cursor state
            mCursorOn = mCursorOn ? false : true;

            //compute a rectangle containing the cursor and invalidate
            //only that region. This makes the paint more efficient
            int xx = XPosToPixel(CurrentCursorLocation.XPos);
            int yy = YPosToPixel(CurrentCursorLocation.YPos) + 35;
            Rectangle rect = new Rectangle(xx, yy, 26, 3);
            this.Invalidate(rect);
        }//timer1_Tick

        /// <summary>
        /// When the lcd control is loaded. Create the 128 bitmaps for the
        /// ascii codes 0-0x7f
        /// Start the timer for the cursor blink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwoLineLCDDisplay_Load(object sender, EventArgs e)
        {
            CreateBitmapImages();
            this.timer1.Enabled = true;
            this.timer1.Start();
        }//TwoLineLCDDisplay_Load

        /// <summary>
        /// This class represents the current cursor location. It also has some helper methods
        /// to validate corrdinates and do some conversions between command bit locations and ints.
        /// </summary>
        public class CursorLocation
        {
            //the current x,y location of the cursor
            private int xpos;       //0-15
            private int ypos;       //0-1

            public int XPos
            {
                get { return xpos; }
                set { xpos = ValidXPos(value); }
            }
            public int YPos
            {
                get { return ypos; }
                set { ypos = ValidYPos(value); }
            }

            /// <summary>
            /// Make the given xpos valid.
            /// </summary>
            /// <param name="xpos"></param>
            /// <returns></returns>
            public static int ValidXPos(int xpos)
            {
                return Math.Max(Math.Min(xpos, 15), 0);
            }

            /// <summary>
            /// Make the given ypos valid.
            /// </summary>
            /// <param name="ypos"></param>
            /// <returns></returns>
            public static int ValidYPos(int ypos)
            {
                return ypos == 0 ? 0 : 1;
            }

            /// <summary>
            /// Set the cursor location based on the command register write value.
            /// </summary>
            /// <param name="cmd"></param>
            public void SetCursorPosition(uint cmd)
            {
                YPos = ((cmd & 0x40) != 0) ? 1 : 0;
                XPos = (int)(cmd & 0x0f);
            }

            /// <summary>
            /// Convert the current cursor location to a register value.
            /// </summary>
            /// <returns></returns>
            public uint ToInt()
            {
                int ret = (ypos == 0) ? 0x00 : 0x40;
                ret |= xpos;
                return (uint)ret;
            }

            /// <summary>
            /// Set the cursor to the home position(line 0, position 0)
            /// </summary>
            public void Home()
            {
                XPos = 0;
                YPos = 0;
            }

            /// <summary>
            /// Shift the display to the right or left one position.
            /// 
            /// </summary>
            /// <param name="shiftRight"></param>
            public void Shift(bool shiftRight)
            {
                xpos += shiftRight ? 1 : -1;
                if (xpos >= 16)
                {
                    xpos = 0;
                    ypos = (ypos == 0) ? 1 : 0;
                }
                else if (xpos < 0)
                {
                    xpos = 15;
                    ypos = (ypos == 0) ? 1 : 0;
                }
            }//Shift

        }//class CursorLocation

    }//class TwoLineLCDDisplay
}
