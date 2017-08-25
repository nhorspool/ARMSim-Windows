namespace ARMSim.Preferences.PreferencesForm.Controls
{
    partial class CacheSize
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
            this.nudBlockSize = new ARMSim.Preferences.PreferencesForm.Controls.PowerOf2();
            this.nudCacheSize = new ARMSim.Preferences.PreferencesForm.Controls.PowerOf2();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbNumBlocks = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlockSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCacheSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nudBlockSize
            // 
            this.nudBlockSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBlockSize.Location = new System.Drawing.Point(107, 42);
            this.nudBlockSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudBlockSize.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudBlockSize.Name = "nudBlockSize";
            this.nudBlockSize.ReadOnly = true;
            this.nudBlockSize.Size = new System.Drawing.Size(85, 32);
            this.nudBlockSize.TabIndex = 2;
            // 
            // nudCacheSize
            // 
            this.nudCacheSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCacheSize.Location = new System.Drawing.Point(7, 42);
            this.nudCacheSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudCacheSize.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudCacheSize.Name = "nudCacheSize";
            this.nudCacheSize.ReadOnly = true;
            this.nudCacheSize.Size = new System.Drawing.Size(92, 32);
            this.nudCacheSize.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbNumBlocks);
            this.groupBox1.Controls.Add(this.nudBlockSize);
            this.groupBox1.Controls.Add(this.nudCacheSize);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(294, 92);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cache Size";
            // 
            // tbNumBlocks
            // 
            this.tbNumBlocks.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNumBlocks.Location = new System.Drawing.Point(211, 42);
            this.tbNumBlocks.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbNumBlocks.Name = "tbNumBlocks";
            this.tbNumBlocks.ReadOnly = true;
            this.tbNumBlocks.Size = new System.Drawing.Size(61, 31);
            this.tbNumBlocks.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 24);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Number of Blocks";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Block Size(Bytes)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cache Size(Bytes)";
            // 
            // CacheSize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CacheSize";
            this.Size = new System.Drawing.Size(294, 92);
            ((System.ComponentModel.ISupportInitialize)(this.nudBlockSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCacheSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private PowerOf2 nudBlockSize;
        private PowerOf2 nudCacheSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNumBlocks;
    }
}
