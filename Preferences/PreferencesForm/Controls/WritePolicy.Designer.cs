namespace ARMSim.Preferences.PreferencesForm.Controls
{
    partial class WritePolicy
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
            this.rbWriteThrough = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbWriteBack = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbWriteThrough
            // 
            this.rbWriteThrough.AutoSize = true;
            this.rbWriteThrough.Location = new System.Drawing.Point(14, 25);
            this.rbWriteThrough.Margin = new System.Windows.Forms.Padding(2);
            this.rbWriteThrough.Name = "rbWriteThrough";
            this.rbWriteThrough.Size = new System.Drawing.Size(93, 17);
            this.rbWriteThrough.TabIndex = 0;
            this.rbWriteThrough.TabStop = true;
            this.rbWriteThrough.Text = "Write Through";
            this.rbWriteThrough.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbWriteBack);
            this.groupBox1.Controls.Add(this.rbWriteThrough);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(133, 92);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Write Policy";
            // 
            // rbWriteBack
            // 
            this.rbWriteBack.AutoSize = true;
            this.rbWriteBack.Location = new System.Drawing.Point(14, 47);
            this.rbWriteBack.Margin = new System.Windows.Forms.Padding(2);
            this.rbWriteBack.Name = "rbWriteBack";
            this.rbWriteBack.Size = new System.Drawing.Size(78, 17);
            this.rbWriteBack.TabIndex = 0;
            this.rbWriteBack.TabStop = true;
            this.rbWriteBack.Text = "Write Back";
            this.rbWriteBack.UseVisualStyleBackColor = true;
            // 
            // WritePolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "WritePolicy";
            this.Size = new System.Drawing.Size(133, 92);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbWriteThrough;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbWriteBack;
    }
}
