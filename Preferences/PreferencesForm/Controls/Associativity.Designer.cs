namespace ARMSim.Preferences.PreferencesForm.Controls
{
    partial class Associativity
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
            this.rbDirectMapped = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.po2BlocksPerSet = new ARMSim.Preferences.PreferencesForm.Controls.PowerOf2();
            this.label1 = new System.Windows.Forms.Label();
            this.rbSetAssociative = new System.Windows.Forms.RadioButton();
            this.rbFullyAssociative = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.po2BlocksPerSet)).BeginInit();
            this.SuspendLayout();
            // 
            // rbDirectMapped
            // 
            this.rbDirectMapped.AutoSize = true;
            this.rbDirectMapped.Location = new System.Drawing.Point(22, 65);
            this.rbDirectMapped.Margin = new System.Windows.Forms.Padding(2);
            this.rbDirectMapped.Name = "rbDirectMapped";
            this.rbDirectMapped.Size = new System.Drawing.Size(95, 17);
            this.rbDirectMapped.TabIndex = 0;
            this.rbDirectMapped.TabStop = true;
            this.rbDirectMapped.Text = "Direct Mapped";
            this.rbDirectMapped.UseVisualStyleBackColor = true;
            this.rbDirectMapped.CheckedChanged += new System.EventHandler(this.rbDirectMapped_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.po2BlocksPerSet);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rbDirectMapped);
            this.groupBox1.Controls.Add(this.rbSetAssociative);
            this.groupBox1.Controls.Add(this.rbFullyAssociative);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(243, 92);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Associativity";
            // 
            // po2BlocksPerSet
            // 
            this.po2BlocksPerSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.po2BlocksPerSet.Location = new System.Drawing.Point(161, 43);
            this.po2BlocksPerSet.Margin = new System.Windows.Forms.Padding(6);
            this.po2BlocksPerSet.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.po2BlocksPerSet.Name = "po2BlocksPerSet";
            this.po2BlocksPerSet.ReadOnly = true;
            this.po2BlocksPerSet.Size = new System.Drawing.Size(59, 32);
            this.po2BlocksPerSet.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(158, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Blocks Per Set";
            // 
            // rbSetAssociative
            // 
            this.rbSetAssociative.AutoSize = true;
            this.rbSetAssociative.Location = new System.Drawing.Point(22, 43);
            this.rbSetAssociative.Margin = new System.Windows.Forms.Padding(2);
            this.rbSetAssociative.Name = "rbSetAssociative";
            this.rbSetAssociative.Size = new System.Drawing.Size(98, 17);
            this.rbSetAssociative.TabIndex = 0;
            this.rbSetAssociative.TabStop = true;
            this.rbSetAssociative.Text = "Set Associative";
            this.rbSetAssociative.UseVisualStyleBackColor = true;
            this.rbSetAssociative.CheckedChanged += new System.EventHandler(this.rbSetAssociative_CheckedChanged);
            // 
            // rbFullyAssociative
            // 
            this.rbFullyAssociative.AutoSize = true;
            this.rbFullyAssociative.Location = new System.Drawing.Point(22, 21);
            this.rbFullyAssociative.Margin = new System.Windows.Forms.Padding(2);
            this.rbFullyAssociative.Name = "rbFullyAssociative";
            this.rbFullyAssociative.Size = new System.Drawing.Size(134, 17);
            this.rbFullyAssociative.TabIndex = 0;
            this.rbFullyAssociative.TabStop = true;
            this.rbFullyAssociative.Text = "Fully Associative(1 Set)";
            this.rbFullyAssociative.UseVisualStyleBackColor = true;
            this.rbFullyAssociative.CheckedChanged += new System.EventHandler(this.rbFullyAssociative_CheckedChanged);
            // 
            // Associativity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "Associativity";
            this.Size = new System.Drawing.Size(243, 92);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.po2BlocksPerSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbDirectMapped;
        private System.Windows.Forms.GroupBox groupBox1;
        private PowerOf2 po2BlocksPerSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbSetAssociative;
        private System.Windows.Forms.RadioButton rbFullyAssociative;

    }
}
