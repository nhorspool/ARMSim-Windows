namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    partial class General
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
			this.cbCloseFiles = new System.Windows.Forms.CheckBox();
			this.cbSync = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbStderrAppend = new System.Windows.Forms.RadioButton();
			this.rbStdoutAppend = new System.Windows.Forms.RadioButton();
			this.rbStderrOverwrite = new System.Windows.Forms.RadioButton();
			this.rbStdoutOverwrite = new System.Windows.Forms.RadioButton();
			this.btnBrowseStderr = new System.Windows.Forms.Button();
			this.btnBrowseStdout = new System.Windows.Forms.Button();
			this.btnBrowseStdin = new System.Windows.Forms.Button();
			this.tbStderrFilename = new System.Windows.Forms.TextBox();
			this.tbStdoutFilename = new System.Windows.Forms.TextBox();
			this.tbStdinFilename = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbCloseFiles
			// 
			this.cbCloseFiles.AutoSize = true;
			this.cbCloseFiles.Location = new System.Drawing.Point(14, 41);
			this.cbCloseFiles.Margin = new System.Windows.Forms.Padding(2);
			this.cbCloseFiles.Name = "cbCloseFiles";
			this.cbCloseFiles.Size = new System.Drawing.Size(154, 17);
			this.cbCloseFiles.TabIndex = 3;
			this.cbCloseFiles.Text = "Close files on program exit?";
			this.cbCloseFiles.UseVisualStyleBackColor = true;
			// 
			// cbSync
			// 
			this.cbSync.AutoSize = true;
			this.cbSync.Location = new System.Drawing.Point(14, 12);
			this.cbSync.Margin = new System.Windows.Forms.Padding(2);
			this.cbSync.Name = "cbSync";
			this.cbSync.Size = new System.Drawing.Size(198, 17);
			this.cbSync.TabIndex = 2;
			this.cbSync.Text = "Synchronize cache on program exit?";
			this.cbSync.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbStderrAppend);
			this.groupBox1.Controls.Add(this.rbStdoutAppend);
			this.groupBox1.Controls.Add(this.rbStderrOverwrite);
			this.groupBox1.Controls.Add(this.rbStdoutOverwrite);
			this.groupBox1.Controls.Add(this.btnBrowseStderr);
			this.groupBox1.Controls.Add(this.btnBrowseStdout);
			this.groupBox1.Controls.Add(this.btnBrowseStdin);
			this.groupBox1.Controls.Add(this.tbStderrFilename);
			this.groupBox1.Controls.Add(this.tbStdoutFilename);
			this.groupBox1.Controls.Add(this.tbStdinFilename);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(14, 76);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox1.Size = new System.Drawing.Size(750, 127);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Standard FIles";
			// 
			// rbStderrAppend
			// 
			this.rbStderrAppend.AutoCheck = false;
			this.rbStderrAppend.AutoSize = true;
			this.rbStderrAppend.Location = new System.Drawing.Point(677, 89);
			this.rbStderrAppend.Margin = new System.Windows.Forms.Padding(2);
			this.rbStderrAppend.Name = "rbStderrAppend";
			this.rbStderrAppend.Size = new System.Drawing.Size(62, 17);
			this.rbStderrAppend.TabIndex = 3;
			this.rbStderrAppend.TabStop = true;
			this.rbStderrAppend.Text = "Append";
			this.rbStderrAppend.UseVisualStyleBackColor = true;
			// 
			// rbStdoutAppend
			// 
			this.rbStdoutAppend.AutoCheck = false;
			this.rbStdoutAppend.AutoSize = true;
			this.rbStdoutAppend.Location = new System.Drawing.Point(677, 64);
			this.rbStdoutAppend.Margin = new System.Windows.Forms.Padding(2);
			this.rbStdoutAppend.Name = "rbStdoutAppend";
			this.rbStdoutAppend.Size = new System.Drawing.Size(62, 17);
			this.rbStdoutAppend.TabIndex = 3;
			this.rbStdoutAppend.TabStop = true;
			this.rbStdoutAppend.Text = "Append";
			this.rbStdoutAppend.UseVisualStyleBackColor = true;
			// 
			// rbStderrOverwrite
			// 
			this.rbStderrOverwrite.AutoCheck = false;
			this.rbStderrOverwrite.AutoSize = true;
			this.rbStderrOverwrite.Location = new System.Drawing.Point(606, 89);
			this.rbStderrOverwrite.Margin = new System.Windows.Forms.Padding(2);
			this.rbStderrOverwrite.Name = "rbStderrOverwrite";
			this.rbStderrOverwrite.Size = new System.Drawing.Size(70, 17);
			this.rbStderrOverwrite.TabIndex = 3;
			this.rbStderrOverwrite.TabStop = true;
			this.rbStderrOverwrite.Text = "Overwrite";
			this.rbStderrOverwrite.UseVisualStyleBackColor = true;
			// 
			// rbStdoutOverwrite
			// 
			this.rbStdoutOverwrite.AutoCheck = false;
			this.rbStdoutOverwrite.AutoSize = true;
			this.rbStdoutOverwrite.Location = new System.Drawing.Point(606, 64);
			this.rbStdoutOverwrite.Margin = new System.Windows.Forms.Padding(2);
			this.rbStdoutOverwrite.Name = "rbStdoutOverwrite";
			this.rbStdoutOverwrite.Size = new System.Drawing.Size(70, 17);
			this.rbStdoutOverwrite.TabIndex = 3;
			this.rbStdoutOverwrite.TabStop = true;
			this.rbStdoutOverwrite.Text = "Overwrite";
			this.rbStdoutOverwrite.UseVisualStyleBackColor = true;
			// 
			// btnBrowseStderr
			// 
			this.btnBrowseStderr.Location = new System.Drawing.Point(569, 89);
			this.btnBrowseStderr.Margin = new System.Windows.Forms.Padding(2);
			this.btnBrowseStderr.Name = "btnBrowseStderr";
			this.btnBrowseStderr.Size = new System.Drawing.Size(23, 19);
			this.btnBrowseStderr.TabIndex = 2;
			this.btnBrowseStderr.Text = "...";
			this.btnBrowseStderr.UseVisualStyleBackColor = true;
			this.btnBrowseStderr.Click += new System.EventHandler(this.btnBrowseStderr_Click);
			// 
			// btnBrowseStdout
			// 
			this.btnBrowseStdout.Location = new System.Drawing.Point(569, 62);
			this.btnBrowseStdout.Margin = new System.Windows.Forms.Padding(2);
			this.btnBrowseStdout.Name = "btnBrowseStdout";
			this.btnBrowseStdout.Size = new System.Drawing.Size(23, 19);
			this.btnBrowseStdout.TabIndex = 2;
			this.btnBrowseStdout.Text = "...";
			this.btnBrowseStdout.UseVisualStyleBackColor = true;
			this.btnBrowseStdout.Click += new System.EventHandler(this.btnBrowseStdout_Click);
			// 
			// btnBrowseStdin
			// 
			this.btnBrowseStdin.Location = new System.Drawing.Point(569, 34);
			this.btnBrowseStdin.Margin = new System.Windows.Forms.Padding(2);
			this.btnBrowseStdin.Name = "btnBrowseStdin";
			this.btnBrowseStdin.Size = new System.Drawing.Size(23, 19);
			this.btnBrowseStdin.TabIndex = 2;
			this.btnBrowseStdin.Text = "...";
			this.btnBrowseStdin.UseVisualStyleBackColor = true;
			this.btnBrowseStdin.Click += new System.EventHandler(this.btnBrowseStdin_Click);
			// 
			// tbStderrFilename
			// 
			this.tbStderrFilename.Location = new System.Drawing.Point(136, 90);
			this.tbStderrFilename.Margin = new System.Windows.Forms.Padding(2);
			this.tbStderrFilename.Name = "tbStderrFilename";
			this.tbStderrFilename.Size = new System.Drawing.Size(429, 20);
			this.tbStderrFilename.TabIndex = 1;
			// 
			// tbStdoutFilename
			// 
			this.tbStdoutFilename.Location = new System.Drawing.Point(136, 61);
			this.tbStdoutFilename.Margin = new System.Windows.Forms.Padding(2);
			this.tbStdoutFilename.Name = "tbStdoutFilename";
			this.tbStdoutFilename.Size = new System.Drawing.Size(429, 20);
			this.tbStdoutFilename.TabIndex = 1;
			// 
			// tbStdinFilename
			// 
			this.tbStdinFilename.Location = new System.Drawing.Point(136, 33);
			this.tbStdinFilename.Margin = new System.Windows.Forms.Padding(2);
			this.tbStdinFilename.Name = "tbStdinFilename";
			this.tbStdinFilename.Size = new System.Drawing.Size(429, 20);
			this.tbStdinFilename.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(4, 93);
			this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(107, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Standard Error(stderr)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 63);
			this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(120, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Standard Output(stdout)";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 32);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(105, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Standard Input(stdin)";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			// 
			// General
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cbCloseFiles);
			this.Controls.Add(this.cbSync);
			this.Name = "General";
			this.Size = new System.Drawing.Size(780, 235);
			this.Load += new System.EventHandler(this.General_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbCloseFiles;
        private System.Windows.Forms.CheckBox cbSync;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbStderrAppend;
        private System.Windows.Forms.RadioButton rbStdoutAppend;
        private System.Windows.Forms.RadioButton rbStderrOverwrite;
        private System.Windows.Forms.RadioButton rbStdoutOverwrite;
        private System.Windows.Forms.Button btnBrowseStderr;
        private System.Windows.Forms.Button btnBrowseStdout;
        private System.Windows.Forms.Button btnBrowseStdin;
        private System.Windows.Forms.TextBox tbStderrFilename;
        private System.Windows.Forms.TextBox tbStdoutFilename;
        private System.Windows.Forms.TextBox tbStdinFilename;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
