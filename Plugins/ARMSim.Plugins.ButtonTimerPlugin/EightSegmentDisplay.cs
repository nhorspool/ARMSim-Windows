using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    public partial class EightSegmentDisplay : UserControl
    {
        [Flags]
        public enum SegmentFlags : byte
        {
            SEG_A=0x80,
            SEG_B=0x40,
            SEG_C=0x20,
            SEG_P=0x10,
            SEG_D=0x08,
            SEG_E=0x04,
            SEG_F=0x02,
            SEG_G=0x01
        }

        private SolidBrush mBrush = new SolidBrush(Color.LightGreen);

        private byte[] mDigits = new byte[]
        {
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_B|SegmentFlags.SEG_C|SegmentFlags.SEG_D|SegmentFlags.SEG_E|SegmentFlags.SEG_G),  //0
            (byte)(SegmentFlags.SEG_B|SegmentFlags.SEG_C),                                                                              //1
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_B|SegmentFlags.SEG_D|SegmentFlags.SEG_E|SegmentFlags.SEG_F),                     //2
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_B|SegmentFlags.SEG_C|SegmentFlags.SEG_D|SegmentFlags.SEG_F),                     //3
            (byte)(SegmentFlags.SEG_B|SegmentFlags.SEG_C|SegmentFlags.SEG_F|SegmentFlags.SEG_G),                                        //4
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_C|SegmentFlags.SEG_D|SegmentFlags.SEG_F|SegmentFlags.SEG_G),                     //5
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_C|SegmentFlags.SEG_D|SegmentFlags.SEG_E|SegmentFlags.SEG_F|SegmentFlags.SEG_G),  //6
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_B|SegmentFlags.SEG_C),                                                           //7
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_B|SegmentFlags.SEG_C|SegmentFlags.SEG_D|SegmentFlags.SEG_E|SegmentFlags.SEG_F|SegmentFlags.SEG_G),  //8
            (byte)(SegmentFlags.SEG_A|SegmentFlags.SEG_B|SegmentFlags.SEG_C|SegmentFlags.SEG_F|SegmentFlags.SEG_G),                     //9
            (byte)0
        };


        //.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_G		@0
        //.word	SEG_B|SEG_C					@1
        //.word	SEG_A|SEG_B|SEG_F|SEG_E|SEG_D			@2
        //.word	SEG_A|SEG_B|SEG_F|SEG_C|SEG_D			@3
        //.word	SEG_G|SEG_F|SEG_B|SEG_C				@4
        //.word	SEG_A|SEG_G|SEG_F|SEG_C|SEG_D			@5
        //.word	SEG_A|SEG_G|SEG_F|SEG_E|SEG_D|SEG_C		@6
        //.word	SEG_A|SEG_B|SEG_C				@7
        //.word	SEG_A|SEG_B|SEG_C|SEG_D|SEG_E|SEG_F|SEG_G	@8
        //.word	SEG_A|SEG_B|SEG_F|SEG_G|SEG_C			@9

        private class Segment
        {
            private int _xpos;
            private int _ypos;
            private bool _vertical;
            private Point[] _points;
            public Segment(bool vertical, int xpos, int ypos)
            {
                _xpos = xpos;
                _ypos = ypos;
                _vertical = vertical;

                _points = new Point[8];
                if (_vertical)
                {
                    int xoffset = _xpos - 33;
                    int yoffset = _ypos - 54;
                    _points[0] = new Point(xoffset + 33, yoffset + 54);
                    _points[1] = new Point(xoffset + 35, yoffset + 52);
                    _points[2] = new Point(xoffset + 35, yoffset + 34);
                    _points[3] = new Point(xoffset + 33, yoffset + 30);
                    _points[4] = new Point(xoffset + 32, yoffset + 30);
                    _points[5] = new Point(xoffset + 30, yoffset + 34);
                    _points[6] = new Point(xoffset + 30, yoffset + 52);
                    _points[7] = new Point(xoffset + 32, yoffset + 54);
                }
                else
                {
                    int xoffset = _xpos - 9;
                    int yoffset = _ypos - 55;
                    _points[0] = new Point(xoffset + 9, yoffset + 55);
                    _points[1] = new Point(xoffset + 11, yoffset + 53);
                    _points[2] = new Point(xoffset + 29, yoffset + 53);
                    _points[3] = new Point(xoffset + 31, yoffset + 55);
                    _points[4] = new Point(xoffset + 31, yoffset + 56);
                    _points[5] = new Point(xoffset + 29, yoffset + 57);
                    _points[6] = new Point(xoffset + 11, yoffset + 57);
                    _points[7] = new Point(xoffset + 9, yoffset + 56);
                }
            }
            public Point[] Points { get { return _points; } }
        }

        private byte mCode;
        private Segment[] mSegments;

        public EightSegmentDisplay()
        {
            InitializeComponent();

            mSegments = new Segment[8];
            mSegments[0] = new Segment(false, 9, 5);//A
            mSegments[1] = new Segment(true, 33, 29);//B
            mSegments[2] = new Segment(true, 33, 54);//C
            mSegments[3] = null;//P
            mSegments[4] = new Segment(false, 9, 55);//D
            mSegments[5] = new Segment(true, 8, 54);//E
            mSegments[6] = new Segment(false, 9, 30);//F
            mSegments[7] = new Segment(true, 8, 29);//G

        }

        /// <summary>
        /// Get/Set the Eight segment display to the given pattern. Each of the 8 bits maps to a specific
        /// segment on the display. Where a bit is a "1" the segment is illuminated.
        /// </summary>
        public byte Code
        {
            get { return mCode; }
            set
            {
                //only set the pattern if it has changed. We dont want to overload the system
                //with paint events unnessecarily.
                if (mCode != value)
                {
                    mCode = value;
                    pictureBox1.Invalidate();
                }
            }//set
        }//property Code

        /// <summary>
        /// Set the Eight segment display to show a given integer digit (0-9) and optionally
        /// show the "Point" segment.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="point"></param>
        public void SetNumberPattern(uint number, bool point)
        {
            if (number >= mDigits.Length)
                return;

            byte code = mDigits[number];

            if (point)
                code |= (byte)SegmentFlags.SEG_P;

            this.Code = code;

        }//SetNumberPattern

        /// <summary>
        /// Paint the Eight segment display based on the 8 bit pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int mask = 0x80;
            for (int ii = 0; ii < mSegments.Length; ii++)
            {
                if ((mask & mCode) != 0)
                {
                    if (mSegments[ii] != null)
                    {
                        g.FillPolygon(mBrush, mSegments[ii].Points);
                    }
                    else
                    {
                        g.FillEllipse(mBrush, 35, 53, 8, 8);
                    }
                }
                mask >>= 1;
            }//for ii

        }

    }//class EightSegmentDisplayControl

}
