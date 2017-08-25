namespace ARMSim.Plugins.ButtonTimerPlugin
{
    partial class ButtonTimerDisplay
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblTrackbar = new System.Windows.Forms.Label();
            this.leds1 = new ARMSim.Plugins.ButtonTimerPlugin.LedsDisplay();
            this.twoLineLCDDisplay1 = new ARMSim.Plugins.ButtonTimerPlugin.TwoLineLCDDisplay();
            this.eightSegmentDisplay1 = new ARMSim.Plugins.ButtonTimerPlugin.EightSegmentDisplay();
            this.blackButtons1 = new ARMSim.Plugins.ButtonTimerPlugin.BlackButtonsDisplay();
            this.timCount1 = new ARMSim.Plugins.ButtonTimerPlugin.TimerDisplay();
            this.timControl1 = new ARMSim.Plugins.ButtonTimerPlugin.TimerDisplay();
            this.timValue1 = new ARMSim.Plugins.ButtonTimerPlugin.TimerDisplay();
            this.timCount2 = new ARMSim.Plugins.ButtonTimerPlugin.TimerDisplay();
            this.timControl2 = new ARMSim.Plugins.ButtonTimerPlugin.TimerDisplay();
            this.timValue2 = new ARMSim.Plugins.ButtonTimerPlugin.TimerDisplay();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.timCount2);
            this.groupBox1.Controls.Add(this.timControl2);
            this.groupBox1.Controls.Add(this.timValue2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(528, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 118);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Timer2";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "Counter";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Control";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.blackButtons1);
            this.groupBox2.ForeColor = System.Drawing.Color.Black;
            this.groupBox2.Location = new System.Drawing.Point(13, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 118);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Black Buttons";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.timCount1);
            this.groupBox3.Controls.Add(this.timControl1);
            this.groupBox3.Controls.Add(this.timValue1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.ForeColor = System.Drawing.Color.Black;
            this.groupBox3.Location = new System.Drawing.Point(314, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(208, 118);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Timer1";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 23);
            this.label4.TabIndex = 1;
            this.label4.Text = "Counter";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 23);
            this.label5.TabIndex = 1;
            this.label5.Text = "Control";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(31, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 23);
            this.label6.TabIndex = 1;
            this.label6.Text = "Value";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.eightSegmentDisplay1);
            this.groupBox5.ForeColor = System.Drawing.Color.Black;
            this.groupBox5.Location = new System.Drawing.Point(737, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(128, 118);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Eight Segment Display";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.leds1);
            this.groupBox4.ForeColor = System.Drawing.Color.Black;
            this.groupBox4.Location = new System.Drawing.Point(161, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(147, 118);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "LEDs";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.trackBar1.AutoSize = false;
            this.trackBar1.LargeChange = 16;
            this.trackBar1.Location = new System.Drawing.Point(499, 162);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(366, 45);
            this.trackBar1.TabIndex = 9;
            this.trackBar1.TickFrequency = 16;
            // 
            // lblTrackbar
            // 
            this.lblTrackbar.AutoSize = true;
            this.lblTrackbar.Location = new System.Drawing.Point(513, 136);
            this.lblTrackbar.Name = "lblTrackbar";
            this.lblTrackbar.Size = new System.Drawing.Size(35, 13);
            this.lblTrackbar.TabIndex = 10;
            this.lblTrackbar.Text = "label7";
            // 
            // leds1
            // 
            this.leds1.LeftLed = false;
            this.leds1.Location = new System.Drawing.Point(12, 30);
            this.leds1.Name = "leds1";
            this.leds1.RightLed = false;
            this.leds1.Size = new System.Drawing.Size(123, 62);
            this.leds1.TabIndex = 0;
            // 
            // twoLineLCDDisplay1
            // 
            this.twoLineLCDDisplay1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.twoLineLCDDisplay1.BackColor = System.Drawing.Color.DarkGreen;
            this.twoLineLCDDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.twoLineLCDDisplay1.CursorBlinking = false;
            this.twoLineLCDDisplay1.CursorDirectionRight = false;
            this.twoLineLCDDisplay1.CursorEnabled = false;
            this.twoLineLCDDisplay1.DisplayEnabled = false;
            this.twoLineLCDDisplay1.DisplayShifted = false;
            this.twoLineLCDDisplay1.Location = new System.Drawing.Point(13, 127);
            this.twoLineLCDDisplay1.Name = "twoLineLCDDisplay1";
            this.twoLineLCDDisplay1.Size = new System.Drawing.Size(480, 80);
            this.twoLineLCDDisplay1.TabIndex = 7;
            // 
            // eightSegmentDisplay1
            // 
            this.eightSegmentDisplay1.Code = ((byte)(0));
            this.eightSegmentDisplay1.Location = new System.Drawing.Point(42, 30);
            this.eightSegmentDisplay1.Name = "eightSegmentDisplay1";
            this.eightSegmentDisplay1.Size = new System.Drawing.Size(42, 62);
            this.eightSegmentDisplay1.TabIndex = 0;
            // 
            // blackButtons1
            // 
            this.blackButtons1.Location = new System.Drawing.Point(11, 32);
            this.blackButtons1.Name = "blackButtons1";
            this.blackButtons1.Size = new System.Drawing.Size(118, 58);
            this.blackButtons1.TabIndex = 2;
            // 
            // timCount1
            // 
            this.timCount1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.timCount1.Location = new System.Drawing.Point(115, 81);
            this.timCount1.Name = "timCount1";
            this.timCount1.Size = new System.Drawing.Size(76, 25);
            this.timCount1.TabIndex = 2;
            this.timCount1.Value = ((ushort)(0));
            // 
            // timControl1
            // 
            this.timControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.timControl1.Location = new System.Drawing.Point(115, 51);
            this.timControl1.Name = "timControl1";
            this.timControl1.Size = new System.Drawing.Size(76, 25);
            this.timControl1.TabIndex = 2;
            this.timControl1.Value = ((ushort)(0));
            // 
            // timValue1
            // 
            this.timValue1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.timValue1.Location = new System.Drawing.Point(115, 21);
            this.timValue1.Name = "timValue1";
            this.timValue1.Size = new System.Drawing.Size(76, 25);
            this.timValue1.TabIndex = 2;
            this.timValue1.Value = ((ushort)(0));
            // 
            // timCount2
            // 
            this.timCount2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.timCount2.Location = new System.Drawing.Point(115, 81);
            this.timCount2.Name = "timCount2";
            this.timCount2.Size = new System.Drawing.Size(76, 25);
            this.timCount2.TabIndex = 2;
            this.timCount2.Value = ((ushort)(0));
            // 
            // timControl2
            // 
            this.timControl2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.timControl2.Location = new System.Drawing.Point(115, 51);
            this.timControl2.Name = "timControl2";
            this.timControl2.Size = new System.Drawing.Size(76, 25);
            this.timControl2.TabIndex = 2;
            this.timControl2.Value = ((ushort)(0));
            // 
            // timValue2
            // 
            this.timValue2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.timValue2.Location = new System.Drawing.Point(115, 21);
            this.timValue2.Name = "timValue2";
            this.timValue2.Size = new System.Drawing.Size(76, 25);
            this.timValue2.TabIndex = 2;
            this.timValue2.Value = ((ushort)(0));
            // 
            // ButtonTimerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.Controls.Add(this.lblTrackbar);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.twoLineLCDDisplay1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "ButtonTimerDisplay";
            this.Size = new System.Drawing.Size(877, 213);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private BlackButtonsDisplay blackButtons1;
        private System.Windows.Forms.GroupBox groupBox2;
        private TimerDisplay timCount2;
        private TimerDisplay timControl2;
        private TimerDisplay timValue2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private TimerDisplay timCount1;
        private TimerDisplay timControl1;
        private TimerDisplay timValue1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private EightSegmentDisplay eightSegmentDisplay1;
        private TwoLineLCDDisplay twoLineLCDDisplay1;
        private System.Windows.Forms.GroupBox groupBox4;
        private LedsDisplay leds1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lblTrackbar;
    }
}
