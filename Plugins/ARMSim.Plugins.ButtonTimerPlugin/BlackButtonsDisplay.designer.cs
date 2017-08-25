namespace ARMSim.Plugins.ButtonTimerPlugin
{
    partial class BlackButtonsDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlackButtonsDisplay));
            this.rightButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rightButton
            // 
            this.rightButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rightButton.BackgroundImage")));
            this.rightButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rightButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightButton.Location = new System.Drawing.Point(60, 0);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(58, 58);
            this.rightButton.TabIndex = 3;
            this.rightButton.Text = "button2";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // leftButton
            // 
            this.leftButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("leftButton.BackgroundImage")));
            this.leftButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.leftButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftButton.Location = new System.Drawing.Point(0, 0);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(58, 58);
            this.leftButton.TabIndex = 2;
            this.leftButton.Text = "button1";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // BlackButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.leftButton);
            this.Name = "BlackButtons";
            this.Size = new System.Drawing.Size(118, 58);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button leftButton;
    }
}
