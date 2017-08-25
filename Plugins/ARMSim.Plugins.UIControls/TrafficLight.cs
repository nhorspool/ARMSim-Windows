using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.Plugins.UIControls
{
    public partial class TrafficLight : UserControl
    {
        private enum TrafficLightBulbs
        {
            TOP,
            MIDDLE,
            BOTTOM
        }

        public enum TrafficLightStates
        {
            BLACK = 0,
            RED = 1,
            YELLOW = 2,
            GREEN = 3
        }

        private TrafficLightStates mState = TrafficLightStates.BLACK;

        public TrafficLight()
        {
            InitializeComponent();
        }

        public TrafficLightStates State { get { return mState; } set { mState = value; this.pictureBox1.Invalidate(); } }

        private void DrawLight(Graphics g, TrafficLightBulbs light)
        {
            int pictureWidth = pictureBox1.Width;
            int radius = pictureWidth / 10;
            int centreX = ((pictureWidth / 2) - radius) - (radius / 5);
            int centreY = 0;

            switch (light)
            {
                case TrafficLightBulbs.TOP:
                    centreY = ((this.Height / 3) - radius) - (radius / 3);
                    break;
                case TrafficLightBulbs.MIDDLE:
                    centreY = ((this.Height / 2) - ((radius * 2) / 3));
                    break;
                case TrafficLightBulbs.BOTTOM:
                    centreY = ((this.Height * 2) / 3) - (radius / 6);
                    break;
            }

            Color color = Color.Black;

            if (mState == TrafficLightStates.RED && light == TrafficLightBulbs.TOP)
                color = Color.Red;
            else if (mState == TrafficLightStates.YELLOW && light == TrafficLightBulbs.MIDDLE)
                color = Color.Yellow;
            else if (mState == TrafficLightStates.GREEN && light == TrafficLightBulbs.BOTTOM)
                color = Color.Green;

            g.FillEllipse(new SolidBrush(color), centreX, centreY, radius * 2, radius*2);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            DrawLight(g, TrafficLightBulbs.TOP);
            DrawLight(g, TrafficLightBulbs.MIDDLE);
            DrawLight(g, TrafficLightBulbs.BOTTOM);

        }
    }
}
