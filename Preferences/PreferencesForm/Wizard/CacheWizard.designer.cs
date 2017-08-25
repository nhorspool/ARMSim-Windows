namespace ARMSim.Preferences.PreferencesForm.Wizard
{
    partial class CacheWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CacheWizard));
            this.wizard1 = new ARMSim.Preferences.PreferencesForm.Wizard.Wizard();
            this.wizardPageSummary = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.cacheSummary2 = new ARMSim.Preferences.PreferencesForm.Wizard.CacheSummary();
            this.cacheSummary1 = new ARMSim.Preferences.PreferencesForm.Wizard.CacheSummary();
            this.wizardDCachePageAllocate = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.allocatePolicy = new ARMSim.Preferences.PreferencesForm.Controls.AllocatePolicy();
            this.wizardPageDCacheWriteThrough = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.writePolicy = new ARMSim.Preferences.PreferencesForm.Controls.WritePolicy();
            this.wizardPageDCacheReplacement = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.dataReplacementStrategy = new ARMSim.Preferences.PreferencesForm.Controls.ReplacementStrategy();
            this.wizardPageDCacheAssociativity = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.dataAssociativity = new ARMSim.Preferences.PreferencesForm.Controls.Associativity();
            this.wizardPageDCacheSize = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.dataCacheSize = new ARMSim.Preferences.PreferencesForm.Controls.CacheSize();
            this.wizardPageDCacheEnabled = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbDCacheEnableNo = new System.Windows.Forms.RadioButton();
            this.rbDCacheEnableYes = new System.Windows.Forms.RadioButton();
            this.wizardPageICacheReplacement = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.instructionReplacementStrategy = new ARMSim.Preferences.PreferencesForm.Controls.ReplacementStrategy();
            this.wizardPageICacheAssociativity = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.instructionAssociativity = new ARMSim.Preferences.PreferencesForm.Controls.Associativity();
            this.wizardPageICacheSize = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.instructionCacheSize = new ARMSim.Preferences.PreferencesForm.Controls.CacheSize();
            this.wizardPageICacheEnabled = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbICacheEnableNo = new System.Windows.Forms.RadioButton();
            this.rbICacheEnableYes = new System.Windows.Forms.RadioButton();
            this.wizardPageUnified = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbUnifiedNo = new System.Windows.Forms.RadioButton();
            this.rbUnifiedYes = new System.Windows.Forms.RadioButton();
            this.wizardPageWelcome = new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage();
            this.wizard1.SuspendLayout();
            this.wizardPageSummary.SuspendLayout();
            this.wizardDCachePageAllocate.SuspendLayout();
            this.wizardPageDCacheWriteThrough.SuspendLayout();
            this.wizardPageDCacheReplacement.SuspendLayout();
            this.wizardPageDCacheAssociativity.SuspendLayout();
            this.wizardPageDCacheSize.SuspendLayout();
            this.wizardPageDCacheEnabled.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.wizardPageICacheReplacement.SuspendLayout();
            this.wizardPageICacheAssociativity.SuspendLayout();
            this.wizardPageICacheSize.SuspendLayout();
            this.wizardPageICacheEnabled.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.wizardPageUnified.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            this.wizard1.Controls.Add(this.wizardPageSummary);
            this.wizard1.Controls.Add(this.wizardDCachePageAllocate);
            this.wizard1.Controls.Add(this.wizardPageDCacheWriteThrough);
            this.wizard1.Controls.Add(this.wizardPageDCacheReplacement);
            this.wizard1.Controls.Add(this.wizardPageDCacheAssociativity);
            this.wizard1.Controls.Add(this.wizardPageDCacheSize);
            this.wizard1.Controls.Add(this.wizardPageDCacheEnabled);
            this.wizard1.Controls.Add(this.wizardPageICacheReplacement);
            this.wizard1.Controls.Add(this.wizardPageICacheAssociativity);
            this.wizard1.Controls.Add(this.wizardPageICacheSize);
            this.wizard1.Controls.Add(this.wizardPageICacheEnabled);
            this.wizard1.Controls.Add(this.wizardPageUnified);
            this.wizard1.Controls.Add(this.wizardPageWelcome);
            this.wizard1.HeaderImage = ((System.Drawing.Image)(resources.GetObject("wizard1.HeaderImage")));
            this.wizard1.Location = new System.Drawing.Point(0, 0);
            this.wizard1.Name = "wizard1";
            this.wizard1.Pages.AddRange(new ARMSim.Preferences.PreferencesForm.Wizard.WizardPage[] {
            this.wizardPageWelcome,
            this.wizardPageUnified,
            this.wizardPageICacheEnabled,
            this.wizardPageICacheSize,
            this.wizardPageICacheAssociativity,
            this.wizardPageICacheReplacement,
            this.wizardPageDCacheEnabled,
            this.wizardPageDCacheSize,
            this.wizardPageDCacheAssociativity,
            this.wizardPageDCacheReplacement,
            this.wizardPageDCacheWriteThrough,
            this.wizardDCachePageAllocate,
            this.wizardPageSummary});
            this.wizard1.Size = new System.Drawing.Size(513, 393);
            this.wizard1.TabIndex = 0;
            this.wizard1.WelcomeImage = ((System.Drawing.Image)(resources.GetObject("wizard1.WelcomeImage")));
            this.wizard1.BeforeSwitchPages += new ARMSim.Preferences.PreferencesForm.Wizard.Wizard.BeforeSwitchPagesEventHandler(this.wizard1_BeforeSwitchPages);
            // 
            // wizardPageSummary
            // 
            this.wizardPageSummary.Controls.Add(this.cacheSummary2);
            this.wizardPageSummary.Controls.Add(this.cacheSummary1);
            this.wizardPageSummary.Description = "These are the Cache settings you have selected.";
            this.wizardPageSummary.Location = new System.Drawing.Point(0, 0);
            this.wizardPageSummary.Name = "wizardPageSummary";
            this.wizardPageSummary.Size = new System.Drawing.Size(513, 345);
            this.wizardPageSummary.WizardStyle = ARMSim.Preferences.PreferencesForm.Wizard.WizardPage.WizardPageStyle.Finish;
            this.wizardPageSummary.TabIndex = 14;
            this.wizardPageSummary.Title = "Summary";
            // 
            // cacheSummary2
            // 
            this.cacheSummary2.Location = new System.Drawing.Point(252, 73);
            this.cacheSummary2.Name = "cacheSummary2";
            this.cacheSummary2.Size = new System.Drawing.Size(255, 272);
            this.cacheSummary2.TabIndex = 3;
            // 
            // cacheSummary1
            // 
            this.cacheSummary1.InstructionCache = true;
            this.cacheSummary1.Location = new System.Drawing.Point(0, 73);
            this.cacheSummary1.Name = "cacheSummary1";
            this.cacheSummary1.Size = new System.Drawing.Size(255, 272);
            this.cacheSummary1.TabIndex = 2;
            // 
            // wizardDCachePageAllocate
            // 
            this.wizardDCachePageAllocate.Controls.Add(this.allocatePolicy);
            this.wizardDCachePageAllocate.Description = "Use this page to select the allocate policy for this cache.";
            this.wizardDCachePageAllocate.Location = new System.Drawing.Point(0, 0);
            this.wizardDCachePageAllocate.Name = "wizardDCachePageAllocate";
            this.wizardDCachePageAllocate.Size = new System.Drawing.Size(513, 345);
            this.wizardDCachePageAllocate.TabIndex = 13;
            this.wizardDCachePageAllocate.Title = "Allocate Policy";
            // 
            // allocatePolicy
            // 
            this.allocatePolicy.AllocatePolicyType = ARMPluginInterfaces.Preferences.AllocatePolicyEnum.Both;
            this.allocatePolicy.Location = new System.Drawing.Point(172, 144);
            this.allocatePolicy.Margin = new System.Windows.Forms.Padding(2);
            this.allocatePolicy.Name = "allocatePolicy";
            this.allocatePolicy.Size = new System.Drawing.Size(112, 94);
            this.allocatePolicy.TabIndex = 0;
            // 
            // wizardPageDCacheWriteThrough
            // 
            this.wizardPageDCacheWriteThrough.Controls.Add(this.writePolicy);
            this.wizardPageDCacheWriteThrough.Description = "Use this page to select the Write Through Policy for this cache.";
            this.wizardPageDCacheWriteThrough.Location = new System.Drawing.Point(0, 0);
            this.wizardPageDCacheWriteThrough.Name = "wizardPageDCacheWriteThrough";
            this.wizardPageDCacheWriteThrough.Size = new System.Drawing.Size(513, 345);
            this.wizardPageDCacheWriteThrough.TabIndex = 12;
            this.wizardPageDCacheWriteThrough.Title = "Write Through Policy";
            // 
            // writePolicy
            // 
            this.writePolicy.Location = new System.Drawing.Point(170, 150);
            this.writePolicy.Margin = new System.Windows.Forms.Padding(2);
            this.writePolicy.Name = "writePolicy";
            this.writePolicy.Size = new System.Drawing.Size(123, 92);
            this.writePolicy.TabIndex = 0;
            this.writePolicy.WritePolicyType = ARMPluginInterfaces.Preferences.WritePolicyEnum.WriteBack;
            // 
            // wizardPageDCacheReplacement
            // 
            this.wizardPageDCacheReplacement.Controls.Add(this.dataReplacementStrategy);
            this.wizardPageDCacheReplacement.Description = "Use this page to define the replacement policy for this Data cache.";
            this.wizardPageDCacheReplacement.Location = new System.Drawing.Point(0, 0);
            this.wizardPageDCacheReplacement.Name = "wizardPageDCacheReplacement";
            this.wizardPageDCacheReplacement.Size = new System.Drawing.Size(513, 345);
            this.wizardPageDCacheReplacement.TabIndex = 11;
            this.wizardPageDCacheReplacement.Title = "Data Replacement Strategy";
            // 
            // dataReplacementStrategy
            // 
            this.dataReplacementStrategy.Location = new System.Drawing.Point(174, 163);
            this.dataReplacementStrategy.Margin = new System.Windows.Forms.Padding(2);
            this.dataReplacementStrategy.Name = "dataReplacementStrategy";
            this.dataReplacementStrategy.ReplaceStrategyType = ARMPluginInterfaces.Preferences.ReplaceStrategiesEnum.RoundRobin;
            this.dataReplacementStrategy.Size = new System.Drawing.Size(135, 93);
            this.dataReplacementStrategy.TabIndex = 1;
            // 
            // wizardPageDCacheAssociativity
            // 
            this.wizardPageDCacheAssociativity.Controls.Add(this.dataAssociativity);
            this.wizardPageDCacheAssociativity.Description = "Use this page to define the Associativity of the Data Cache.";
            this.wizardPageDCacheAssociativity.Location = new System.Drawing.Point(0, 0);
            this.wizardPageDCacheAssociativity.Name = "wizardPageDCacheAssociativity";
            this.wizardPageDCacheAssociativity.Size = new System.Drawing.Size(513, 345);
            this.wizardPageDCacheAssociativity.TabIndex = 10;
            this.wizardPageDCacheAssociativity.Title = "Data Associativity";
            // 
            // dataAssociativity
            // 
            this.dataAssociativity.Location = new System.Drawing.Point(126, 161);
            this.dataAssociativity.Margin = new System.Windows.Forms.Padding(2);
            this.dataAssociativity.Name = "dataAssociativity";
            this.dataAssociativity.Size = new System.Drawing.Size(243, 93);
            this.dataAssociativity.TabIndex = 1;
            // 
            // wizardPageDCacheSize
            // 
            this.wizardPageDCacheSize.Controls.Add(this.dataCacheSize);
            this.wizardPageDCacheSize.Description = "Use this page to set the size of the data cache in bytes.";
            this.wizardPageDCacheSize.Location = new System.Drawing.Point(0, 0);
            this.wizardPageDCacheSize.Name = "wizardPageDCacheSize";
            this.wizardPageDCacheSize.Size = new System.Drawing.Size(513, 345);
            this.wizardPageDCacheSize.TabIndex = 9;
            this.wizardPageDCacheSize.Title = "Data Cache Size";
            // 
            // dataCacheSize
            // 
            this.dataCacheSize.Location = new System.Drawing.Point(88, 146);
            this.dataCacheSize.Margin = new System.Windows.Forms.Padding(2);
            this.dataCacheSize.Name = "dataCacheSize";
            this.dataCacheSize.Size = new System.Drawing.Size(298, 93);
            this.dataCacheSize.TabIndex = 1;
            // 
            // wizardPageDCacheEnabled
            // 
            this.wizardPageDCacheEnabled.Controls.Add(this.groupBox3);
            this.wizardPageDCacheEnabled.Description = "Use this page to specify if the data cache is to be enabled or disabled.";
            this.wizardPageDCacheEnabled.Location = new System.Drawing.Point(0, 0);
            this.wizardPageDCacheEnabled.Name = "wizardPageDCacheEnabled";
            this.wizardPageDCacheEnabled.Size = new System.Drawing.Size(513, 345);
            this.wizardPageDCacheEnabled.TabIndex = 8;
            this.wizardPageDCacheEnabled.Title = "Data Cache Enable";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbDCacheEnableNo);
            this.groupBox3.Controls.Add(this.rbDCacheEnableYes);
            this.groupBox3.Location = new System.Drawing.Point(95, 94);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(317, 202);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data Cache Enable";
            // 
            // rbDCacheEnableNo
            // 
            this.rbDCacheEnableNo.AutoSize = true;
            this.rbDCacheEnableNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDCacheEnableNo.Location = new System.Drawing.Point(47, 131);
            this.rbDCacheEnableNo.Name = "rbDCacheEnableNo";
            this.rbDCacheEnableNo.Size = new System.Drawing.Size(190, 20);
            this.rbDCacheEnableNo.TabIndex = 1;
            this.rbDCacheEnableNo.TabStop = true;
            this.rbDCacheEnableNo.Text = "Data Cache is Disabled";
            this.rbDCacheEnableNo.UseVisualStyleBackColor = true;
            // 
            // rbDCacheEnableYes
            // 
            this.rbDCacheEnableYes.AutoSize = true;
            this.rbDCacheEnableYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDCacheEnableYes.Location = new System.Drawing.Point(47, 46);
            this.rbDCacheEnableYes.Name = "rbDCacheEnableYes";
            this.rbDCacheEnableYes.Size = new System.Drawing.Size(185, 20);
            this.rbDCacheEnableYes.TabIndex = 0;
            this.rbDCacheEnableYes.TabStop = true;
            this.rbDCacheEnableYes.Text = "Data Cache is Enabled";
            this.rbDCacheEnableYes.UseVisualStyleBackColor = true;
            // 
            // wizardPageICacheReplacement
            // 
            this.wizardPageICacheReplacement.Controls.Add(this.instructionReplacementStrategy);
            this.wizardPageICacheReplacement.Description = "Use this page to define the replacement policy for this Instruction cache.";
            this.wizardPageICacheReplacement.Location = new System.Drawing.Point(0, 0);
            this.wizardPageICacheReplacement.Name = "wizardPageICacheReplacement";
            this.wizardPageICacheReplacement.Size = new System.Drawing.Size(513, 345);
            this.wizardPageICacheReplacement.TabIndex = 7;
            this.wizardPageICacheReplacement.Title = "Instruction Replacement Strategy";
            // 
            // instructionReplacementStrategy
            // 
            this.instructionReplacementStrategy.Location = new System.Drawing.Point(167, 159);
            this.instructionReplacementStrategy.Margin = new System.Windows.Forms.Padding(2);
            this.instructionReplacementStrategy.Name = "instructionReplacementStrategy";
            this.instructionReplacementStrategy.ReplaceStrategyType = ARMPluginInterfaces.Preferences.ReplaceStrategiesEnum.RoundRobin;
            this.instructionReplacementStrategy.Size = new System.Drawing.Size(135, 93);
            this.instructionReplacementStrategy.TabIndex = 0;
            // 
            // wizardPageICacheAssociativity
            // 
            this.wizardPageICacheAssociativity.Controls.Add(this.instructionAssociativity);
            this.wizardPageICacheAssociativity.Description = "Use this page to define the Associativity of the Instruction Cache.";
            this.wizardPageICacheAssociativity.Location = new System.Drawing.Point(0, 0);
            this.wizardPageICacheAssociativity.Name = "wizardPageICacheAssociativity";
            this.wizardPageICacheAssociativity.Size = new System.Drawing.Size(513, 345);
            this.wizardPageICacheAssociativity.TabIndex = 6;
            this.wizardPageICacheAssociativity.Title = "Instruction Associativity";
            // 
            // instructionAssociativity
            // 
            this.instructionAssociativity.Location = new System.Drawing.Point(119, 162);
            this.instructionAssociativity.Margin = new System.Windows.Forms.Padding(2);
            this.instructionAssociativity.Name = "instructionAssociativity";
            this.instructionAssociativity.Size = new System.Drawing.Size(243, 93);
            this.instructionAssociativity.TabIndex = 0;
            // 
            // wizardPageICacheSize
            // 
            this.wizardPageICacheSize.Controls.Add(this.instructionCacheSize);
            this.wizardPageICacheSize.Description = "Use this page to set the size of the instruction cache in bytes.";
            this.wizardPageICacheSize.Location = new System.Drawing.Point(0, 0);
            this.wizardPageICacheSize.Name = "wizardPageICacheSize";
            this.wizardPageICacheSize.Size = new System.Drawing.Size(513, 345);
            this.wizardPageICacheSize.TabIndex = 5;
            this.wizardPageICacheSize.Title = "Instruction Cache Size";
            // 
            // instructionCacheSize
            // 
            this.instructionCacheSize.Location = new System.Drawing.Point(93, 144);
            this.instructionCacheSize.Margin = new System.Windows.Forms.Padding(2);
            this.instructionCacheSize.Name = "instructionCacheSize";
            this.instructionCacheSize.Size = new System.Drawing.Size(298, 93);
            this.instructionCacheSize.TabIndex = 0;
            // 
            // wizardPageICacheEnabled
            // 
            this.wizardPageICacheEnabled.Controls.Add(this.groupBox2);
            this.wizardPageICacheEnabled.Description = "Use this page to specify if the instruction cache is to be enabled or disabled.";
            this.wizardPageICacheEnabled.Location = new System.Drawing.Point(0, 0);
            this.wizardPageICacheEnabled.Name = "wizardPageICacheEnabled";
            this.wizardPageICacheEnabled.Size = new System.Drawing.Size(513, 345);
            this.wizardPageICacheEnabled.TabIndex = 4;
            this.wizardPageICacheEnabled.Title = "Instruction Cache Enable";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbICacheEnableNo);
            this.groupBox2.Controls.Add(this.rbICacheEnableYes);
            this.groupBox2.Location = new System.Drawing.Point(80, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 202);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Instruction Cache Enable";
            // 
            // rbICacheEnableNo
            // 
            this.rbICacheEnableNo.AutoSize = true;
            this.rbICacheEnableNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbICacheEnableNo.Location = new System.Drawing.Point(47, 131);
            this.rbICacheEnableNo.Name = "rbICacheEnableNo";
            this.rbICacheEnableNo.Size = new System.Drawing.Size(227, 20);
            this.rbICacheEnableNo.TabIndex = 1;
            this.rbICacheEnableNo.TabStop = true;
            this.rbICacheEnableNo.Text = "Instruction Cache is Disabled";
            this.rbICacheEnableNo.UseVisualStyleBackColor = true;
            // 
            // rbICacheEnableYes
            // 
            this.rbICacheEnableYes.AutoSize = true;
            this.rbICacheEnableYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbICacheEnableYes.Location = new System.Drawing.Point(47, 46);
            this.rbICacheEnableYes.Name = "rbICacheEnableYes";
            this.rbICacheEnableYes.Size = new System.Drawing.Size(222, 20);
            this.rbICacheEnableYes.TabIndex = 0;
            this.rbICacheEnableYes.TabStop = true;
            this.rbICacheEnableYes.Text = "Instruction Cache is Enabled";
            this.rbICacheEnableYes.UseVisualStyleBackColor = true;
            // 
            // wizardPageUnified
            // 
            this.wizardPageUnified.Controls.Add(this.groupBox1);
            this.wizardPageUnified.Description = "Please define if the simulated cache is to be Unified or Separated";
            this.wizardPageUnified.Location = new System.Drawing.Point(0, 0);
            this.wizardPageUnified.Name = "wizardPageUnified";
            this.wizardPageUnified.Size = new System.Drawing.Size(513, 345);
            this.wizardPageUnified.TabIndex = 3;
            this.wizardPageUnified.Title = "Unified Cache";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbUnifiedNo);
            this.groupBox1.Controls.Add(this.rbUnifiedYes);
            this.groupBox1.Location = new System.Drawing.Point(87, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 232);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Unified Cache";
            // 
            // rbUnifiedNo
            // 
            this.rbUnifiedNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUnifiedNo.Location = new System.Drawing.Point(31, 112);
            this.rbUnifiedNo.Name = "rbUnifiedNo";
            this.rbUnifiedNo.Size = new System.Drawing.Size(232, 62);
            this.rbUnifiedNo.TabIndex = 1;
            this.rbUnifiedNo.TabStop = true;
            this.rbUnifiedNo.Text = "No, this cache contains separate Instruction and Data Caches";
            this.rbUnifiedNo.UseVisualStyleBackColor = true;
            // 
            // rbUnifiedYes
            // 
            this.rbUnifiedYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUnifiedYes.Location = new System.Drawing.Point(31, 32);
            this.rbUnifiedYes.Name = "rbUnifiedYes";
            this.rbUnifiedYes.Size = new System.Drawing.Size(232, 64);
            this.rbUnifiedYes.TabIndex = 0;
            this.rbUnifiedYes.TabStop = true;
            this.rbUnifiedYes.Text = "Yes, this cache contains both the Instruction and Data Cache memory.";
            this.rbUnifiedYes.UseVisualStyleBackColor = true;
            // 
            // wizardPageWelcome
            // 
            this.wizardPageWelcome.Description = "This wizard will guide you through the steps of performing the definition of the " +
                "Cache Settings.";
            this.wizardPageWelcome.Location = new System.Drawing.Point(0, 0);
            this.wizardPageWelcome.Name = "wizardPageWelcome";
            this.wizardPageWelcome.Size = new System.Drawing.Size(513, 345);
            this.wizardPageWelcome.WizardStyle = ARMSim.Preferences.PreferencesForm.Wizard.WizardPage.WizardPageStyle.Welcome;
            this.wizardPageWelcome.TabIndex = 2;
            this.wizardPageWelcome.Title = "Welcome to the Cache Wizard";
            // 
            // CacheWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 393);
            this.Controls.Add(this.wizard1);
            this.Text = this.Name = "CacheWizard";
            this.wizard1.ResumeLayout(false);
            this.wizardPageSummary.ResumeLayout(false);
            this.wizardDCachePageAllocate.ResumeLayout(false);
            this.wizardPageDCacheWriteThrough.ResumeLayout(false);
            this.wizardPageDCacheReplacement.ResumeLayout(false);
            this.wizardPageDCacheAssociativity.ResumeLayout(false);
            this.wizardPageDCacheSize.ResumeLayout(false);
            this.wizardPageDCacheEnabled.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.wizardPageICacheReplacement.ResumeLayout(false);
            this.wizardPageICacheAssociativity.ResumeLayout(false);
            this.wizardPageICacheSize.ResumeLayout(false);
            this.wizardPageICacheEnabled.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.wizardPageUnified.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Wizard wizard1;
        private WizardPage wizardPageWelcome;
        private WizardPage wizardPageUnified;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbUnifiedNo;
        private System.Windows.Forms.RadioButton rbUnifiedYes;
        private WizardPage wizardPageICacheEnabled;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbICacheEnableNo;
        private System.Windows.Forms.RadioButton rbICacheEnableYes;
        private WizardPage wizardPageICacheSize;
        private ARMSim.Preferences.PreferencesForm.Controls.CacheSize instructionCacheSize;
        private WizardPage wizardPageICacheAssociativity;
        private WizardPage wizardPageICacheReplacement;
        private ARMSim.Preferences.PreferencesForm.Controls.ReplacementStrategy instructionReplacementStrategy;
        private ARMSim.Preferences.PreferencesForm.Controls.Associativity instructionAssociativity;
        private WizardPage wizardPageDCacheEnabled;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbDCacheEnableNo;
        private System.Windows.Forms.RadioButton rbDCacheEnableYes;
        private WizardPage wizardPageDCacheReplacement;
        private ARMSim.Preferences.PreferencesForm.Controls.ReplacementStrategy dataReplacementStrategy;
        private WizardPage wizardPageDCacheAssociativity;
        private ARMSim.Preferences.PreferencesForm.Controls.Associativity dataAssociativity;
        private WizardPage wizardPageDCacheSize;
        private ARMSim.Preferences.PreferencesForm.Controls.CacheSize dataCacheSize;
        private WizardPage wizardPageDCacheWriteThrough;
        private ARMSim.Preferences.PreferencesForm.Controls.WritePolicy writePolicy;
        private WizardPage wizardDCachePageAllocate;
        private ARMSim.Preferences.PreferencesForm.Controls.AllocatePolicy allocatePolicy;
        private WizardPage wizardPageSummary;
        private CacheSummary cacheSummary2;
        private CacheSummary cacheSummary1;
    }
}