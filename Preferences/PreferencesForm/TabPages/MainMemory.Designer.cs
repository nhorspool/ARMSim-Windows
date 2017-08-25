namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    partial class MainMemory
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
            this.cbMisalignedStop = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudHeapArea = new System.Windows.Forms.NumericUpDown();
            this.nudStackArea = new System.Windows.Forms.NumericUpDown();
            this.nudStartAddress = new System.Windows.Forms.NumericUpDown();
            this.hexFillPattern = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbStackGrowsUp = new System.Windows.Forms.RadioButton();
            this.rbStackGrowsDown = new System.Windows.Forms.RadioButton();
            this.cbProtectTextArea = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeapArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStackArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartAddress)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbMisalignedStop
            // 
            this.cbMisalignedStop.AutoSize = true;
            this.cbMisalignedStop.Checked = true;
            this.cbMisalignedStop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMisalignedStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMisalignedStop.Location = new System.Drawing.Point(27, 258);
            this.cbMisalignedStop.Margin = new System.Windows.Forms.Padding(2);
            this.cbMisalignedStop.Name = "cbMisalignedStop";
            this.cbMisalignedStop.Size = new System.Drawing.Size(455, 28);
            this.cbMisalignedStop.TabIndex = 14;
            this.cbMisalignedStop.Text = "Stop program on misaligned memory access?";
            this.cbMisalignedStop.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(21, 90);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(190, 24);
            this.label8.TabIndex = 13;
            this.label8.Text = "Memory Fill Pattern";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 54);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 24);
            this.label3.TabIndex = 11;
            this.label3.Text = "Heap Area(KB)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 24);
            this.label2.TabIndex = 12;
            this.label2.Text = "Size(KB)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(318, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Starting Address of Main Memory";
            // 
            // nudHeapArea
            // 
            this.nudHeapArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudHeapArea.Location = new System.Drawing.Point(514, 53);
            this.nudHeapArea.Margin = new System.Windows.Forms.Padding(2);
            this.nudHeapArea.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudHeapArea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeapArea.Name = "nudHeapArea";
            this.nudHeapArea.ReadOnly = true;
            this.nudHeapArea.Size = new System.Drawing.Size(111, 26);
            this.nudHeapArea.TabIndex = 7;
            this.nudHeapArea.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // nudStackArea
            // 
            this.nudStackArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudStackArea.Location = new System.Drawing.Point(117, 35);
            this.nudStackArea.Margin = new System.Windows.Forms.Padding(2);
            this.nudStackArea.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudStackArea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudStackArea.Name = "nudStackArea";
            this.nudStackArea.ReadOnly = true;
            this.nudStackArea.Size = new System.Drawing.Size(87, 29);
            this.nudStackArea.TabIndex = 8;
            this.nudStackArea.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // nudStartAddress
            // 
            this.nudStartAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudStartAddress.Hexadecimal = true;
            this.nudStartAddress.Increment = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudStartAddress.Location = new System.Drawing.Point(514, 19);
            this.nudStartAddress.Margin = new System.Windows.Forms.Padding(2);
            this.nudStartAddress.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.nudStartAddress.Name = "nudStartAddress";
            this.nudStartAddress.ReadOnly = true;
            this.nudStartAddress.Size = new System.Drawing.Size(111, 26);
            this.nudStartAddress.TabIndex = 9;
            this.nudStartAddress.Value = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            // 
            // hexFillPattern
            // 
            this.hexFillPattern.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexFillPattern.Location = new System.Drawing.Point(514, 88);
            this.hexFillPattern.Name = "hexFillPattern";
            this.hexFillPattern.Size = new System.Drawing.Size(100, 26);
            this.hexFillPattern.TabIndex = 15;
            this.hexFillPattern.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.hexFillPattern_KeyPress);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbStackGrowsUp);
            this.groupBox1.Controls.Add(this.rbStackGrowsDown);
            this.groupBox1.Controls.Add(this.nudStackArea);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(25, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(600, 92);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stack Area";
            // 
            // rbStackGrowsUp
            // 
            this.rbStackGrowsUp.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbStackGrowsUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbStackGrowsUp.Location = new System.Drawing.Point(418, 35);
            this.rbStackGrowsUp.Name = "rbStackGrowsUp";
            this.rbStackGrowsUp.Size = new System.Drawing.Size(171, 34);
            this.rbStackGrowsUp.TabIndex = 14;
            this.rbStackGrowsUp.TabStop = true;
            this.rbStackGrowsUp.Text = "Stack Grows Up";
            this.rbStackGrowsUp.UseVisualStyleBackColor = true;
            // 
            // rbStackGrowsDown
            // 
            this.rbStackGrowsDown.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbStackGrowsDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbStackGrowsDown.Location = new System.Drawing.Point(230, 35);
            this.rbStackGrowsDown.Name = "rbStackGrowsDown";
            this.rbStackGrowsDown.Size = new System.Drawing.Size(171, 34);
            this.rbStackGrowsDown.TabIndex = 13;
            this.rbStackGrowsDown.TabStop = true;
            this.rbStackGrowsDown.Text = "Stack Grows Down";
            this.rbStackGrowsDown.UseVisualStyleBackColor = true;
            // 
            // cbProtectTextArea
            // 
            this.cbProtectTextArea.AutoSize = true;
            this.cbProtectTextArea.Checked = true;
            this.cbProtectTextArea.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbProtectTextArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbProtectTextArea.Location = new System.Drawing.Point(27, 226);
            this.cbProtectTextArea.Margin = new System.Windows.Forms.Padding(2);
            this.cbProtectTextArea.Name = "cbProtectTextArea";
            this.cbProtectTextArea.Size = new System.Drawing.Size(202, 28);
            this.cbProtectTextArea.TabIndex = 14;
            this.cbProtectTextArea.Text = "Protect Text Area?";
            this.cbProtectTextArea.UseVisualStyleBackColor = true;
            // 
            // MainMemory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.hexFillPattern);
            this.Controls.Add(this.cbProtectTextArea);
            this.Controls.Add(this.cbMisalignedStop);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudHeapArea);
            this.Controls.Add(this.nudStartAddress);
            this.Name = "MainMemory";
            this.Size = new System.Drawing.Size(651, 302);
            this.Load += new System.EventHandler(this.MainMemory_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudHeapArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStackArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartAddress)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbMisalignedStop;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudHeapArea;
        private System.Windows.Forms.NumericUpDown nudStackArea;
        private System.Windows.Forms.NumericUpDown nudStartAddress;
        private System.Windows.Forms.TextBox hexFillPattern;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbStackGrowsUp;
        private System.Windows.Forms.RadioButton rbStackGrowsDown;
        private System.Windows.Forms.CheckBox cbProtectTextArea;
    }
}
