namespace ARMSim.GUI.Views
{
    partial class AddWatch
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbHexDecimal = new System.Windows.Forms.RadioButton();
            this.rbDecimal = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbUnsigned = new System.Windows.Forms.RadioButton();
            this.rbSigned = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.rbCharacter = new System.Windows.Forms.RadioButton();
            this.rbWord = new System.Windows.Forms.RadioButton();
            this.rbHalfword = new System.Windows.Forms.RadioButton();
            this.rbByte = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbLabels = new System.Windows.Forms.ListBox();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.HexAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(288, 210);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 19);
            this.button2.TabIndex = 17;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(361, 210);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 19);
            this.button1.TabIndex = 16;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbHexDecimal);
            this.groupBox3.Controls.Add(this.rbDecimal);
            this.groupBox3.Location = new System.Drawing.Point(315, 107);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(102, 77);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Base";
            // 
            // rbHexDecimal
            // 
            this.rbHexDecimal.AutoSize = true;
            this.rbHexDecimal.Checked = true;
            this.rbHexDecimal.Location = new System.Drawing.Point(16, 44);
            this.rbHexDecimal.Margin = new System.Windows.Forms.Padding(2);
            this.rbHexDecimal.Name = "rbHexDecimal";
            this.rbHexDecimal.Size = new System.Drawing.Size(86, 17);
            this.rbHexDecimal.TabIndex = 0;
            this.rbHexDecimal.TabStop = true;
            this.rbHexDecimal.Text = "Hexadecimal";
            this.rbHexDecimal.UseVisualStyleBackColor = true;
            // 
            // rbDecimal
            // 
            this.rbDecimal.AutoSize = true;
            this.rbDecimal.Location = new System.Drawing.Point(16, 22);
            this.rbDecimal.Margin = new System.Windows.Forms.Padding(2);
            this.rbDecimal.Name = "rbDecimal";
            this.rbDecimal.Size = new System.Drawing.Size(63, 17);
            this.rbDecimal.TabIndex = 0;
            this.rbDecimal.Text = "Decimal";
            this.rbDecimal.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbUnsigned);
            this.groupBox2.Controls.Add(this.rbSigned);
            this.groupBox2.Location = new System.Drawing.Point(315, 25);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(102, 77);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Integer Format";
            // 
            // rbUnsigned
            // 
            this.rbUnsigned.AutoSize = true;
            this.rbUnsigned.Checked = true;
            this.rbUnsigned.Location = new System.Drawing.Point(16, 50);
            this.rbUnsigned.Margin = new System.Windows.Forms.Padding(2);
            this.rbUnsigned.Name = "rbUnsigned";
            this.rbUnsigned.Size = new System.Drawing.Size(70, 17);
            this.rbUnsigned.TabIndex = 0;
            this.rbUnsigned.TabStop = true;
            this.rbUnsigned.Text = "Unsigned";
            this.rbUnsigned.UseVisualStyleBackColor = true;
            this.rbUnsigned.CheckedChanged += new System.EventHandler(this.rbIntegerFormat_CheckedChanged);
            // 
            // rbSigned
            // 
            this.rbSigned.AutoSize = true;
            this.rbSigned.Location = new System.Drawing.Point(16, 26);
            this.rbSigned.Margin = new System.Windows.Forms.Padding(2);
            this.rbSigned.Name = "rbSigned";
            this.rbSigned.Size = new System.Drawing.Size(58, 17);
            this.rbSigned.TabIndex = 0;
            this.rbSigned.Text = "Signed";
            this.rbSigned.UseVisualStyleBackColor = true;
            this.rbSigned.CheckedChanged += new System.EventHandler(this.rbIntegerFormat_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbString);
            this.groupBox1.Controls.Add(this.rbCharacter);
            this.groupBox1.Controls.Add(this.rbWord);
            this.groupBox1.Controls.Add(this.rbHalfword);
            this.groupBox1.Controls.Add(this.rbByte);
            this.groupBox1.Location = new System.Drawing.Point(220, 25);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(90, 159);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display As";
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Location = new System.Drawing.Point(12, 124);
            this.rbString.Margin = new System.Windows.Forms.Padding(2);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(52, 17);
            this.rbString.TabIndex = 0;
            this.rbString.Text = "String";
            this.rbString.UseVisualStyleBackColor = true;
            this.rbString.CheckedChanged += new System.EventHandler(this.rbDisplayAs_CheckedChanged);
            // 
            // rbCharacter
            // 
            this.rbCharacter.AutoSize = true;
            this.rbCharacter.Location = new System.Drawing.Point(12, 99);
            this.rbCharacter.Margin = new System.Windows.Forms.Padding(2);
            this.rbCharacter.Name = "rbCharacter";
            this.rbCharacter.Size = new System.Drawing.Size(71, 17);
            this.rbCharacter.TabIndex = 0;
            this.rbCharacter.Text = "Character";
            this.rbCharacter.UseVisualStyleBackColor = true;
            this.rbCharacter.CheckedChanged += new System.EventHandler(this.rbDisplayAs_CheckedChanged);
            // 
            // rbWord
            // 
            this.rbWord.AutoSize = true;
            this.rbWord.Checked = true;
            this.rbWord.Location = new System.Drawing.Point(12, 75);
            this.rbWord.Margin = new System.Windows.Forms.Padding(2);
            this.rbWord.Name = "rbWord";
            this.rbWord.Size = new System.Drawing.Size(51, 17);
            this.rbWord.TabIndex = 0;
            this.rbWord.TabStop = true;
            this.rbWord.Text = "Word";
            this.rbWord.UseVisualStyleBackColor = true;
            this.rbWord.CheckedChanged += new System.EventHandler(this.rbDisplayAs_CheckedChanged);
            // 
            // rbHalfword
            // 
            this.rbHalfword.AutoSize = true;
            this.rbHalfword.Location = new System.Drawing.Point(12, 50);
            this.rbHalfword.Margin = new System.Windows.Forms.Padding(2);
            this.rbHalfword.Name = "rbHalfword";
            this.rbHalfword.Size = new System.Drawing.Size(67, 17);
            this.rbHalfword.TabIndex = 0;
            this.rbHalfword.Text = "Halfword";
            this.rbHalfword.UseVisualStyleBackColor = true;
            this.rbHalfword.CheckedChanged += new System.EventHandler(this.rbDisplayAs_CheckedChanged);
            // 
            // rbByte
            // 
            this.rbByte.AutoSize = true;
            this.rbByte.Location = new System.Drawing.Point(12, 26);
            this.rbByte.Margin = new System.Windows.Forms.Padding(2);
            this.rbByte.Name = "rbByte";
            this.rbByte.Size = new System.Drawing.Size(46, 17);
            this.rbByte.TabIndex = 0;
            this.rbByte.Text = "Byte";
            this.rbByte.UseVisualStyleBackColor = true;
            this.rbByte.CheckedChanged += new System.EventHandler(this.rbDisplayAs_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Labels";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Files";
            // 
            // lbLabels
            // 
            this.lbLabels.FormattingEnabled = true;
            this.lbLabels.Location = new System.Drawing.Point(119, 25);
            this.lbLabels.Margin = new System.Windows.Forms.Padding(2);
            this.lbLabels.Name = "lbLabels";
            this.lbLabels.Size = new System.Drawing.Size(91, 160);
            this.lbLabels.Sorted = true;
            this.lbLabels.TabIndex = 10;
            this.lbLabels.SelectedIndexChanged += new System.EventHandler(this.lbLabels_SelectedIndexChanged);
            // 
            // lbFiles
            // 
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.Location = new System.Drawing.Point(15, 25);
            this.lbFiles.Margin = new System.Windows.Forms.Padding(2);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(91, 160);
            this.lbFiles.Sorted = true;
            this.lbFiles.TabIndex = 9;
            this.lbFiles.SelectedIndexChanged += new System.EventHandler(this.lbFiles_SelectedIndexChanged);
            // 
            // HexAddress
            // 
            this.HexAddress.Location = new System.Drawing.Point(106, 213);
            this.HexAddress.Name = "HexAddress";
            this.HexAddress.Size = new System.Drawing.Size(100, 20);
            this.HexAddress.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Hex Address:";
            // 
            // AddWatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 250);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.HexAddress);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbLabels);
            this.Controls.Add(this.lbFiles);
            this.Name = "AddWatch";
            this.Text = "AddWatch";
            this.Load += new System.EventHandler(this.AddWatch_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbHexDecimal;
        private System.Windows.Forms.RadioButton rbDecimal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbUnsigned;
        private System.Windows.Forms.RadioButton rbSigned;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbString;
        private System.Windows.Forms.RadioButton rbCharacter;
        private System.Windows.Forms.RadioButton rbWord;
        private System.Windows.Forms.RadioButton rbHalfword;
        private System.Windows.Forms.RadioButton rbByte;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbLabels;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.TextBox HexAddress;
        private System.Windows.Forms.Label label3;
    }
}