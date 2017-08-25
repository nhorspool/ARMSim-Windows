namespace ARMSim.GUI
{
    partial class CacheStatistics
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lbCRdMissrate = new System.Windows.Forms.Label();
            this.lbCRdHitrate = new System.Windows.Forms.Label();
            this.lbCRdMisses = new System.Windows.Forms.Label();
            this.lbCRdHits = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbWrMissrate = new System.Windows.Forms.Label();
            this.lbWrHitrate = new System.Windows.Forms.Label();
            this.lbWrMisses = new System.Windows.Forms.Label();
            this.lbWrHits = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbRdMissrate = new System.Windows.Forms.Label();
            this.lbRdHitrate = new System.Windows.Forms.Label();
            this.lbRdMisses = new System.Windows.Forms.Label();
            this.lbRdHits = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(411, 308);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 19);
            this.button2.TabIndex = 7;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 308);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 19);
            this.button1.TabIndex = 6;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Location = new System.Drawing.Point(11, 159);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(232, 144);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Instruction Cache";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lbCRdMissrate);
            this.groupBox5.Controls.Add(this.lbCRdHitrate);
            this.groupBox5.Controls.Add(this.lbCRdMisses);
            this.groupBox5.Controls.Add(this.lbCRdHits);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Location = new System.Drawing.Point(4, 17);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(219, 119);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Read";
            // 
            // lbCRdMissrate
            // 
            this.lbCRdMissrate.AutoSize = true;
            this.lbCRdMissrate.Location = new System.Drawing.Point(119, 90);
            this.lbCRdMissrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCRdMissrate.Name = "lbCRdMissrate";
            this.lbCRdMissrate.Size = new System.Drawing.Size(0, 13);
            this.lbCRdMissrate.TabIndex = 7;
            // 
            // lbCRdHitrate
            // 
            this.lbCRdHitrate.AutoSize = true;
            this.lbCRdHitrate.Location = new System.Drawing.Point(119, 67);
            this.lbCRdHitrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCRdHitrate.Name = "lbCRdHitrate";
            this.lbCRdHitrate.Size = new System.Drawing.Size(0, 13);
            this.lbCRdHitrate.TabIndex = 6;
            // 
            // lbCRdMisses
            // 
            this.lbCRdMisses.AutoSize = true;
            this.lbCRdMisses.Location = new System.Drawing.Point(119, 45);
            this.lbCRdMisses.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCRdMisses.Name = "lbCRdMisses";
            this.lbCRdMisses.Size = new System.Drawing.Size(0, 13);
            this.lbCRdMisses.TabIndex = 9;
            // 
            // lbCRdHits
            // 
            this.lbCRdHits.AutoSize = true;
            this.lbCRdHits.Location = new System.Drawing.Point(119, 22);
            this.lbCRdHits.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCRdHits.Name = "lbCRdHits";
            this.lbCRdHits.Size = new System.Drawing.Size(0, 13);
            this.lbCRdHits.TabIndex = 8;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(14, 90);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Read Miss Rate";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 67);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Read Hit Rate";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 45);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(68, 13);
            this.label15.TabIndex = 5;
            this.label15.Text = "Read Misses";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(14, 22);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(54, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Read Hits";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(11, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(456, 144);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Cache";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbWrMissrate);
            this.groupBox3.Controls.Add(this.lbWrHitrate);
            this.groupBox3.Controls.Add(this.lbWrMisses);
            this.groupBox3.Controls.Add(this.lbWrHits);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(228, 17);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(219, 119);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Write";
            // 
            // lbWrMissrate
            // 
            this.lbWrMissrate.AutoSize = true;
            this.lbWrMissrate.Location = new System.Drawing.Point(118, 90);
            this.lbWrMissrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbWrMissrate.Name = "lbWrMissrate";
            this.lbWrMissrate.Size = new System.Drawing.Size(0, 13);
            this.lbWrMissrate.TabIndex = 7;
            // 
            // lbWrHitrate
            // 
            this.lbWrHitrate.AutoSize = true;
            this.lbWrHitrate.Location = new System.Drawing.Point(118, 67);
            this.lbWrHitrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbWrHitrate.Name = "lbWrHitrate";
            this.lbWrHitrate.Size = new System.Drawing.Size(0, 13);
            this.lbWrHitrate.TabIndex = 6;
            // 
            // lbWrMisses
            // 
            this.lbWrMisses.AutoSize = true;
            this.lbWrMisses.Location = new System.Drawing.Point(118, 45);
            this.lbWrMisses.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbWrMisses.Name = "lbWrMisses";
            this.lbWrMisses.Size = new System.Drawing.Size(0, 13);
            this.lbWrMisses.TabIndex = 9;
            // 
            // lbWrHits
            // 
            this.lbWrHits.AutoSize = true;
            this.lbWrHits.Location = new System.Drawing.Point(118, 22);
            this.lbWrHits.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbWrHits.Name = "lbWrHits";
            this.lbWrHits.Size = new System.Drawing.Size(0, 13);
            this.lbWrHits.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 90);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Write Miss Rate";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 67);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Write Hit Rate";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 45);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Write Misses";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 22);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Write Hits";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbRdMissrate);
            this.groupBox2.Controls.Add(this.lbRdHitrate);
            this.groupBox2.Controls.Add(this.lbRdMisses);
            this.groupBox2.Controls.Add(this.lbRdHits);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(4, 17);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(219, 119);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Read";
            // 
            // lbRdMissrate
            // 
            this.lbRdMissrate.AutoSize = true;
            this.lbRdMissrate.Location = new System.Drawing.Point(119, 90);
            this.lbRdMissrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRdMissrate.Name = "lbRdMissrate";
            this.lbRdMissrate.Size = new System.Drawing.Size(0, 13);
            this.lbRdMissrate.TabIndex = 1;
            // 
            // lbRdHitrate
            // 
            this.lbRdHitrate.AutoSize = true;
            this.lbRdHitrate.Location = new System.Drawing.Point(119, 67);
            this.lbRdHitrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRdHitrate.Name = "lbRdHitrate";
            this.lbRdHitrate.Size = new System.Drawing.Size(0, 13);
            this.lbRdHitrate.TabIndex = 1;
            // 
            // lbRdMisses
            // 
            this.lbRdMisses.AutoSize = true;
            this.lbRdMisses.Location = new System.Drawing.Point(119, 45);
            this.lbRdMisses.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRdMisses.Name = "lbRdMisses";
            this.lbRdMisses.Size = new System.Drawing.Size(0, 13);
            this.lbRdMisses.TabIndex = 1;
            // 
            // lbRdHits
            // 
            this.lbRdHits.AutoSize = true;
            this.lbRdHits.Location = new System.Drawing.Point(119, 22);
            this.lbRdHits.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRdHits.Name = "lbRdHits";
            this.lbRdHits.Size = new System.Drawing.Size(0, 13);
            this.lbRdHits.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 90);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Read Miss Rate";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Read Hit Rate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Read Misses";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Read Hits";
            // 
            // CacheStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 339);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Text = this.Name = "CacheStatistics";
            this.Load += new System.EventHandler(this.CacheStatistics_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lbCRdMissrate;
        private System.Windows.Forms.Label lbCRdHitrate;
        private System.Windows.Forms.Label lbCRdMisses;
        private System.Windows.Forms.Label lbCRdHits;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbWrMissrate;
        private System.Windows.Forms.Label lbWrHitrate;
        private System.Windows.Forms.Label lbWrMisses;
        private System.Windows.Forms.Label lbWrHits;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbRdMissrate;
        private System.Windows.Forms.Label lbRdHitrate;
        private System.Windows.Forms.Label lbRdMisses;
        private System.Windows.Forms.Label lbRdHits;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}