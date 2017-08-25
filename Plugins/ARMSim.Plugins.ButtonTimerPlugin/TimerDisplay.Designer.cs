namespace ARMSim.Plugins.ButtonTimerPlugin
{
    partial class TimerDisplay
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
            this.lblTimerValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTimerValue
            // 
            this.lblTimerValue.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimerValue.Location = new System.Drawing.Point(3, 0);
            this.lblTimerValue.Name = "lblTimerValue";
            this.lblTimerValue.Size = new System.Drawing.Size(76, 23);
            this.lblTimerValue.TabIndex = 0;
            this.lblTimerValue.Text = "ABCD";
            this.lblTimerValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.lblTimerValue);
            this.Name = "TimerDisplay";
            this.Size = new System.Drawing.Size(76, 28);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTimerValue;

    }
}
