namespace ARMSim.Preferences.PreferencesForm.Controls
{
    partial class AllocatePolicy
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
            this.rbBothAllocate = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbWriteAllocate = new System.Windows.Forms.RadioButton();
            this.rbReadAllocate = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbBothAllocate
            // 
            this.rbBothAllocate.AutoSize = true;
            this.rbBothAllocate.Location = new System.Drawing.Point(14, 65);
            this.rbBothAllocate.Margin = new System.Windows.Forms.Padding(2);
            this.rbBothAllocate.Name = "rbBothAllocate";
            this.rbBothAllocate.Size = new System.Drawing.Size(47, 17);
            this.rbBothAllocate.TabIndex = 0;
            this.rbBothAllocate.TabStop = true;
            this.rbBothAllocate.Text = "Both";
            this.rbBothAllocate.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbBothAllocate);
            this.groupBox1.Controls.Add(this.rbWriteAllocate);
            this.groupBox1.Controls.Add(this.rbReadAllocate);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(117, 92);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Allocate Policy";
            // 
            // rbWriteAllocate
            // 
            this.rbWriteAllocate.AutoSize = true;
            this.rbWriteAllocate.Location = new System.Drawing.Point(14, 43);
            this.rbWriteAllocate.Margin = new System.Windows.Forms.Padding(2);
            this.rbWriteAllocate.Name = "rbWriteAllocate";
            this.rbWriteAllocate.Size = new System.Drawing.Size(91, 17);
            this.rbWriteAllocate.TabIndex = 0;
            this.rbWriteAllocate.TabStop = true;
            this.rbWriteAllocate.Text = "Write Allocate";
            this.rbWriteAllocate.UseVisualStyleBackColor = true;
            // 
            // rbReadAllocate
            // 
            this.rbReadAllocate.AutoSize = true;
            this.rbReadAllocate.Location = new System.Drawing.Point(14, 21);
            this.rbReadAllocate.Margin = new System.Windows.Forms.Padding(2);
            this.rbReadAllocate.Name = "rbReadAllocate";
            this.rbReadAllocate.Size = new System.Drawing.Size(92, 17);
            this.rbReadAllocate.TabIndex = 0;
            this.rbReadAllocate.TabStop = true;
            this.rbReadAllocate.Text = "Read Allocate";
            this.rbReadAllocate.UseVisualStyleBackColor = true;
            // 
            // AllocatePolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "AllocatePolicy";
            this.Size = new System.Drawing.Size(117, 92);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbBothAllocate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbWriteAllocate;
        private System.Windows.Forms.RadioButton rbReadAllocate;
    }
}
