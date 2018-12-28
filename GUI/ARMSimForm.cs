using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

using ARMSim.GUI.Views;

using ARMSim.Simulator;
using ARMSim.Preferences;
using ARMSimWindowManager;
//using System.Diagnostics.CodeAnalysis;

namespace ARMSim.GUI
{
    public partial class ARMSimForm : Form
    {
        /// <summary>
        /// If the user closes the application while it is running a program
        /// we need a way to detect this and terminate properly.
        /// This flag is set true if the user closes the main window.
        /// </summary>
        private bool mFormClosing;

        private bool mSimulationStopped = true;

        /// <summary>
        /// Instance of the simulator engine
        /// </summary>
        private ApplicationJimulator mJM;

        /// <summary>
        /// Filename of the settings file
        /// </summary>
        private const string mSettingsFilename = ".ARMSimLayout.xml";

        /// <summary>
        /// Root xml element name of the application settings section
        /// </summary>
        private const string mApplicationSettingsElement = "ARMSimApplicationSettings";

        /// <summary>
        /// Xml element name of the mainform attributes (size, location, state etc)
        /// </summary>
        private const string mMainFormElement = "MainForm";

        /// <summary>
        /// Xml element name of the window settings
        /// </summary>
        private const string mWindowSettingsElement = "WindowSettings";

        /// <summary>
        /// Xml element name of the Views settings section
        /// </summary>
        private const string mViewsSettingsElement = "Views";

        /// <summary>
        /// Window manager interface. Handles communication with the loaded window manager
        /// (Docking Windows or the Static Windows)
        /// </summary>
        private IWindowManager mWindowManager;

        /// <summary>
        /// Most recently used file menu
        /// </summary>
        private MruStripMenuInline mMruMenu;

        /// <summary>
        /// Map of views to the content interface of the view.
        /// Fast lookup of a view name to its interface
        /// </summary>
        private Dictionary<string, IContent> mContents = new Dictionary<string, IContent>();

        //the Simulation Views
        private IContent mCodeContent;
        private IContent mRegistersContent;
        private IContent mOutputContent;
        private IContent mWatchContent;
        private IContent mStackContent;
        private IContent mDataCacheContent;
        private IContent mInstructionCacheContent;
        private IContent mUnifiedCacheContent;
        private IContent mPluginsUIViewContent;

		/// <summary>
		/// Data structure for managing memory view indices.
		/// </summary>
		private FirstAvailableIndexDispenser memoryViewIndexDispenser = new FirstAvailableIndexDispenser();

        /// <summary>
        /// Flag indicating that the simulation is running
        /// </summary>
        //private bool mSimulatorRunning;

        /// <summary>
        /// Preferences loaded from the settings file at startup
        /// </summary>
        private ARMPreferences _preferences;

        /// <summary>
        /// Full path:filename of application settings file. Set in constructor
        /// </summary>
        private readonly string mSettingsFileLocation;

        /// <summary>
        /// Arguments passed on command line to application
        /// </summary>
        private ARMSim.CommandLine.ARMSimArguments mParsedArgs;

        public ARMSimForm(ARMSim.CommandLine.ARMSimArguments parsedArgs)
        {
            //save the commandline arguments
            mParsedArgs = parsedArgs;

            // create a new instance of the simulator engine. (Must be done before call to InitializeComponent).
            mJM = new ApplicationJimulator(mParsedArgs);

            string layoutDirectory = System.Configuration.ConfigurationManager.AppSettings["LayoutFileLocation"];
            if (layoutDirectory == null)
            {
                //get the application data directory from the OS. Build a full path and filename to the
                //application settings xml document.
                layoutDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                layoutDirectory = Path.Combine(layoutDirectory, "ARMSim");
                if (!Directory.Exists(layoutDirectory))
                {
                    Directory.CreateDirectory(layoutDirectory);
                }
            }

            //layout filename is the layout directory combined with the layout filename
            mSettingsFileLocation = Path.Combine(layoutDirectory, mSettingsFilename);
            ARMPluginInterfaces.Utils.OutputDebugString("Layout file:{0}\n", mSettingsFileLocation);

            // Reduce the amount of flicker that occurs when windows are redocked within
            // the container. As this prevents unsightly backcolors being drawn in the
            // WM_ERASEBACKGROUND that seems to occur.
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            string dockingWindowStyle = System.Configuration.ConfigurationManager.AppSettings["DockingWindowStyle"];

			string windowManagerAssemblyName;
			switch (dockingWindowStyle)
			{
                case "StaticWindows":
                    windowManagerAssemblyName = "StaticWindows.dll";
                    break;
				case "DockingWindows":
					windowManagerAssemblyName = "DockingWindows.dll";
					break;
				case "DockingWindows2":
                default:
					windowManagerAssemblyName = "DockingWindows2.dll";
					break;	
			}

			string dpath = Path.Combine(System.Windows.Forms.Application.StartupPath, windowManagerAssemblyName);
            System.Reflection.Assembly windowAssembly = System.Reflection.Assembly.LoadFrom(dpath);
            foreach (Type type in windowAssembly.GetTypes())
            {
                //ARMPluginInterfaces.Utils.OutputDebugString("Found type:" + type.ToString());
                if (type.GetInterface(typeof(IWindowManager).ToString(), false) != null)
                {
                    //if(type.GetCustomAttributes(typeof(PlugDisplayNameAttribute),false).Length!=1)
                    mWindowManager = (Activator.CreateInstance(type) as IWindowManager);
                    break;
                }//if
            }//foreach


            
			Control WindowManagerContainer = mWindowManager.GenerateManagerControl();

			WindowManagerContainer.Dock = DockStyle.Fill;
			//Add the window manager container first (before initializing the rest of the window)
			//so it will be on the bottom (apparently Controls.add behaves like a stack)
			this.Controls.Add(WindowManagerContainer);
			InitializeComponent();

            // Setup custom config handling
            mWindowManager.SaveCustomConfig += OnSaveConfig;
            mWindowManager.LoadCustomConfig += OnLoadConfig;

			mWindowManager.OnMemoryViewClosed += OnMemoryViewClosed;
			

            //initialize the MRU file list
            mMruMenu = new MruStripMenuInline(fileToolStripMenuItem, menuFileRecentFile, OnMenuFileMru);

        }//ARMSimForm ctor

        private IContent CodeContent { get { return mCodeContent; } }
        private IContent RegistersContent { get { return mRegistersContent; } }
        private IContent OutputContent { get { return mOutputContent; } }
        private IContent WatchContent { get { return mWatchContent; } }
        private IContent StackContent { get { return mStackContent; } }
        private IContent DataCacheContent { get { return mDataCacheContent; } }
        private IContent InstructionCacheContent { get { return mInstructionCacheContent; } }
        private IContent UnifiedCacheContent { get { return mUnifiedCacheContent; } }
        private IContent PluginsUIViewContent { get { return mPluginsUIViewContent; } }

        private CodeView CodeView { get { return this.CodeContent.Control as CodeView; } }
        private OutputView OutputView { get { return this.OutputContent.Control as OutputView; } }
        private PluginsUIView PluginsUIView { get { return this.PluginsUIViewContent.Control as PluginsUIView; } }
        private WatchView WatchView { get { return this.WatchContent.Control as WatchView; } }

        private void OnSaveConfig(XmlWriter xmlOut)
        {
            try
            {
                //Add a node for application settings
                xmlOut.WriteStartElement(mApplicationSettingsElement);

                //Create a new node for the window size and position settings
                xmlOut.WriteStartElement(mMainFormElement);

                xmlOut.WriteStartElement(mWindowSettingsElement);
                xmlOut.WriteAttributeString("Location", this.Bounds.ToString());
                xmlOut.WriteAttributeString("State", this.WindowState.ToString());
                xmlOut.WriteEndElement();

                xmlOut.WriteEndElement();

                //iterate thru the views, giving each a chance to save its state to the save document
                xmlOut.WriteStartElement(mViewsSettingsElement);
                foreach (IContent c in this.mContents.Values)
                {
                    (c.Control as IViewXMLSettings).saveState(xmlOut);
                }//foreach
                xmlOut.WriteEndElement();

                //save the MRU file list to save document
                mMruMenu.saveState(xmlOut);

                //save the preferences
                _preferences.saveState(xmlOut);
                xmlOut.WriteEndElement();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                //if an exception occurs, simply save nothing to the file
            }
        }//OnSaveConfig

        private void loadMainFormElements(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                xmlIn.Read();
                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement)
                        break;

                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        //if the MRU list , hand off
                        if (xmlIn.Name == mWindowSettingsElement)
                        {
                            string windowLocation = xmlIn.GetAttribute("Location");
                            string windowState = xmlIn.GetAttribute("State");

                            //State="Normal
                            if (windowState == "Maximized")
                            {
                                this.WindowState = FormWindowState.Maximized;
                            }//if
                            else
                            {
                                //{X=81,Y=93,Width=912,Height=607}
                                Match m = new Regex(@"^{X=(?<xpos>\d+),Y=(?<ypos>\d+),Width=(?<width>\d+),Height=(?<height>\d+)}", RegexOptions.Compiled).Match(windowLocation);
                                this.Location = new Point(Convert.ToInt32(m.Result("${xpos}")), Convert.ToInt32(m.Result("${ypos}")));
                                this.Size = new Size(Convert.ToInt32(m.Result("${width}")), Convert.ToInt32(m.Result("${height}")));
                            }//else
                        }//if
                    }//if
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                //System.Console.WriteLine(ex.Message);
            }
        }//loadMainFormElements


        private void LoadViewSettings(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement) break;
                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        //if the MRU list , hand off
                        if (xmlIn.Name.StartsWith(MemoryView.Prefix))
                        {
							//TODO investigate and disprove following comments.
                            //only reload a memoryview if docking windows active.
                            //This limitation because of a tabcontrol bug in mono
                            if (mWindowManager.IsDockingWindows)
                            {

                                using (MemoryView mv = new MemoryView(mJM))
                                {
                                    mv.LoadFromXML(xmlIn);
                                    if (!memoryViewIndexDispenser.ReserveIndex(mv.Index))
                                        mv.Index = memoryViewIndexDispenser.GetIndex();
                                    this.CreateMemoryViewContent(mv);
                                }
                            }
                        }
                        else if (xmlIn.Name == CodeView.ViewName)
                        {
                            this.CodeView.LoadFromXML(xmlIn.ReadSubtree());
                        }
                        else
                        {
                            foreach (IContent c in this.mContents.Values)
                            {
                                if (c.Title == xmlIn.Name)
                                {
                                    (c.Control as IViewXMLSettings).LoadFromXML(xmlIn.ReadSubtree());
                                    break;
                                }
                            }//foreach
                        }//else
                    }//if
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
            }
        }//loadViewSettings

        private void CreateMemoryViewContent(MemoryView mv)
        {
            mv.Text = mv.ViewName;
            //if (!mWindowManager.IsDockingWindows)
            //{
            //    Button btnClose = new Button();
            //    btnClose.Location = new System.Drawing.Point(146, 14);
            //    btnClose.Name = "btnClose";
            //    btnClose.Size = new System.Drawing.Size(69, 23);
            //    btnClose.TabIndex = 11;
            //    btnClose.Text = "Close";
            //    btnClose.UseVisualStyleBackColor = true;
            //    btnClose.Click += delegate(object sender2, EventArgs e2) { mWindowManager.RemoveMemoryView(mv.Text); mContents.Remove(mv.Text); };
            //    mv.Controls.Add(btnClose);
            //}//if
            mv.ResolveSymbolHandler = fResolveSymbolHandler;
            IContent content = mWindowManager.CreateMemoryView(mv,mv.Index);
            mContents.Add(mv.Text, content);
        }

		private void OnMemoryViewClosed(Control memoryView, int memoryViewIndex)
		{
			MemoryView mv = (MemoryView)memoryView;
			memoryViewIndexDispenser.ReturnIndex(mv.Index);
			mContents.Remove(mv.Text);
		}

        private void OnLoadConfig(XmlReader xmlDockingConfig)
        {
            try
            {
                // We are expecting our custom element to be the current one
                if (xmlDockingConfig.NodeType != XmlNodeType.Element || xmlDockingConfig.Name != mApplicationSettingsElement)
                    return;

                XmlReader xmlIn = xmlDockingConfig.ReadSubtree();
                xmlIn.MoveToContent();
                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement)
                        break;

                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        //if (xmlIn.Name == _JM.PluginManagerTagName)
                        //{
                        //    _JM.LoadPluginSettings(xmlIn.ReadSubtree());
                        //}
                        //if the MRU list , hand off
                        if (xmlIn.Name == MruStripMenu.TagName)
                        {
                            mMruMenu.LoadFromXML(xmlIn.ReadSubtree());
                            xmlIn.ReadEndElement();
                            continue;
                        }
                        else if (xmlIn.Name == mMainFormElement)
                        {
                            loadMainFormElements(xmlIn.ReadSubtree());

                            if(xmlIn.NodeType == XmlNodeType.EndElement)
                                xmlIn.ReadEndElement();

                            continue;
                        }
                        //check if the preferences
                        else if (xmlIn.Name == ARMPreferences.TagName)
                        {
                            _preferences.LoadFromXML(xmlIn.ReadSubtree());
                            xmlIn.ReadEndElement();
                            continue;
                        }
                        else if (xmlIn.Name == mViewsSettingsElement)
                        {
                            LoadViewSettings(xmlIn.ReadSubtree());
                            xmlIn.ReadEndElement();
                            continue;
                        }
                    }
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }//try
            catch (Exception ex)
            {
                //if an exception occurs, simply accept the preferences defaults
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                _preferences.defaultSettings();
            }
        }//OnLoadConfig

        /// <summary>
        /// Callback attempts to resolve a given symbol to an address.
        /// checks if the symbol is a register first and if so, returns that registers
        /// value. Otherwise checks if it is a label and if so returns that labels offset.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool fResolveSymbolHandler(string symbol, ref uint address)
        {
            uint reg = 0;
            if (GeneralPurposeRegisters.isRegister(symbol, ref reg))
            {
                address = mJM.GPR[reg];
                return true;
            }

            if (symbol.StartsWith("0x") || symbol.StartsWith("0X")) {
                uint val = 0;
                if (UInt32.TryParse(symbol.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out val)) {
                    address = val;
                    return true;
                }
            }
            if (isLabel(symbol, ref address)) return true;

            return false;
        }//fResolveSymbolHandler

        /// <summary>
        /// Function checks if a given string is a label defined in the currently loaded program space.
        /// if it is, returns the address
        /// </summary>
        /// <param name="label"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private bool isLabel(string label, ref uint address)
        {
            if (mJM.CodeLabels == null)
                return false;

            ARMPluginInterfaces.IAddressLabelPair addressPair = mJM.CodeLabels.LabelToAddress(label);
            if(addressPair == null)
                return false;

            address = addressPair.Address;
            return true;
        }//isLabel

        /// <summary>
        /// A file was selected from the MRU file list.
        /// Same as opening a new file
        /// </summary>
        /// <param name="number"></param>
        /// <param name="filename"></param>
        private void OnMenuFileMru(int number, string filename)
        {
            LoadAssemblerFile(new string[] { filename });
        }//OnMenuFileMru

        /// <summary>
        /// Load a new program into the simulator. This function will compile it, load it and setup the
        /// simulator for a run.
        /// </summary>
        /// <param name="fileNames"></param>
        private void LoadAssemblerFile(IList<string> fileNames)
        {
            //clear the output view and display the loading message
            this.OutputView.ClearText();

            //add the loaded program into the MRU list
            mMruMenu.AddFiles(fileNames);

            IList<string> fileListCopy = new List<string>(fileNames);
            if (!mJM.Load(fileListCopy))
            {
                //load the codeview with the program and the errors
                this.CodeView.InitErrors(mJM.ArmAssemblerInstance.FileInfoTable);

                //make sure the simulator is NOT in a valid state
                //mJM.ValidLoadedProgram = false;

                //If assembler errors were detected then bring the OutputView to the front
                //so it is obvious to the user what has happened.The outputview has the assembler
                //error messages listed. - dale
                mWindowManager.BringToFront(this.OutputContent);
                //this.OutputContent.BringToFront();

            }
            else
            {
                //good compile. Load the codeView with the program and code
                mJM.CreatingCodeView = true;
                this.CodeView.InitNormal(mJM.ArmAssemblerInstance.FileInfoTable);
                this.ResetControl();
                mJM.CreatingCodeView = false;
            }//else

            this.EnableToolbar();
            this.UpdateControl();

        }//loadAssemblerFile

        /// <summary>
        /// This function checks if the simulator is ready to execute another instruction.
        /// they are:
        /// 1) ValidLoadedProgram - has the simulator been loaded with a memory image
        /// 2) Stopped - is the simulator already running a program
        /// 3) At swi 0x11 - is the simulator at an SWI instruction.
        /// </summary>
        /// <returns></returns>
        private bool isSimulatorReady()
        {
            return (mJM.ValidLoadedProgram && mSimulationStopped && !mJM.isCurrentOpCodeSWIExit());
        }

        /// <summary>
        /// This function enables or disables toobar buttons depending on simulator state.
        /// </summary>
        private void EnableToolbar()
        {
            bool simulatorReady = isSimulatorReady();
            bool valid = mJM.ValidLoadedProgram;

            toolStrip1.Items[0].Enabled = simulatorReady;                   //step into
            toolStrip1.Items[1].Enabled = simulatorReady;                   //step over
            toolStrip1.Items[2].Enabled = valid;                            //stop
            toolStrip1.Items[3].Enabled = simulatorReady;                   //run
            toolStrip1.Items[4].Enabled = mSimulationStopped;                      //restart
            toolStrip1.Items[6].Enabled = mSimulationStopped && (mMruMenu.MostRecentFile != null);//reload
        }//EnableToolbar

        /// <summary>
        /// Called when the simulation has been reset.
        /// </summary>
        private void ResetControl()
        {
            try
            {
                foreach (IContent c in this.mContents.Values)
                    (c.Control as IView).resetView();

                UpdateControl();
                this.CodeView.Focus();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception caught:" + ex.Message);
            }

        }//ResetControl

        /// <summary>
        /// Called when it is time to update the views because the state of the
        /// simulation has changed.
        /// </summary>
        private void UpdateControl()
        {
            try
            {
                foreach (IContent c in this.mContents.Values)
                    (c.Control as IView).updateView();

                EnableToolbar();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception caught:" + ex.Message);
            }
        }//UpdateControl

        //Only show splash screen in release mode
        [System.Diagnostics.Conditional("ShowSplashScreen")]
        private void ShowSplashScreen()
        {
            new SplashScreen().ShowDialog(this);
        }

        private IContent CreateAndAddContent(Control view, string title)
        {
            IContent content = mWindowManager.CreateContent(view, title);
            mContents.Add(title, content);
            return content;
        }

        /// <summary>
        /// Called when the main form is loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ARMSimForm_Load(object sender, EventArgs e)
        {
            try
            {
                ShowSplashScreen();

                //the codeView requires a post create method so that the default graphic elements
                //can be set.
                CodeView codeView = new CodeView(mJM);
                mCodeContent = CreateAndAddContent(codeView, CodeView.ViewName);

                //creates the output view. Set the code selection callback
                //set the character output handler for both the simulator engine and the codeview
                //to the output view. This will direct the codeview output to the outputview and any
                //writes to standard output from the sim engine as well.
                OutputView outputView = new OutputView(mJM);
                mOutputContent = CreateAndAddContent(outputView, OutputView.ViewName);

                //_JM.Stdout.OnCharOutput = outputView.WriteStdout;
                mJM.ConsoleOutput = outputView.WriteConsole;
                codeView.CharOutputHandler = outputView.WriteConsole;

                WatchView watchView = new WatchView(mJM, getCodeLabels, fResolveSymbolHandler);
                mWatchContent = CreateAndAddContent(watchView, WatchView.ViewName);
                
                RegistersView registersView = new RegistersView(mJM);
                mRegistersContent = CreateAndAddContent(registersView, RegistersView.ViewName);
                mRegistersContent.ComputeWidthBasedOnFont += registersView.ComputeWidthBasedOnFont;

                DataCacheView dataCacheView = new DataCacheView(mJM);
                InstructionCacheView instructionCacheView = new InstructionCacheView(mJM);
                UnifiedCacheView unifiedCacheView = new UnifiedCacheView(mJM);
                mDataCacheContent = CreateAndAddContent(dataCacheView, DataCacheView.ViewName);
                mInstructionCacheContent = CreateAndAddContent(instructionCacheView, InstructionCacheView.ViewName);
                mUnifiedCacheContent = CreateAndAddContent(unifiedCacheView, UnifiedCacheView.ViewName);

                StackView stackView = new StackView(mJM);
                mStackContent = CreateAndAddContent(stackView, StackView.ViewName);
                mStackContent.ComputeWidthBasedOnFont += stackView.ComputeWidthBasedOnFont;

                PluginsUIView pluginsUIView = new PluginsUIView();
                mPluginsUIViewContent = CreateAndAddContent(pluginsUIView, PluginsUIView.ViewName);

                //stackView.OnRecalLayout += RecalcLayout;
                //registersView.OnRecalLayout += RecalcLayout;
                mWindowManager.CreateViews(mCodeContent, mOutputContent, mWatchContent, mRegistersContent,
                                            mDataCacheContent, mInstructionCacheContent, mUnifiedCacheContent,
                                            mStackContent, mPluginsUIViewContent);


                //create the application preferences object
                _preferences = new ARMPreferences();

                //attempt to load the docking manager and application settings from saved xml file
                ARMPluginInterfaces.Utils.OutputDebugString("Layout file:{0}\n", mSettingsFileLocation);
                if (File.Exists(mSettingsFileLocation))
                    mWindowManager.LoadConfigFromFile(mSettingsFileLocation);

                //always enable the SWI instructions plugin
                _preferences.PluginPreferences.EnableSWIExtendedInstructions();
                mJM.ARMPreferences = _preferences;

                //if the cache is disabled in the preferences, then make sure the CacheView
                //is not visible. The menu Popup callback will make sure the cacheView option
                //is disabled.
                this.EnableCaches();

                mJM.InitPlugins(this.PluginsUIView, this.OutputView);
                if (mParsedArgs.Files != null && mParsedArgs.Files.Length > 0)
                {
                    this.LoadAssemblerFile(mParsedArgs.Files);
                }

                //update all the views
                this.UpdateControl();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Error in loadconfig:" + ex.Message);
            }
        }

        /// <summary>
        /// Return the code labels
        /// </summary>
        /// <returns></returns>
        private CodeLabels getCodeLabels()
        {
            return mJM.CodeLabels;
        }

        private void onExit()
        {
			
            mFormClosing = true;
            TerminateInput();

            mJM.HaltSimulation();

            mWindowManager.SaveConfigToFile(mSettingsFileLocation);
            //close all user and standard file handles.
            mJM.Shutdown();
			this.Close();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onExit();
        }

        private void TerminateInput()
        {
            foreach (IContent c in this.mContents.Values)
            {
                (c.Control as IView).TerminateInput();
            }
            //mJM.SetApplicationClosing();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog(this);
        }

        private void handleViewState(IContent content, ToolStripMenuItem tsmi)
        {
            mWindowManager.ShowContent(content, tsmi.Checked);
            if (tsmi.Checked)
                (content.Control as IView).updateView();

        }//handleViewState

        private void registersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(RegistersContent, (sender as ToolStripMenuItem));
        }
        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(OutputContent, (sender as ToolStripMenuItem));
        }
        private void stackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(StackContent, (sender as ToolStripMenuItem));
        }
        private void watchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            handleViewState(WatchContent, (sender as ToolStripMenuItem));
        }
        private void dataCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(DataCacheContent, (sender as ToolStripMenuItem));
        }
        private void instructionCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(InstructionCacheContent, (sender as ToolStripMenuItem));
        }
        private void unifiedCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(UnifiedCacheContent, (sender as ToolStripMenuItem));
        }
        private void PluginsUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleViewState(PluginsUIViewContent, (sender as ToolStripMenuItem));
        }

        private void memoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
			int viewIndex = memoryViewIndexDispenser.GetIndex();

            MemoryView mv = new MemoryView(mJM, viewIndex);
            this.CreateMemoryViewContent(mv);

        }//memoryToolStripMenuItem_Click

		private void resetLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mWindowManager.DefaultLayout();
		}

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mFormClosing)
                return;

            ToolStripDropDownItem tdd = sender as ToolStripDropDownItem;
            if (tdd == null)
                return;

            (tdd.DropDownItems[0] as ToolStripMenuItem).Checked = RegistersContent.IsShowing;
            (tdd.DropDownItems[1] as ToolStripMenuItem).Checked = OutputContent.IsShowing;
            (tdd.DropDownItems[2] as ToolStripMenuItem).Checked = StackContent.IsShowing;
            (tdd.DropDownItems[3] as ToolStripMenuItem).Checked = WatchContent.IsShowing;

            (tdd.DropDownItems[4] as ToolStripMenuItem).Checked = DataCacheContent.IsShowing;
            (tdd.DropDownItems[5] as ToolStripMenuItem).Checked = InstructionCacheContent.IsShowing;
            (tdd.DropDownItems[6] as ToolStripMenuItem).Checked = UnifiedCacheContent.IsShowing;

            (tdd.DropDownItems[4] as ToolStripMenuItem).Enabled = (DataCacheContent.Control as DataCacheView).CacheEnabled;
            (tdd.DropDownItems[5] as ToolStripMenuItem).Enabled = (InstructionCacheContent.Control as InstructionCacheView).CacheEnabled;
            (tdd.DropDownItems[6] as ToolStripMenuItem).Enabled = (UnifiedCacheContent.Control as UnifiedCacheView).CacheEnabled;

            (tdd.DropDownItems[7] as ToolStripMenuItem).Checked = PluginsUIViewContent.IsShowing;
        }//viewToolStripMenuItem_DropDownOpening

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = mMruMenu.MostRecentFile;
			openFileDialog.Filter = "All Recognized Formats (*.s, *.o)|*.s; *.o; *.a|Source files (*.s)|*.s|Object files (*.o)|*.o|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadAssemblerFile(new string[] { openFileDialog.FileName });
            }//if

        }//loadToolStripMenuItem_Click

        /// <summary>
        /// Reloads the last file(s) loaded into ARMSim
        /// First checks if a multi-file project was loaded last, if so reload the files
        /// If a single file, then reload that last file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TerminateInput();

            IList<string> files;
            if (mMruMenu.MultipleFilesLoaded)
            {
                files = mMruMenu.RecentMultipleFile;
            }
            else
            {
                List<string> localFiles = new List<string>();
                localFiles.Add(mMruMenu.MostRecentFile.Trim());
                if (string.IsNullOrEmpty(localFiles[0]))
                    return;

                files = localFiles;
            }
            LoadAssemblerFile(files);
        }//reloadToolStripMenuItem_Click

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ARMSim.Preferences.PreferencesForm.PreferencesForm pf = new ARMSim.Preferences.PreferencesForm.PreferencesForm(_preferences, mJM);
            if (pf.ShowDialog() == DialogResult.OK)
            {
                _preferences = pf.Preferences;
                mJM.DefineCache(_preferences.CachePreferences);
                mJM.DefinePlugins(_preferences.PluginPreferences);
                this.ResetControl();
            }//if
            this.EnableCaches();
        }//preferencesToolStripMenuItem_Click

        private void EnableCaches()
        {
            UnifiedCacheView unifiedCacheView = (this.UnifiedCacheContent.Control as UnifiedCacheView);
            InstructionCacheView instructionCacheView = (this.InstructionCacheContent.Control as InstructionCacheView);
            DataCacheView dataCacheView = (this.DataCacheContent.Control as DataCacheView);

            if (_preferences.CachePreferences.UnifiedCache)
            {
                unifiedCacheView.CacheEnabled = _preferences.CachePreferences.DataCachePreferences.Enabled;
                instructionCacheView.CacheEnabled = false;
                dataCacheView.CacheEnabled = false;
            }
            else
            {
                unifiedCacheView.CacheEnabled = false;
                instructionCacheView.CacheEnabled = _preferences.CachePreferences.InstructionCachePreferences.Enabled;
                dataCacheView.CacheEnabled = _preferences.CachePreferences.DataCachePreferences.Enabled;
            }
            mWindowManager.ShowContent(this.InstructionCacheContent, instructionCacheView.CacheEnabled && InstructionCacheContent.IsShowing);
            mWindowManager.ShowContent(this.DataCacheContent, dataCacheView.CacheEnabled && DataCacheContent.IsShowing);
            mWindowManager.ShowContent(this.UnifiedCacheContent, unifiedCacheView.CacheEnabled && UnifiedCacheContent.IsShowing);

        }//enableCaches

        private void stepStart()
        {
            try
            {
                //this.CodeView.stepStart();
                foreach (IContent c in this.mContents.Values)
                {
                    (c.Control as IView).stepStart();
                }
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception caught:" + ex.Message);
                //menuItem_Debug_Restart(null, null);
            }
        }//stepStart

        private void StepEnd()
        {
            try
            {
                foreach (IContent c in this.mContents.Values)
                    (c.Control as IView).stepEnd();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception caught:" + ex.Message);
            }
        }//StepEnd

        private void stepIntoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mJM.ValidLoadedProgram)
                return;

            //ignore this keystroke if sim is stopped
            if (!isSimulatorReady())
                return;

            mSimulationStopped = false;
            EnableToolbar();
            this.stepStart();

            mJM.Execute();									//execute the instruction
            Application.DoEvents();

            if (mFormClosing)
                return;

            mSimulationStopped = true;
            EnableToolbar();

            this.StepEnd();
            this.UpdateControl();							//and update the views.
        }//stepIntoToolStripMenuItem_Click

        private void stepOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mJM.ValidLoadedProgram)
                return;

            //ignore this keystroke if sim is stopped
            if (!isSimulatorReady())
                return;

            //if the current instruction is not a BL, then treat like a step into
            if (!mJM.isCurrentOpCodeBL())
            {
                stepIntoToolStripMenuItem_Click(sender, e);
                return;
            }//if

            //otherwise we are going to simulate a Run condition until the instruction after this one.
            uint address = (uint)(mJM.GPR.PC + (mJM.CPSR.tf ? 2 : 4));
            mJM.HaltRequested = false;
            do
            {
                mJM.Execute();
                Application.DoEvents();
                if (mFormClosing)
                    return;
            } while (mJM.GPR.PC != address && !mJM.Breakpoints.AtBreakpoint(mJM.GPR.PC) && !mJM.isCurrentOpCodeSWIExit());

            mSimulationStopped = true;
            EnableToolbar();

            this.StepEnd();
            this.UpdateControl();							//and update the views.
        }//stepOverToolStripMenuItem_Click

        private void RunUntilBreakpoint()
        {
            // Ignore this keystroke if sim is stopped
            if (!isSimulatorReady())
                return;

            mJM.StartSimulation();
            mSimulationStopped = false;
            mJM.HaltRequested = false;

            this.EnableToolbar();
            this.stepStart();

            //and Run the simulation until it reaches any breakpoint (including this one)
            do
            {
                mJM.Execute();
                Application.DoEvents();

                if (mFormClosing)
                    return;

            } while (!mJM.Breakpoints.AtBreakpoint(mJM.GPR.PC) && !mJM.isCurrentOpCodeSWIExit());

            if (mJM.isCurrentOpCodeSWIExit())
                mJM.HaltSimulation();

            mSimulationStopped = true;
            this.EnableToolbar();
            this.StepEnd();
            this.UpdateControl();

        }//RunUntilBreakpoint

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ignore this keystroke if sim is stopped
            if (!isSimulatorReady())
                return;

            mJM.InstructionCount = 0;
            mJM.OutputConsoleString("Execution starting ...\n");
            DateTime nowDT = DateTime.Now;

            RunUntilBreakpoint();

            //the form closing flag means the user has terminated the application by closing the main window.
            //We need to make sure no other user interface action occurs (exceptions will be thrown).
            if (mFormClosing)
                return;

            TimeSpan ts = DateTime.Now.Subtract(nowDT);
            mJM.OutputConsoleString("\nExecution ending, Instruction Count:{0} Elapsed Time:{1}\n", mJM.InstructionCount, ts.ToString());
            if (ts.Milliseconds > 0)
            {
                double dInstructions = (double)mJM.InstructionCount;
                double dElapsedTime = (double)ts.TotalSeconds;
                double rate = (dInstructions / dElapsedTime);
                mJM.OutputConsoleString("Instructions per second:{0}\n", ((uint)rate).ToString());
            }//if
        }//runToolStripMenuItem_Click

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mJM.ValidLoadedProgram)
                return;

            mJM.Restart();

            this.ResetControl();
        }//restartToolStripMenuItem_Click

        /// <summary>
        /// Stop the simulation.
        /// The user has clicked the stop button while the simulation was running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //notify all the views that the simulation is stopping.
            //This will ensure that any plugin that is waiting for input properly terminate.
            //ie the swi 0x6a function that reads from stdinput)
            TerminateInput();

            //halt the simulation in the sim engine.
            mJM.HaltSimulation();

        }//stopToolStripMenuItem_Click

        private void clearAllBreakpointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isSimulatorReady())
                return;

            mJM.Breakpoints.Clear();
            this.CodeView.updateView();
            this.OutputView.WriteLine("All breakpoints cleared");
            this.UpdateControl();
        }//clearAllBreakpointsToolStripMenuItem_Click

        private void addWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WatchView.AddWatchItem();
        }

        private void removeWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WatchView.DeleteCurrentWatchItem();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WatchView.ClearAllWatchItems();
        }

        /// <summary>
        /// Called when the Watch pulldown menu is opening. Here we will enable/disable
        /// the watch functions based on the state of the simulator engine.
        /// We don't want to enable watch funtions when the simulator is not ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mFormClosing)
                return;

            ToolStripDropDownItem tdd = sender as ToolStripDropDownItem;
            if (tdd == null)
                return;

            bool simulatorReady = isSimulatorReady();
            (tdd.DropDownItems[0] as ToolStripMenuItem).Enabled = simulatorReady;
            (tdd.DropDownItems[1] as ToolStripMenuItem).Enabled = simulatorReady;
            (tdd.DropDownItems[2] as ToolStripMenuItem).Enabled = simulatorReady;
        }//watchToolStripMenuItem_DropDownOpening

        private void loadMultipleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MultipleFileOpen mfo = new MultipleFileOpen(mMruMenu.RecentMultipleFile);
            if (mfo.ShowDialog(this) == DialogResult.OK)
            {
                LoadAssemblerFile(mfo.OpenFiles);
            }
        }//loadMultipleToolStripMenuItem_Click

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mJM.ValidLoadedProgram)
                return;

            mJM.ResetCache();
            this.UpdateControl();
        }//resetToolStripMenuItem_Click

        private void purgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mJM.ValidLoadedProgram)
                return;

            mJM.PurgeCache();
            this.UpdateControl();
        }//purgeToolStripMenuItem_Click

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CacheStatistics(mJM.InstructionCacheMemory, mJM.DataCacheMemory).ShowDialog(this);
        }//statisticsToolStripMenuItem_Click

        private void cacheToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mFormClosing)
                return;

            ToolStripDropDownItem tdd = sender as ToolStripDropDownItem;
            if (tdd == null)
                return;

            bool simulatorReady = isSimulatorReady();
            (tdd.DropDownItems[0] as ToolStripMenuItem).Enabled = simulatorReady;
            (tdd.DropDownItems[1] as ToolStripMenuItem).Enabled = simulatorReady;
            (tdd.DropDownItems[3] as ToolStripMenuItem).Enabled = simulatorReady;
        }//cacheToolStripMenuItem_DropDownOpening

        private void debugToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mFormClosing)
                return;

            ToolStripDropDownItem tdd = sender as ToolStripDropDownItem;
            if (tdd == null)
                return;

            bool simulatorReady = isSimulatorReady();
            (tdd.DropDownItems[0] as ToolStripMenuItem).Enabled = simulatorReady;//Run
            (tdd.DropDownItems[2] as ToolStripMenuItem).Enabled = simulatorReady;//Step-Into
            (tdd.DropDownItems[3] as ToolStripMenuItem).Enabled = simulatorReady;//Step-Over
            (tdd.DropDownItems[5] as ToolStripMenuItem).Enabled = mSimulationStopped;//Restart
            (tdd.DropDownItems[6] as ToolStripMenuItem).Enabled = mJM.ValidLoadedProgram;//Stop
            (tdd.DropDownItems[8] as ToolStripMenuItem).Enabled = simulatorReady;//Clear All Breakpoints
        }//debugToolStripMenuItem_DropDownOpening

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mFormClosing)
                return;

            ToolStripDropDownItem tdd = sender as ToolStripDropDownItem;
            if (tdd == null)
                return;

            bool simulatorReady = mSimulationStopped;
            for (int ii = 0; ii < tdd.DropDownItems.Count; ii++)
            {
                if (tdd.DropDownItems[ii] is ToolStripMenuItem)
                {
                    ToolStripMenuItem tsmi = tdd.DropDownItems[ii] as ToolStripMenuItem;
                    if(tsmi.Text != "E&xit")
                    {
                        tsmi.Enabled = simulatorReady;
                    }
                }
            }//for ii
        }//fileToolStripMenuItem_DropDownOpening

        /// <summary>
        /// We need to trap the Close button being pressed in the system menu.
        /// This appears to be the only way to capture that event.
        /// If so we will call some code to cleanup, specifically terminate
        /// any input loops and start shutting down the app.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //http://msdn.microsoft.com/en-us/library/ms646360(VS.85).aspx

            int WM_SYSCOMMAND = 0x0112;
            int SC_CLOSE = 0xf060;

            if (m.Msg == WM_SYSCOMMAND)
            {
                if ((int)m.WParam == SC_CLOSE)
                {
                    onExit();
                }
            }
            base.WndProc(ref m);
        }

    }//class ARMSimForm
}
