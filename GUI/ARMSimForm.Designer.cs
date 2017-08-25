namespace ARMSim.GUI
{
    partial class ARMSimForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ARMSimForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadMultipleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuFileRecentFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.registersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.watchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.dataCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.instructionCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unifiedCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.boardControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.memoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.purgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.stepIntoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.clearAllBreakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.watchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeWatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
			this.resetLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.menuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.cacheToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.watchToolStripMenuItem,
            this.helToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(518, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.loadMultipleToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator1,
            this.menuFileRecentFile,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.loadToolStripMenuItem.Text = "&Load";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// loadMultipleToolStripMenuItem
			// 
			this.loadMultipleToolStripMenuItem.Name = "loadMultipleToolStripMenuItem";
			this.loadMultipleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.loadMultipleToolStripMenuItem.Text = "L&oad Multiple";
			this.loadMultipleToolStripMenuItem.Click += new System.EventHandler(this.loadMultipleToolStripMenuItem_Click);
			// 
			// reloadToolStripMenuItem
			// 
			this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
			this.reloadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.reloadToolStripMenuItem.Text = "&Reload";
			this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.preferencesToolStripMenuItem.Text = "&Preferences";
			this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// menuFileRecentFile
			// 
			this.menuFileRecentFile.Enabled = false;
			this.menuFileRecentFile.Name = "menuFileRecentFile";
			this.menuFileRecentFile.Size = new System.Drawing.Size(152, 22);
			this.menuFileRecentFile.Text = "Recent Files";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registersToolStripMenuItem,
            this.outputToolStripMenuItem,
            this.stackToolStripMenuItem,
            this.watchToolStripMenuItem1,
            this.dataCacheToolStripMenuItem,
            this.instructionCacheToolStripMenuItem,
            this.unifiedCacheToolStripMenuItem,
            this.boardControlsToolStripMenuItem,
            this.toolStripSeparator3,
            this.memoryToolStripMenuItem,
            this.toolStripSeparator9,
            this.resetLayoutToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.viewToolStripMenuItem.Text = "&View";
			this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
			// 
			// registersToolStripMenuItem
			// 
			this.registersToolStripMenuItem.CheckOnClick = true;
			this.registersToolStripMenuItem.Name = "registersToolStripMenuItem";
			this.registersToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.registersToolStripMenuItem.Text = "&Registers";
			this.registersToolStripMenuItem.Click += new System.EventHandler(this.registersToolStripMenuItem_Click);
			// 
			// outputToolStripMenuItem
			// 
			this.outputToolStripMenuItem.CheckOnClick = true;
			this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
			this.outputToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.outputToolStripMenuItem.Text = "&Output";
			this.outputToolStripMenuItem.Click += new System.EventHandler(this.outputToolStripMenuItem_Click);
			// 
			// stackToolStripMenuItem
			// 
			this.stackToolStripMenuItem.CheckOnClick = true;
			this.stackToolStripMenuItem.Name = "stackToolStripMenuItem";
			this.stackToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.stackToolStripMenuItem.Text = "&Stack";
			this.stackToolStripMenuItem.Click += new System.EventHandler(this.stackToolStripMenuItem_Click);
			// 
			// watchToolStripMenuItem1
			// 
			this.watchToolStripMenuItem1.CheckOnClick = true;
			this.watchToolStripMenuItem1.Name = "watchToolStripMenuItem1";
			this.watchToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
			this.watchToolStripMenuItem1.Text = "&Watch";
			this.watchToolStripMenuItem1.Click += new System.EventHandler(this.watchToolStripMenuItem1_Click);
			// 
			// dataCacheToolStripMenuItem
			// 
			this.dataCacheToolStripMenuItem.CheckOnClick = true;
			this.dataCacheToolStripMenuItem.Name = "dataCacheToolStripMenuItem";
			this.dataCacheToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.dataCacheToolStripMenuItem.Text = "&Data Cache";
			this.dataCacheToolStripMenuItem.Click += new System.EventHandler(this.dataCacheToolStripMenuItem_Click);
			// 
			// instructionCacheToolStripMenuItem
			// 
			this.instructionCacheToolStripMenuItem.CheckOnClick = true;
			this.instructionCacheToolStripMenuItem.Name = "instructionCacheToolStripMenuItem";
			this.instructionCacheToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.instructionCacheToolStripMenuItem.Text = "&Instruction Cache";
			this.instructionCacheToolStripMenuItem.Click += new System.EventHandler(this.instructionCacheToolStripMenuItem_Click);
			// 
			// unifiedCacheToolStripMenuItem
			// 
			this.unifiedCacheToolStripMenuItem.CheckOnClick = true;
			this.unifiedCacheToolStripMenuItem.Name = "unifiedCacheToolStripMenuItem";
			this.unifiedCacheToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.unifiedCacheToolStripMenuItem.Text = "&Unified Cache";
			this.unifiedCacheToolStripMenuItem.Click += new System.EventHandler(this.unifiedCacheToolStripMenuItem_Click);
			// 
			// boardControlsToolStripMenuItem
			// 
			this.boardControlsToolStripMenuItem.CheckOnClick = true;
			this.boardControlsToolStripMenuItem.Name = "boardControlsToolStripMenuItem";
			this.boardControlsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.boardControlsToolStripMenuItem.Text = "&PluginsUI";
			this.boardControlsToolStripMenuItem.Click += new System.EventHandler(this.PluginsUIToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(156, 6);
			// 
			// memoryToolStripMenuItem
			// 
			this.memoryToolStripMenuItem.Name = "memoryToolStripMenuItem";
			this.memoryToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.memoryToolStripMenuItem.Text = "&Memory";
			this.memoryToolStripMenuItem.Click += new System.EventHandler(this.memoryToolStripMenuItem_Click);
			// 
			// cacheToolStripMenuItem
			// 
			this.cacheToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.purgeToolStripMenuItem,
            this.toolStripSeparator4,
            this.statisticsToolStripMenuItem});
			this.cacheToolStripMenuItem.Name = "cacheToolStripMenuItem";
			this.cacheToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
			this.cacheToolStripMenuItem.Text = "&Cache";
			this.cacheToolStripMenuItem.DropDownOpening += new System.EventHandler(this.cacheToolStripMenuItem_DropDownOpening);
			// 
			// resetToolStripMenuItem
			// 
			this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
			this.resetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.resetToolStripMenuItem.Text = "&Reset";
			this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
			// 
			// purgeToolStripMenuItem
			// 
			this.purgeToolStripMenuItem.Name = "purgeToolStripMenuItem";
			this.purgeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.purgeToolStripMenuItem.Text = "&Purge";
			this.purgeToolStripMenuItem.Click += new System.EventHandler(this.purgeToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
			// 
			// statisticsToolStripMenuItem
			// 
			this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
			this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.statisticsToolStripMenuItem.Text = "&Statistics";
			this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.toolStripSeparator5,
            this.stepIntoToolStripMenuItem,
            this.stepOverToolStripMenuItem,
            this.toolStripSeparator6,
            this.restartToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripSeparator7,
            this.clearAllBreakpointsToolStripMenuItem});
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			this.debugToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
			this.debugToolStripMenuItem.Text = "&Debug";
			this.debugToolStripMenuItem.DropDownOpening += new System.EventHandler(this.debugToolStripMenuItem_DropDownOpening);
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.runToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.runToolStripMenuItem.Text = "&Run";
			this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(169, 6);
			// 
			// stepIntoToolStripMenuItem
			// 
			this.stepIntoToolStripMenuItem.Name = "stepIntoToolStripMenuItem";
			this.stepIntoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
			this.stepIntoToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.stepIntoToolStripMenuItem.Text = "Step &Into";
			this.stepIntoToolStripMenuItem.Click += new System.EventHandler(this.stepIntoToolStripMenuItem_Click);
			// 
			// stepOverToolStripMenuItem
			// 
			this.stepOverToolStripMenuItem.Name = "stepOverToolStripMenuItem";
			this.stepOverToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
			this.stepOverToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.stepOverToolStripMenuItem.Text = "Step &Over";
			this.stepOverToolStripMenuItem.Click += new System.EventHandler(this.stepOverToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(169, 6);
			// 
			// restartToolStripMenuItem
			// 
			this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
			this.restartToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.restartToolStripMenuItem.Text = "&Restart";
			this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.stopToolStripMenuItem.Text = "&Stop";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(169, 6);
			// 
			// clearAllBreakpointsToolStripMenuItem
			// 
			this.clearAllBreakpointsToolStripMenuItem.Name = "clearAllBreakpointsToolStripMenuItem";
			this.clearAllBreakpointsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.clearAllBreakpointsToolStripMenuItem.Text = "&Clear All Breakpoints";
			this.clearAllBreakpointsToolStripMenuItem.Click += new System.EventHandler(this.clearAllBreakpointsToolStripMenuItem_Click);
			// 
			// watchToolStripMenuItem
			// 
			this.watchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addWatchToolStripMenuItem,
            this.removeWatchToolStripMenuItem,
            this.clearAllToolStripMenuItem});
			this.watchToolStripMenuItem.Name = "watchToolStripMenuItem";
			this.watchToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
			this.watchToolStripMenuItem.Text = "&Watch";
			this.watchToolStripMenuItem.DropDownOpening += new System.EventHandler(this.watchToolStripMenuItem_DropDownOpening);
			// 
			// addWatchToolStripMenuItem
			// 
			this.addWatchToolStripMenuItem.Name = "addWatchToolStripMenuItem";
			this.addWatchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.addWatchToolStripMenuItem.Text = "&Add Watch";
			this.addWatchToolStripMenuItem.Click += new System.EventHandler(this.addWatchToolStripMenuItem_Click);
			// 
			// removeWatchToolStripMenuItem
			// 
			this.removeWatchToolStripMenuItem.Name = "removeWatchToolStripMenuItem";
			this.removeWatchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.removeWatchToolStripMenuItem.Text = "&Remove Watch";
			this.removeWatchToolStripMenuItem.Click += new System.EventHandler(this.removeWatchToolStripMenuItem_Click);
			// 
			// clearAllToolStripMenuItem
			// 
			this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
			this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.clearAllToolStripMenuItem.Text = "&Clear All";
			this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
			// 
			// helToolStripMenuItem
			// 
			this.helToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helToolStripMenuItem.Name = "helToolStripMenuItem";
			this.helToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helToolStripMenuItem.Text = "&Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripSeparator8,
            this.toolStripButton6});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(518, 25);
			this.toolStrip1.TabIndex = 3;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "Step Into";
			this.toolStripButton1.Click += new System.EventHandler(this.stepIntoToolStripMenuItem_Click);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton2.Text = "Step Over";
			this.toolStripButton2.ToolTipText = "Step Over";
			this.toolStripButton2.Click += new System.EventHandler(this.stepOverToolStripMenuItem_Click);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton3.Text = "Stop";
			this.toolStripButton3.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
			this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton4.Text = "Run";
			this.toolStripButton4.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
			// 
			// toolStripButton5
			// 
			this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
			this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton5.Name = "toolStripButton5";
			this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton5.Text = "Restart";
			this.toolStripButton5.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton6
			// 
			this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
			this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton6.Name = "toolStripButton6";
			this.toolStripButton6.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton6.Text = "Reload";
			this.toolStripButton6.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
			// 
			// resetLayoutToolStripMenuItem
			// 
			this.resetLayoutToolStripMenuItem.Name = "resetLayoutToolStripMenuItem";
			this.resetLayoutToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.resetLayoutToolStripMenuItem.Text = "Reset Layout";
			this.resetLayoutToolStripMenuItem.Click += new System.EventHandler(this.resetLayoutToolStripMenuItem_Click);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(156, 6);
			// 
			// ARMSimForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(518, 362);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ARMSimForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ARMSim# - The ARM Simulator Dept. of Computer Science";
			this.Load += new System.EventHandler(this.ARMSimForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMultipleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFileRecentFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem dataCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unifiedCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boardControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem memoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem purgeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem stepIntoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepOverToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem clearAllBreakpointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addWatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeWatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem resetLayoutToolStripMenuItem;
       //private System.Windows.Forms.Panel panel1;
    }
}

