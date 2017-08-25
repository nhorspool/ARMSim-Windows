namespace ARMSim.Plugins.ButtonTimerPlugin
{
    partial class TwoLineLCDDisplay
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TwoLineLCDDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Lime;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Name = "TwoLineLCDDisplay";
            this.Size = new System.Drawing.Size(480, 80);
            this.Load += new System.EventHandler(this.TwoLineLCDDisplay_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TwoLineLCDDisplay_Paint);
            this.SizeChanged += new System.EventHandler(this.TwoLineLCDDisplay_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
    }
}
