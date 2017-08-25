namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    partial class InterruptControllerDisplay
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ChannelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Channel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pending = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Current = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblIRQEnabled = new System.Windows.Forms.Label();
            this.lblFIQEnabled = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblVectorMode = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.CausesValidation = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChannelName,
            this.Channel,
            this.Pending,
            this.Mode,
            this.Mask,
            this.Current});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(369, 395);
            this.dataGridView1.TabIndex = 0;
            // 
            // ChannelName
            // 
            this.ChannelName.HeaderText = "Name";
            this.ChannelName.Name = "ChannelName";
            this.ChannelName.ReadOnly = true;
            this.ChannelName.Width = 75;
            // 
            // Channel
            // 
            this.Channel.HeaderText = "Channel";
            this.Channel.Name = "Channel";
            this.Channel.ReadOnly = true;
            this.Channel.Width = 60;
            // 
            // Pending
            // 
            this.Pending.HeaderText = "Pending";
            this.Pending.Name = "Pending";
            this.Pending.ReadOnly = true;
            this.Pending.Width = 60;
            // 
            // Mode
            // 
            this.Mode.HeaderText = "Mode";
            this.Mode.Name = "Mode";
            this.Mode.ReadOnly = true;
            this.Mode.Width = 35;
            // 
            // Mask
            // 
            this.Mask.HeaderText = "Mask";
            this.Mask.Name = "Mask";
            this.Mask.ReadOnly = true;
            this.Mask.Width = 35;
            // 
            // Current
            // 
            this.Current.HeaderText = "Current";
            this.Current.Name = "Current";
            this.Current.ReadOnly = true;
            this.Current.Width = 60;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(374, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IRQ Enabled?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(375, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "FIQ Enabled?";
            // 
            // lblIRQEnabled
            // 
            this.lblIRQEnabled.AutoSize = true;
            this.lblIRQEnabled.Location = new System.Drawing.Point(455, 29);
            this.lblIRQEnabled.Name = "lblIRQEnabled";
            this.lblIRQEnabled.Size = new System.Drawing.Size(35, 13);
            this.lblIRQEnabled.TabIndex = 2;
            this.lblIRQEnabled.Text = "label3";
            // 
            // lblFIQEnabled
            // 
            this.lblFIQEnabled.AutoSize = true;
            this.lblFIQEnabled.Location = new System.Drawing.Point(455, 52);
            this.lblFIQEnabled.Name = "lblFIQEnabled";
            this.lblFIQEnabled.Size = new System.Drawing.Size(35, 13);
            this.lblFIQEnabled.TabIndex = 2;
            this.lblFIQEnabled.Text = "label3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(375, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Vector Mode?";
            // 
            // lblVectorMode
            // 
            this.lblVectorMode.AutoSize = true;
            this.lblVectorMode.Location = new System.Drawing.Point(456, 7);
            this.lblVectorMode.Name = "lblVectorMode";
            this.lblVectorMode.Size = new System.Drawing.Size(35, 13);
            this.lblVectorMode.TabIndex = 2;
            this.lblVectorMode.Text = "label3";
            // 
            // InterruptControllerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblFIQEnabled);
            this.Controls.Add(this.lblVectorMode);
            this.Controls.Add(this.lblIRQEnabled);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "InterruptControllerDisplay";
            this.Size = new System.Drawing.Size(498, 395);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Channel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pending;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mask;
        private System.Windows.Forms.DataGridViewTextBoxColumn Current;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblIRQEnabled;
        private System.Windows.Forms.Label lblFIQEnabled;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblVectorMode;
    }
}
