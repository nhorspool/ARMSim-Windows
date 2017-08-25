/* DockingWindows2Manager.cs
 * 
 * The Manager interface for the new Docking Windows implementation.
 * The underlying docking window library is called DockPanelSuite
 * and is available at http://dockpanelsuite.com/. The namespace
 * WeifenLuo.WinFormsUI.Docking used in the code below refers to the
 * DockPanelSuite library. 
 * 
 * 
 * B. Bird - 08/09/2014
*/

using System;
using System.Collections.Generic;
using System.Text;

using ARMSimWindowManager;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Xml.XPath;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;

namespace DockingWindows2
{

	public class DockingWindows2Manager: IWindowManager
	{

		private DockManagerControl dockManager;

		public event SaveConfigEventHandler SaveCustomConfig;
		public event LoadConfigEventHandler LoadCustomConfig;


		public event MemoryViewClosedHandler OnMemoryViewClosed;

		private Boolean loadingInProgress = false;

		public Control GenerateManagerControl()
		{
			dockManager = new DockManagerControl();
			return dockManager;
		}


		public void CreateViews(IContent codeView, IContent outputView, IContent watchView, IContent registersView,
			 IContent dataCacheView, IContent instructionCacheView, IContent unifiedCacheView,
			 IContent stackView, IContent pluginUIControlsView)
		{
			dockManager.RegisterViews((DockContentWrapper)codeView, (DockContentWrapper)outputView, (DockContentWrapper)watchView, (DockContentWrapper)registersView,
				 (DockContentWrapper)dataCacheView, (DockContentWrapper)instructionCacheView, (DockContentWrapper)unifiedCacheView,
				 (DockContentWrapper)stackView, (DockContentWrapper)pluginUIControlsView);
			dockManager.DefaultLayout();
		}

		public void DefaultLayout()
		{
			dockManager.DefaultLayout();
		}



		public bool IsDockingWindows { get { return true; } }

		public void LoadConfigFromFile(string settingsFilename)
		{
			loadingInProgress = true;
			//Back up the current layout (since if there is an error while loading, the dock manager
			//might be rendered useless).
			DockManagerControl.SavedLayout backupLayout = dockManager.SaveLayout();
			try
			{
				if (File.Exists(settingsFilename))
				{
					XPathDocument xmlIn = new XPathDocument(settingsFilename);
					XPathNavigator xpath = xmlIn.CreateNavigator();
					//TODO The "ARMSIMApplicationSettings" key is probably incorrect (for symmetry with the save method, it should probably be "DockingWindows2")
					XmlReader xmlReader = xpath.SelectSingleNode("//ARMSimApplicationSettings").ReadSubtree();
					xmlReader.MoveToContent();

					if (this.LoadCustomConfig != null)
						LoadCustomConfig(xmlReader);

					//TODO
					//mStaticMainForm.LoadXML(xpath.SelectSingleNode("//" + StaticMainForm.XMLKEYNAME));
					if (!WeifenLuo.WinFormsUI.Docking.Win32Helper.IsRunningOnMono)
					{
                        var xmlReader1 = xpath.SelectSingleNode("//DockingLayout");
                        if (xmlReader1 != null)
                        {
                            xmlReader = xmlReader1.ReadSubtree();
                            xmlReader.MoveToContent();
                            DockManagerControl.SavedLayout layout = DockManagerControl.SavedLayout.LoadFromXML(xmlReader);
                            dockManager.LoadLayout(layout);
                        }
					}
				}
			}
			catch (Exception ex)
			{
				ARMPluginInterfaces.Utils.OutputDebugString("Exception in loading xml save file:" + ex.Message);
				//If there was some kind of error in the loading process, revert to the backed up layout.
				dockManager.LoadLayout(backupLayout);
			}
			loadingInProgress = false;
		}

		public void SaveConfigToFile(string settingsFilename)
		{
            var encoding = new UTF8Encoding(false);
            try {
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
                        xmlOut.WriteStartDocument();

                        xmlOut.WriteStartElement("DockingWindows2");

                        if (this.SaveCustomConfig != null)
                            SaveCustomConfig(xmlOut);

                        //Saving the layout causes problems on Mono
                        if (onWindows) {
                            xmlOut.WriteStartElement("DockingLayout");
                            DockManagerControl.SavedLayout layout = dockManager.SaveLayout();
                            layout.SaveToXML(xmlOut);
                            xmlOut.WriteEndElement();
                        }

                        xmlOut.WriteEndElement();
                        xmlOut.Close();
                    }
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine("Error writing XML settings file:\n{0}", e);
            }


		}

		public IContent CreateMemoryView(Control view, int memoryViewIndex)
		{
			//mStaticMainForm.CreateMemoryView(view);
			MemoryViewWrapper c = new MemoryViewWrapper(view, view.Text, memoryViewIndex);
			c.FormClosed += MemoryViewClosedHandler;
			//If we're currently loading an XML file, do not attempt to display the new memory view
			//Otherwise, display it.
			dockManager.AddMemoryView(c,!loadingInProgress);
			return c;
		}
		private void MemoryViewClosedHandler(object sender, FormClosedEventArgs e){
			MemoryViewWrapper c = (MemoryViewWrapper)sender;
			if (this.OnMemoryViewClosed != null)
				OnMemoryViewClosed(c.Control, c.Index);
		}

		public IContent CreateContent(Control control, string title)
		{
			return new DockContentWrapper(control, title);
		}

		public void BringToFront(IContent content)
		{
			//mStaticMainForm.BringToFront(content.Control);
			((DockContentWrapper)content).BringToFront();
		}

		public void ShowContent(IContent content, bool show)
		{
			content.IsShowing = show;
		}


	}
}
