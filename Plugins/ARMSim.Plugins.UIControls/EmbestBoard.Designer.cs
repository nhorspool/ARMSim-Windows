namespace ARMSim.Plugins.UIControls
{
    partial class EmbestBoard
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
            this.blueButtons1 = new ARMSim.Plugins.UIControls.BlueButtons();
            this.leds1 = new ARMSim.Plugins.UIControls.Leds();
            this.blackButtons1 = new ARMSim.Plugins.UIControls.BlackButtons();
            this.eightSegmentDisplayControl1 = new ARMSim.Plugins.UIControls.EightSegmentDisplay();
            this.lcd1 = new ARMSim.Plugins.UIControls.Lcd();
            this.SuspendLayout();
            // 
            // blueButtons1
            // 
            this.blueButtons1.Location = new System.Drawing.Point(342, 3);
            this.blueButtons1.Name = "blueButtons1";
            this.blueButtons1.Size = new System.Drawing.Size(208, 192);
            this.blueButtons1.TabIndex = 12;
            // 
            // leds1
            // 
            this.leds1.LeftLed = false;
            this.leds1.Location = new System.Drawing.Point(52, 4);
            this.leds1.Name = "leds1";
            this.leds1.RightLed = false;
            this.leds1.Size = new System.Drawing.Size(143, 67);
            this.leds1.TabIndex = 10;
            // 
            // blackButtons1
            // 
            this.blackButtons1.Location = new System.Drawing.Point(200, 4);
            this.blackButtons1.Name = "blackButtons1";
            this.blackButtons1.Size = new System.Drawing.Size(138, 62);
            this.blackButtons1.TabIndex = 7;
            // 
            // eightSegmentDisplayControl1
            // 
            this.eightSegmentDisplayControl1.Code = ((byte)(0));
            this.eightSegmentDisplayControl1.Location = new System.Drawing.Point(3, 4);
            this.eightSegmentDisplayControl1.Name = "eightSegmentDisplayControl1";
            this.eightSegmentDisplayControl1.Size = new System.Drawing.Size(49, 67);
            this.eightSegmentDisplayControl1.TabIndex = 6;
            // 
            // lcd1
            // 
            this.lcd1.BackColor = System.Drawing.Color.Green;
            this.lcd1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcd1.Location = new System.Drawing.Point(557, 4);
            this.lcd1.Margin = new System.Windows.Forms.Padding(4);
            this.lcd1.Name = "lcd1";
            this.lcd1.Size = new System.Drawing.Size(285, 200);
            this.lcd1.TabIndex = 13;
            // 
            // EmbestBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lcd1);
            this.Controls.Add(this.blueButtons1);
            this.Controls.Add(this.leds1);
            this.Controls.Add(this.blackButtons1);
            this.Controls.Add(this.eightSegmentDisplayControl1);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "EmbestBoard";
            this.Size = new System.Drawing.Size(881, 213);
            this.ResumeLayout(false);

        }

        #endregion

        private Leds leds1;
        private BlackButtons blackButtons1;
        private EightSegmentDisplay eightSegmentDisplayControl1;
        private BlueButtons blueButtons1;
        private Lcd lcd1;
    }
}
