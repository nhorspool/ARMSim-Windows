namespace ARMSim.Preferences.PreferencesForm.Controls
{
    partial class DataCache
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbCacheEnable = new System.Windows.Forms.CheckBox();
            this.writePolicy = new ARMSim.Preferences.PreferencesForm.Controls.WritePolicy();
            this.allocatePolicy = new ARMSim.Preferences.PreferencesForm.Controls.AllocatePolicy();
            this.replacementStrategy = new ARMSim.Preferences.PreferencesForm.Controls.ReplacementStrategy();
            this.cacheSize = new ARMSim.Preferences.PreferencesForm.Controls.CacheSize();
            this.associativity = new ARMSim.Preferences.PreferencesForm.Controls.Associativity();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbCacheEnable);
            this.groupBox1.Controls.Add(this.writePolicy);
            this.groupBox1.Controls.Add(this.allocatePolicy);
            this.groupBox1.Controls.Add(this.replacementStrategy);
            this.groupBox1.Controls.Add(this.cacheSize);
            this.groupBox1.Controls.Add(this.associativity);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(712, 242);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Cache";
            // 
            // cbCacheEnable
            // 
            this.cbCacheEnable.AutoSize = true;
            this.cbCacheEnable.Location = new System.Drawing.Point(16, 20);
            this.cbCacheEnable.Name = "cbCacheEnable";
            this.cbCacheEnable.Size = new System.Drawing.Size(65, 17);
            this.cbCacheEnable.TabIndex = 11;
            this.cbCacheEnable.Text = "Enabled";
            this.cbCacheEnable.UseVisualStyleBackColor = true;
            this.cbCacheEnable.CheckedChanged += new System.EventHandler(this.cbCacheEnable_CheckedChanged);
            // 
            // writePolicy
            // 
            this.writePolicy.Location = new System.Drawing.Point(139, 141);
            this.writePolicy.Name = "writePolicy";
            this.writePolicy.Size = new System.Drawing.Size(133, 92);
            this.writePolicy.TabIndex = 10;
            this.writePolicy.WritePolicyType = ARMPluginInterfaces.Preferences.WritePolicyEnum.WriteBack;
            // 
            // allocatePolicy
            // 
            this.allocatePolicy.AllocatePolicyType = ARMPluginInterfaces.Preferences.AllocatePolicyEnum.Both;
            this.allocatePolicy.Location = new System.Drawing.Point(16, 141);
            this.allocatePolicy.Name = "allocatePolicy";
            this.allocatePolicy.Size = new System.Drawing.Size(117, 92);
            this.allocatePolicy.TabIndex = 9;
            // 
            // replacementStrategy
            // 
            this.replacementStrategy.Location = new System.Drawing.Point(565, 43);
            this.replacementStrategy.Name = "replacementStrategy";
            this.replacementStrategy.ReplaceStrategyType = ARMPluginInterfaces.Preferences.ReplaceStrategiesEnum.RoundRobin;
            this.replacementStrategy.Size = new System.Drawing.Size(137, 92);
            this.replacementStrategy.TabIndex = 8;
            // 
            // cacheSize
            // 
            this.cacheSize.Location = new System.Drawing.Point(16, 43);
            this.cacheSize.Name = "cacheSize";
            this.cacheSize.Size = new System.Drawing.Size(294, 92);
            this.cacheSize.TabIndex = 7;
            // 
            // associativity
            // 
            this.associativity.Location = new System.Drawing.Point(316, 43);
            this.associativity.Name = "associativity";
            this.associativity.Size = new System.Drawing.Size(243, 92);
            this.associativity.TabIndex = 6;
            // 
            // DataCache
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DataCache";
            this.Size = new System.Drawing.Size(712, 242);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Associativity associativity;
        private CacheSize cacheSize;
        private ReplacementStrategy replacementStrategy;
        private WritePolicy writePolicy;
        private AllocatePolicy allocatePolicy;
        private System.Windows.Forms.CheckBox cbCacheEnable;
    }
}
