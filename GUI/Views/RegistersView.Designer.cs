namespace ARMSim.GUI.Views
{
    partial class RegistersView
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSigned = new System.Windows.Forms.RadioButton();
            this.btnUsigned = new System.Windows.Forms.RadioButton();
            this.btnHex = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDouble = new System.Windows.Forms.RadioButton();
            this.btnSingle = new System.Windows.Forms.RadioButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(211, 369);
            this.tabControl1.TabIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.btnSigned);
            this.tabPage1.Controls.Add(this.btnUsigned);
            this.tabPage1.Controls.Add(this.btnHex);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabPage1.Size = new System.Drawing.Size(203, 343);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General Purpose";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Resize += new System.EventHandler(this.tabPage1_Resize);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(1, 76);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(201, 265);
            this.panel1.TabIndex = 6;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSigned
            // 
            this.btnSigned.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSigned.AutoSize = true;
            this.btnSigned.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSigned.Location = new System.Drawing.Point(1, 48);
            this.btnSigned.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnSigned.Name = "btnSigned";
            this.btnSigned.Size = new System.Drawing.Size(201, 23);
            this.btnSigned.TabIndex = 5;
            this.btnSigned.Text = "Signed Decimal";
            this.btnSigned.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSigned.UseVisualStyleBackColor = true;
            this.btnSigned.CheckedChanged += new System.EventHandler(this.btn_CheckedChanged);
            // 
            // btnUsigned
            // 
            this.btnUsigned.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnUsigned.AutoSize = true;
            this.btnUsigned.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUsigned.Location = new System.Drawing.Point(1, 25);
            this.btnUsigned.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnUsigned.Name = "btnUsigned";
            this.btnUsigned.Size = new System.Drawing.Size(201, 23);
            this.btnUsigned.TabIndex = 4;
            this.btnUsigned.Text = "Unsigned Decimal";
            this.btnUsigned.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnUsigned.UseVisualStyleBackColor = true;
            this.btnUsigned.CheckedChanged += new System.EventHandler(this.btn_CheckedChanged);
            // 
            // btnHex
            // 
            this.btnHex.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnHex.AutoSize = true;
            this.btnHex.Checked = true;
            this.btnHex.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHex.Location = new System.Drawing.Point(1, 2);
            this.btnHex.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(201, 23);
            this.btnHex.TabIndex = 3;
            this.btnHex.TabStop = true;
            this.btnHex.Text = "Hexadecimal";
            this.btnHex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.CheckedChanged += new System.EventHandler(this.btn_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.btnDouble);
            this.tabPage2.Controls.Add(this.btnSingle);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.tabPage2.Size = new System.Drawing.Size(203, 343);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Floating Point";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Resize += new System.EventHandler(this.tabPage2_Resize);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BackColor = System.Drawing.Color.Silver;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold);
            this.panel2.Location = new System.Drawing.Point(1, 65);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(201, 276);
            this.panel2.TabIndex = 3;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // btnDouble
            // 
            this.btnDouble.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnDouble.AutoSize = true;
            this.btnDouble.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDouble.Location = new System.Drawing.Point(1, 25);
            this.btnDouble.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnDouble.Name = "btnDouble";
            this.btnDouble.Size = new System.Drawing.Size(201, 23);
            this.btnDouble.TabIndex = 2;
            this.btnDouble.TabStop = true;
            this.btnDouble.Text = "Double";
            this.btnDouble.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnDouble.UseVisualStyleBackColor = true;
            this.btnDouble.CheckedChanged += new System.EventHandler(this.btn_CheckedChanged);
            // 
            // btnSingle
            // 
            this.btnSingle.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSingle.AutoSize = true;
            this.btnSingle.Checked = true;
            this.btnSingle.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSingle.Location = new System.Drawing.Point(1, 2);
            this.btnSingle.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnSingle.Name = "btnSingle";
            this.btnSingle.Size = new System.Drawing.Size(201, 23);
            this.btnSingle.TabIndex = 1;
            this.btnSingle.TabStop = true;
            this.btnSingle.Text = "Single";
            this.btnSingle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSingle.UseVisualStyleBackColor = true;
            this.btnSingle.CheckedChanged += new System.EventHandler(this.btn_CheckedChanged);
            // 
            // RegistersView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "RegistersView";
            this.Size = new System.Drawing.Size(211, 369);
            this.Load += new System.EventHandler(this.RegistersView_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RadioButton btnSigned;
        private System.Windows.Forms.RadioButton btnUsigned;
        private System.Windows.Forms.RadioButton btnHex;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RadioButton btnDouble;
        private System.Windows.Forms.RadioButton btnSingle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }

}
