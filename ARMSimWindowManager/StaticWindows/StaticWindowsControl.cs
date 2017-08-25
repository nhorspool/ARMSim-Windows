using System;
using System.Collections.Generic;
using System.Text;

using ARMSimWindowManager;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace StaticWindows
{
    public class StaticWindowsControl : IWindowManager
    {
        //private Form mParent;
        //private Control mToolStripControl;
        private Panel mPanel;

        public event SaveConfigEventHandler SaveCustomConfig;
        public event LoadConfigEventHandler LoadCustomConfig;

		public event MemoryViewClosedHandler OnMemoryViewClosed;

        //private UpperPanelControl mUpperPanelControl;
        //private LowerPanelControl mLowerPanelControl;
        //private SplitContainer mSplitContainer;
        private StaticMainForm mStaticMainForm;

		/*
        public void Create(Form parent, Control toolStrip, Panel panel)
        {
            mParent = parent;
            mToolStripControl = toolStrip;
            mPanel = panel;
            //mPanel.Resize += mainPanel_Resize;
        }
		*/

		public Control GenerateManagerControl()
		{
			mPanel = new Panel();
			mPanel.Dock = DockStyle.Fill;
			return mPanel;
		}


		public void DefaultLayout()
		{
			/* ... unsupported ... */
		}

        public void CreateViews(IContent codeView, IContent outputView, IContent watchView, IContent registersView,
             IContent dataCacheView, IContent instructionCacheView, IContent unifiedCacheView,
             IContent stackView, IContent pluginUIControlsView)
        {
            mPanel.SuspendLayout();

            mStaticMainForm = new StaticMainForm(registersView, codeView, stackView,
                                                 dataCacheView, instructionCacheView, unifiedCacheView,
                                                 outputView, watchView, pluginUIControlsView);
            mStaticMainForm.SuspendLayout();
            mStaticMainForm.Dock = DockStyle.Fill;
            mPanel.Controls.Add(mStaticMainForm);

            mStaticMainForm.ResumeLayout(false);
            mPanel.ResumeLayout(false);
        }

        public bool IsDockingWindows { get { return false; } }

        public void LoadConfigFromFile(string settingsFile)
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    XPathDocument xmlIn = new XPathDocument(settingsFile);
                    XPathNavigator xpath = xmlIn.CreateNavigator();
                    XmlReader xmlReader = xpath.SelectSingleNode("//ARMSimApplicationSettings").ReadSubtree();
                    xmlReader.MoveToContent();

                    if (this.LoadCustomConfig != null)
                        LoadCustomConfig(xmlReader);

                    mStaticMainForm.LoadXML(xpath.SelectSingleNode("//" + StaticMainForm.XMLKEYNAME));
                }
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception in loading xml save file:" + ex.Message);
            }
        }

        public void SaveConfigToFile(string settingsFilename)
        {
            var encoding = new UTF8Encoding(false);
            bool onWindows = Environment.OSVersion.Platform != PlatformID.Unix &&
                             Environment.OSVersion.Platform != PlatformID.MacOSX;
            using (StreamWriter xout =
                new StreamWriter(new FileStream(settingsFilename, FileMode.Create), encoding))
            {
                var xsettings = new XmlWriterSettings { Encoding = encoding, Indent = true };
                if (!onWindows)
                    xsettings.NewLineChars = "\n";
                using (XmlWriter xmlOut = XmlWriter.Create(xout, xsettings))
                {
                    xmlOut.WriteStartElement("StaticWindows");

                    if (this.SaveCustomConfig != null)
                        SaveCustomConfig(xmlOut);

                    ////xmlOut.WriteStartElement("PluginUIWindow");
                    ////xmlOut.WriteAttributeString("PluginUIVisible", mPluginUIControlsForm.Visible.ToString());
                    ////xmlOut.WriteAttributeString("PluginUIPosition", mPluginUIControlsForm.Location.ToString());
                    ////xmlOut.WriteAttributeString("PluginUISize", mPluginUIControlsForm.Size.ToString());
                    ////xmlOut.WriteEndElement();

                    mStaticMainForm.SaveXML(xmlOut);
                    xmlOut.WriteEndElement();
                    xmlOut.Close();
                }
            }
        }

		//TODO Delete this method.
		//BB - 08/20/2014
		//The method is no longer part of the interface (and was never called by any other part of the program)
		//Currently the StaticWindows system has no option to close a memory view after it is created (so MemoryViews
		//will stack up forever). On the other hand, MemoryViews are not saved into the XML layout for some reason,
		//and since the StaticWindows manager is only for compatibility's sake, the issue might be moot.
		//The method here (and the RemoveMemoryView method in StaticMainForm) might be useful in the future
		//if such functionality is desired, so I'm leaving it in for now.
        public void RemoveMemoryView(string title)
        {

            mStaticMainForm.RemoveMemoryView(title);

            ARMPluginInterfaces.Utils.OutputDebugString("RemoveMemoryView Done");

        }

        public IContent CreateMemoryView(Control view, int memoryViewIndex)
        {
            mStaticMainForm.CreateMemoryView(view);
            return this.CreateContent(view, view.Text);
        }

        public IContent CreateContent(Control control, string title)
        {
            return new ContentControl(control, title);
        }

        public void BringToFront(IContent content)
        {
            mStaticMainForm.BringToFront(content.Control);
        }

        public void ShowContent(IContent content, bool show)
        {
            mStaticMainForm.ShowContent(content, show);
        }

    }//class StaticWindowsControl
}
