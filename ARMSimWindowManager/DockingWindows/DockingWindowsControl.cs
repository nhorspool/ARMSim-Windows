using System;
using System.Collections.Generic;
using System.Text;

using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;

using ARMSimWindowManager;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using System.IO;

namespace DockingWindows
{

    public class DockingWindowsControl : IWindowManager
    {
        private DockingManager mManager;			//docking manager
        private ImageList mInternalImages;
        //private Dictionary<string, IContent> mContents = new Dictionary<string, IContent>();

        public event SaveConfigEventHandler SaveCustomConfig;
        public event LoadConfigEventHandler LoadCustomConfig;

		public event MemoryViewClosedHandler OnMemoryViewClosed;
		/*
        public void Create(Form parent, Control toolStrip, Panel codeView)
        {
            mManager = new DockingManager(parent, VisualStyle.IDE2005);
            mManager.InnerControl = codeView;
            mManager.OuterControl = toolStrip;
            mManager.LoadCustomConfig += OnLoadConfig;
            mManager.SaveCustomConfig += OnSaveConfig;

            mInternalImages = ResourceHelper.LoadBitmapStrip(this.GetType(),
                                "DockingWindows.Resources.SampleImages.bmp",
                                new Size(16, 16),
                                new Point(0, 0));

        }
		 * */
		public Control GenerateManagerControl()
		{
			Panel containerPanel = new Panel();
			containerPanel.Dock = DockStyle.Fill;

			Panel insidePanel = new Panel();
			insidePanel.Dock = DockStyle.Fill;

			mManager = new DockingManager(containerPanel, VisualStyle.IDE2005);
			mManager.InnerControl = insidePanel;
			mManager.OuterControl = new Panel();
			mManager.LoadCustomConfig += OnLoadConfig;
			mManager.SaveCustomConfig += OnSaveConfig;

			mInternalImages = ResourceHelper.LoadBitmapStrip(this.GetType(),
								"DockingWindows.Resources.SampleImages.bmp",
								new Size(16, 16),
								new Point(0, 0));
			return containerPanel;
		}

        public void SaveConfigToFile(string settingsFilename)
        {
            mManager.SaveConfigToFile(settingsFilename);
        }

        public void LoadConfigFromFile(string settingsFile)
        {
            try
            {
                if (File.Exists(settingsFile))
                    mManager.LoadConfigFromFile(settingsFile);
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception in loading xml save file:" + ex.Message);
            }
        }

		public void DefaultLayout()
		{
			/* ... unsupported ... */
		}

        public void CreateViews(IContent codeView, IContent outputView, IContent watchView, IContent registersView,
                 IContent dataCacheView, IContent instructionCacheView, IContent unifiedCacheView,
                 IContent stackView, IContent pluginsUIView)
        {
			/* BB - 08/10/2014 -
			 * The original position of the codeView was as the 'background' which occupied
			 * whatever space wasn't used by the docked windows. This approach broke somehow
			 * during the interface change, so now the codeView is treated as another docking
			 * window. It's not a high priority to fix the problem since this docking interface
			 * is being deprecated.
			 */
            /*
			codeView.Control.Dock = DockStyle.Fill;
            mManager.InnerControl.Controls.Add(codeView.Control);
			*/
			//The codeView is added at the bottom of this method.

            Content c = mManager.Contents.Add(registersView.Control, registersView.Control.Text, mInternalImages, 0);
            mManager.AddContentWithState(c, State.DockLeft);
            registersView.Tag = c;

            Content c1 = mManager.Contents.Add(outputView.Control, outputView.Control.Text, mInternalImages, 0);
            outputView.Tag = c1;
            Content c2 = mManager.Contents.Add(watchView.Control, watchView.Control.Text, mInternalImages, 0);
            watchView.Tag = c2;
            mManager.Contents.Add(c2);
            WindowContent wc = mManager.AddContentWithState(c1, State.DockBottom) as WindowContent;
            mManager.AddContentToWindowContent(c2, wc);

            //mContents.Add(outputView.Text, new ContentControl(outputView, outputView.Text, c1, mManager));
            //mContents.Add(watchView.Text, new ContentControl(watchView, watchView.Text, c2, mManager));

            c = mManager.Contents.Add(dataCacheView.Control, dataCacheView.Control.Text, mInternalImages, 0);
            dataCacheView.Tag = c;
            mManager.AddContentWithState(c, State.DockTop);
            mManager.HideContent(c);
            //mContents.Add(dataCacheView.Text, new ContentControl(dataCacheView, dataCacheView.Text, c, mManager));

            c = mManager.Contents.Add(instructionCacheView.Control, instructionCacheView.Control.Text, mInternalImages, 0);
            instructionCacheView.Tag = c;
            mManager.AddContentWithState(c, State.DockTop);
            mManager.HideContent(c);
            //mContents.Add(instructionCacheView.Text, new ContentControl(instructionCacheView, instructionCacheView.Text, c, mManager));

            c = mManager.Contents.Add(unifiedCacheView.Control, unifiedCacheView.Control.Text, mInternalImages, 0);
            unifiedCacheView.Tag = c;
            mManager.AddContentWithState(c, State.DockTop);
            mManager.HideContent(c);
            //mContents.Add(unifiedCacheView.Text, new ContentControl(unifiedCacheView, unifiedCacheView.Text, c, mManager));

            c = mManager.Contents.Add(stackView.Control, stackView.Control.Text, mInternalImages, 0);
            stackView.Tag = c;
            mManager.AddContentWithState(c, State.DockRight);
            mManager.ToggleContentAutoHide(c);
            //mContents.Add(stackView.Text, new ContentControl(stackView, stackView.Text, c, mManager));

            //pluginsUIView.Visible = false;
            c = mManager.Contents.Add(pluginsUIView.Control, pluginsUIView.Control.Text, mInternalImages, 0);
            pluginsUIView.Tag = c;
            mManager.AddContentWithState(c, State.DockTop);
            mManager.HideContent(c);
            //mContents.Add(pluginsUIView.Text, new ContentControl(pluginsUIView, pluginsUIView.Text, c, mManager));

			Content c0 = mManager.Contents.Add(codeView.Control, codeView.Control.Text, mInternalImages, 0);
			mManager.AddContentWithState(c0, State.Floating);
			codeView.Tag = c0;
        }//CreateViews

        public void OnSaveConfig(XmlWriter xmlOut)
        {
            if (this.SaveCustomConfig != null)
                SaveCustomConfig(xmlOut);
        }

        public void OnLoadConfig(XmlReader xmlDockingConfig)
        {
            if (this.LoadCustomConfig != null)
                LoadCustomConfig(xmlDockingConfig);
        }

        public IContent CreateMemoryView(Control view, int memoryViewIndex)
        {
            Content content = mManager.Contents.Add(view, view.Text, mInternalImages, 0);
            content.CloseOnHide = true;
            mManager.AddContentWithState(content, State.DockBottom);

            return new ContentControl(view, view.Text, mManager);
            //mContents.Add(view.Text, new ContentControl(view, view.Text, content, mManager));
        }

        public void RecalcLayout(Control content) { }

        public void BringToFront(IContent conten) { }


        public IContent CreateContent(Control control, string title)
        {
            return new ContentControl(control, title, mManager);
        }

        public bool IsDockingWindows { get { return true; } }

        public void ShowContent(IContent content, bool show)
        {
            Content c = mManager.Contents[content.Control.Text];
            if(c != null)
            {
                if(show)
                    mManager.ShowContent(c);
                else
                    mManager.HideContent(c);
            }

            content.IsShowing = show;

        }

    }//class DockingWindowsControl
}
