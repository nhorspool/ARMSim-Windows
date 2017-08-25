namespace ARMSim.GUI.Views
{
    partial class MemoryView
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rb32Bit = new System.Windows.Forms.RadioButton();
			this.rb16Bit = new System.Windows.Forms.RadioButton();
			this.rb8Bit = new System.Windows.Forms.RadioButton();
			this.tbAddress = new ARMSim.GUI.Views.NText();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Silver;
			this.panel1.ContextMenuStrip = this.contextMenuStrip1;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.panel1.Location = new System.Drawing.Point(0, 49);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(760, 218);
			this.panel1.TabIndex = 9;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point(115, 11);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(16, 31);
			this.vScrollBar1.TabIndex = 8;
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.groupBox1.Controls.Add(this.rb32Bit);
			this.groupBox1.Controls.Add(this.rb16Bit);
			this.groupBox1.Controls.Add(this.rb8Bit);
			this.groupBox1.Location = new System.Drawing.Point(616, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(141, 42);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Word Size";
			// 
			// rb32Bit
			// 
			this.rb32Bit.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb32Bit.AutoSize = true;
			this.rb32Bit.Checked = true;
			this.rb32Bit.Location = new System.Drawing.Point(94, 14);
			this.rb32Bit.Name = "rb32Bit";
			this.rb32Bit.Size = new System.Drawing.Size(41, 23);
			this.rb32Bit.TabIndex = 0;
			this.rb32Bit.TabStop = true;
			this.rb32Bit.Text = "32Bit";
			this.rb32Bit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.rb32Bit.UseVisualStyleBackColor = true;
			this.rb32Bit.CheckedChanged += new System.EventHandler(this.rbBit_CheckedChanged);
			// 
			// rb16Bit
			// 
			this.rb16Bit.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb16Bit.AutoSize = true;
			this.rb16Bit.Location = new System.Drawing.Point(47, 14);
			this.rb16Bit.Name = "rb16Bit";
			this.rb16Bit.Size = new System.Drawing.Size(41, 23);
			this.rb16Bit.TabIndex = 0;
			this.rb16Bit.Text = "16Bit";
			this.rb16Bit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.rb16Bit.UseVisualStyleBackColor = true;
			this.rb16Bit.CheckedChanged += new System.EventHandler(this.rbBit_CheckedChanged);
			// 
			// rb8Bit
			// 
			this.rb8Bit.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb8Bit.AutoSize = true;
			this.rb8Bit.Location = new System.Drawing.Point(6, 14);
			this.rb8Bit.Name = "rb8Bit";
			this.rb8Bit.Size = new System.Drawing.Size(35, 23);
			this.rb8Bit.TabIndex = 0;
			this.rb8Bit.Text = "8Bit";
			this.rb8Bit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.rb8Bit.UseVisualStyleBackColor = true;
			this.rb8Bit.CheckedChanged += new System.EventHandler(this.rbBit_CheckedChanged);
			// 
			// tbAddress
			// 
			this.tbAddress.AcceptsReturn = true;
			this.tbAddress.Location = new System.Drawing.Point(12, 14);
			this.tbAddress.Name = "tbAddress";
			this.tbAddress.ResolveSymbolHandler = null;
			this.tbAddress.Size = new System.Drawing.Size(100, 20);
			this.tbAddress.TabIndex = 10;
			this.tbAddress.Text = "00000000";
			this.tbAddress.Value = ((uint)(0u));
			this.tbAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAddress_KeyDown);
			// 
			// MemoryView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tbAddress);
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.groupBox1);
			this.Name = "MemoryView";
			this.Size = new System.Drawing.Size(760, 267);
			this.Resize += new System.EventHandler(this.MemoryView_Resize);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb32Bit;
        private System.Windows.Forms.RadioButton rb16Bit;
        private System.Windows.Forms.RadioButton rb8Bit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private NText tbAddress;
    }
}
