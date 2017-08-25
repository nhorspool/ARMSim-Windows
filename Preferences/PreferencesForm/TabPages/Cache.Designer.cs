namespace ARMSim.Preferences.PreferencesForm.TabPages
{
    partial class ARMCache
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
            ARMSim.Preferences.DataCachePreferences dataCachePreferences2 = new ARMSim.Preferences.DataCachePreferences();
            ARMSim.Preferences.InstructionCachePreferences instructionCachePreferences2 = new ARMSim.Preferences.InstructionCachePreferences();
            this.cbUnifiedCache = new System.Windows.Forms.CheckBox();
            this.dataCache = new ARMSim.Preferences.PreferencesForm.Controls.DataCache();
            this.instructionCache = new ARMSim.Preferences.PreferencesForm.Controls.DataCache();
            this.btnCacheWizard = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbUnifiedCache
            // 
            this.cbUnifiedCache.AutoSize = true;
            this.cbUnifiedCache.Location = new System.Drawing.Point(12, 11);
            this.cbUnifiedCache.Margin = new System.Windows.Forms.Padding(2);
            this.cbUnifiedCache.Name = "cbUnifiedCache";
            this.cbUnifiedCache.Size = new System.Drawing.Size(198, 17);
            this.cbUnifiedCache.TabIndex = 2;
            this.cbUnifiedCache.Text = "Unified Data and Instruction Cache?";
            this.cbUnifiedCache.UseVisualStyleBackColor = true;
            this.cbUnifiedCache.CheckedChanged += new System.EventHandler(this.cbUnifiedCache_CheckedChanged);
            // 
            // dataCache
            // 
            this.dataCache.Location = new System.Drawing.Point(12, 185);
            this.dataCache.Name = "dataCache";
            dataCachePreferences2.AllocatePolicy = ARMPluginInterfaces.Preferences.AllocatePolicyEnum.Both;
            dataCachePreferences2.BlockSize = ((uint)(4u));
            dataCachePreferences2.BlocksPerSet = ((uint)(1u));
            dataCachePreferences2.Enabled = false;
            dataCachePreferences2.NumberBlocks = ((uint)(1u));
            dataCachePreferences2.ReplaceStrategy = ARMPluginInterfaces.Preferences.ReplaceStrategiesEnum.RoundRobin;
            dataCachePreferences2.WritePolicy = ARMPluginInterfaces.Preferences.WritePolicyEnum.WriteBack;
            this.dataCache.Preferences = dataCachePreferences2;
            this.dataCache.Size = new System.Drawing.Size(712, 242);
            this.dataCache.TabIndex = 4;
            this.dataCache.Title = "Data Cache";
            // 
            // instructionCache
            // 
            this.instructionCache.InstructionCache = true;
            this.instructionCache.Location = new System.Drawing.Point(12, 33);
            this.instructionCache.Name = "instructionCache";
            instructionCachePreferences2.BlockSize = ((uint)(4u));
            instructionCachePreferences2.BlocksPerSet = ((uint)(1u));
            instructionCachePreferences2.Enabled = false;
            instructionCachePreferences2.NumberBlocks = ((uint)(1u));
            instructionCachePreferences2.ReplaceStrategy = ARMPluginInterfaces.Preferences.ReplaceStrategiesEnum.RoundRobin;
            this.instructionCache.Preferences = instructionCachePreferences2;
            this.instructionCache.Size = new System.Drawing.Size(712, 146);
            this.instructionCache.TabIndex = 3;
            this.instructionCache.Title = "Instruction Cache";
            // 
            // btnCacheWizard
            // 
            this.btnCacheWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCacheWizard.Location = new System.Drawing.Point(116, 433);
            this.btnCacheWizard.Margin = new System.Windows.Forms.Padding(2);
            this.btnCacheWizard.Name = "btnCacheWizard";
            this.btnCacheWizard.Size = new System.Drawing.Size(100, 19);
            this.btnCacheWizard.TabIndex = 6;
            this.btnCacheWizard.Text = "Cache Wizard";
            this.btnCacheWizard.UseVisualStyleBackColor = true;
            this.btnCacheWizard.Click += new System.EventHandler(this.btnCacheWizard_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestore.Location = new System.Drawing.Point(12, 433);
            this.btnRestore.Margin = new System.Windows.Forms.Padding(2);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(100, 19);
            this.btnRestore.TabIndex = 5;
            this.btnRestore.Text = "Restore Defaults";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // Cache
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCacheWizard);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.dataCache);
            this.Controls.Add(this.instructionCache);
            this.Controls.Add(this.cbUnifiedCache);
            this.Name = "Cache";
            this.Size = new System.Drawing.Size(737, 464);
            this.Load += new System.EventHandler(this.Cache_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUnifiedCache;
        private ARMSim.Preferences.PreferencesForm.Controls.DataCache instructionCache;
        private ARMSim.Preferences.PreferencesForm.Controls.DataCache dataCache;
        private System.Windows.Forms.Button btnCacheWizard;
        private System.Windows.Forms.Button btnRestore;
    }
}
