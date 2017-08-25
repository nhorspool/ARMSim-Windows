using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ARMSimWindowManager;
using System.Xml;
using System.Xml.XPath;

namespace StaticWindows
{
    public partial class StaticMainForm : UserControl
    {
        public const string XMLKEYNAME = "StaticMainForm";

        private TabPage mWatchViewTabPage;
        private TabPage mOutputViewTabPage;
        private TabPage mDataCacheTabPage;
        private TabPage mInstructionCacheTabPage;
        private TabPage mUnifiedCacheTabPage;

        private Form mPluginUIControlsForm;

        private IContent mRegistersView;
        private IContent mCodeView;
        private IContent mStackView;
        private IContent mDataCacheView;
        private IContent mInstructionCacheView;
        private IContent mUnifiedCacheView;
        private IContent mOutputView;
        private IContent mWatchView;
        private IContent mPluginUIControlsView;
        public StaticMainForm(IContent registersView, IContent codeView, IContent stackView,
                              IContent dataCacheView, IContent instructionCacheView, IContent unifiedCacheView,
                              IContent outputView, IContent watchView, IContent pluginUIControlsView)
        {
            mRegistersView = registersView;
            mCodeView = codeView;
            mStackView = stackView;
            mDataCacheView = dataCacheView;
            mInstructionCacheView = instructionCacheView;
            mUnifiedCacheView = unifiedCacheView;
            mOutputView = outputView;
            mWatchView = watchView;
            mPluginUIControlsView = pluginUIControlsView;
            this.DefaultSettings();

            InitializeComponent();

            mRegistersView.Control.Dock = DockStyle.Fill;
            mCodeView.Control.Dock = DockStyle.Fill;
            mStackView.Control.Dock = DockStyle.Fill;

            this.registersPanel.Controls.Add(mRegistersView.Control);
            this.splitContainer2.Panel2.Controls.Add(mCodeView.Control);
            this.stackPanel.Controls.Add(mStackView.Control);

            mDataCacheTabPage = new TabPage();
            mDataCacheTabPage.Text = mDataCacheView.Control.Text;
            mDataCacheView.Control.Dock = DockStyle.Fill;
            mDataCacheTabPage.Controls.Add(mDataCacheView.Control);

            mInstructionCacheTabPage = new TabPage();
            mInstructionCacheTabPage.Text = mInstructionCacheView.Control.Text;
            mInstructionCacheView.Control.Dock = DockStyle.Fill;
            mInstructionCacheTabPage.Controls.Add(mInstructionCacheView.Control);

            mUnifiedCacheTabPage = new TabPage();
            mUnifiedCacheTabPage.Text = mUnifiedCacheView.Control.Text;
            mUnifiedCacheView.Control.Dock = DockStyle.Fill;
            mUnifiedCacheTabPage.Controls.Add(mUnifiedCacheView.Control);

            this.tabControl2.TabPages.Add(mDataCacheTabPage);
            this.tabControl2.TabPages.Add(mInstructionCacheTabPage);
            this.tabControl2.TabPages.Add(mUnifiedCacheTabPage);


            mOutputViewTabPage = new TabPage();
            mOutputViewTabPage.Text = mOutputView.Control.Text;
            mOutputView.Control.Dock = DockStyle.Fill;
            mOutputViewTabPage.Controls.Add(mOutputView.Control);

            mWatchViewTabPage = new TabPage();
            mWatchViewTabPage.Text = mWatchView.Control.Text;
            mWatchView.Control.Dock = DockStyle.Fill;
            mWatchViewTabPage.Controls.Add(mWatchView.Control);

            this.tabControl1.TabPages.Add(mOutputViewTabPage);
            this.tabControl1.TabPages.Add(mWatchViewTabPage);

        }

        public void SaveXML(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(StaticMainForm.XMLKEYNAME);
            xmlOut.WriteStartElement("CacheWindow");
            xmlOut.WriteAttributeString("SplitterDistance", splitContainer2.SplitterDistance.ToString());
            xmlOut.WriteAttributeString("DataCacheVisible", mDataCacheView.IsShowing.ToString());
            xmlOut.WriteAttributeString("InstructionCacheVisible", mInstructionCacheView.IsShowing.ToString());
            xmlOut.WriteAttributeString("UnifiedCacheViewVisible", mUnifiedCacheView.IsShowing.ToString());
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("OutputWindow");
            xmlOut.WriteAttributeString("SplitterDistance", splitContainer1.SplitterDistance.ToString());
            xmlOut.WriteAttributeString("OutputViewVisible", mOutputView.IsShowing.ToString());
            xmlOut.WriteAttributeString("WatchViewVisible", mWatchView.IsShowing.ToString());
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("RegistersWindow");
            xmlOut.WriteAttributeString("Visible", mRegistersView.IsShowing.ToString());
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("StackWindow");
            xmlOut.WriteAttributeString("Visible", mStackView.IsShowing.ToString());
            xmlOut.WriteEndElement();

            if (mPluginUIControlsForm != null)
            {
                xmlOut.WriteStartElement("PluginUIWindow");
                xmlOut.WriteAttributeString("Visible", mPluginUIControlsForm.Visible.ToString());
                xmlOut.WriteAttributeString("Location", mPluginUIControlsForm.Location.ToString());
                xmlOut.WriteAttributeString("Size", mPluginUIControlsForm.Size.ToString());
                xmlOut.WriteEndElement();
            }
            xmlOut.WriteEndElement();
        }

        private void DefaultSettings()
        {
        }

        private static XmlReader SafeSelectSingleNode(XPathNavigator xpath, string path)
        {
            XPathNavigator node = xpath.SelectSingleNode(path);
            if (node == null)
                return null;
            return node.ReadSubtree();
        }

        public void LoadXML(XPathNavigator xpath)
        {
            this.DefaultSettings();
            if (xpath == null)
                return;

            XmlReader xmlReader = SafeSelectSingleNode(xpath, "//CacheWindow");
            if (xmlReader != null)
            {
                xmlReader.MoveToContent();
                int cacheSplitterDistance;
                if (int.TryParse(xmlReader.GetAttribute("SplitterDistance"), out cacheSplitterDistance))
                    this.splitContainer2.SplitterDistance = cacheSplitterDistance;

                bool visible;
                if (bool.TryParse(xmlReader.GetAttribute("DataCacheVisible"), out visible))
                    this.ShowContent(mDataCacheView, visible);
                if (bool.TryParse(xmlReader.GetAttribute("InstructionCacheVisible"), out visible))
                    this.ShowContent(mInstructionCacheView, visible);
                if (bool.TryParse(xmlReader.GetAttribute("UnifiedCacheViewVisible"), out visible))
                    this.ShowContent(mUnifiedCacheView, visible);

            }
            xmlReader = SafeSelectSingleNode(xpath, "//OutputWindow");
            if (xmlReader != null)
            {
                xmlReader.MoveToContent();
                int outputSplitterDistance;
                if (int.TryParse(xmlReader.GetAttribute("SplitterDistance"), out outputSplitterDistance))
                    this.splitContainer1.SplitterDistance = outputSplitterDistance;

                bool visible;
                if (bool.TryParse(xmlReader.GetAttribute("OutputViewVisible"), out visible))
                    this.ShowContent(mOutputView, visible);
                if (bool.TryParse(xmlReader.GetAttribute("WatchViewVisible"), out visible))
                    this.ShowContent(mWatchView, visible);

            }//if

            xmlReader = SafeSelectSingleNode(xpath, "//PluginUIWindow");
            if (xmlReader != null)
            {
                xmlReader.MoveToContent();
                bool visible;
                if (bool.TryParse(xmlReader.GetAttribute("Visible"), out visible))
                    this.ShowContent(mPluginUIControlsView, visible);

                string locationString = xmlReader.GetAttribute("Location");
                string sizeString = xmlReader.GetAttribute("Size");
                if (!string.IsNullOrEmpty(locationString) && !string.IsNullOrEmpty(sizeString))
                {
                    Point location;
                    if (PPoint.TryParse(locationString, out location))
                    {
                        mPluginUIControlsForm.Location = location;
                    }
                    Size size;
                    if (PSize.TryParse(sizeString, out size))
                    {
                        mPluginUIControlsForm.Size = size;
                    }
                }//if
            }//if

            xmlReader = SafeSelectSingleNode(xpath, "//RegistersWindow");
            if (xmlReader != null)
            {
                xmlReader.MoveToContent();
                bool visible;
                if (bool.TryParse(xmlReader.GetAttribute("Visible"), out visible))
                    this.ShowContent(mRegistersView, visible);

            }//if

            xmlReader = SafeSelectSingleNode(xpath, "//StackWindow");
            if (xmlReader != null)
            {
                xmlReader.MoveToContent();
                bool visible;
                if (bool.TryParse(xmlReader.GetAttribute("Visible"), out visible))
                    this.ShowContent(mStackView, visible);

            }//if

        }//LoadXML

        public void CreateMemoryView(Control view)
        {
            TabPage tabPageMemoryView = new TabPage();

            ContextMenuStrip cms = new ContextMenuStrip();
            tabPageMemoryView.ContextMenuStrip = cms;
            cms.Name = "contextMenuStrip1";
            cms.Size = new System.Drawing.Size(61, 4);
            cms.Opening += this.contextMenuStrip1_Opening;

            tabPageMemoryView.Name = view.Text;
            tabPageMemoryView.Padding = new System.Windows.Forms.Padding(3);
            tabPageMemoryView.TabIndex = 0;
            tabPageMemoryView.Text = view.Text;
            tabPageMemoryView.UseVisualStyleBackColor = true;
            view.Dock = DockStyle.Fill;
            tabPageMemoryView.Controls.Add(view);

            this.tabControl1.TabPages.Add(tabPageMemoryView);

            this.tabControl1.SelectedTab = tabPageMemoryView;
            splitContainer1.Panel2.PerformLayout();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();
            cms.Items.Add(new ToolStripMenuItem("&Close", null, menuItem_Close));

            //_graphicElements.Popup(cms, false);
        }

        private void menuItem_Close(Object sender, System.EventArgs e)
        {

            try
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Count:" + this.tabControl1.TabPages.Count.ToString());
                TabPage tp = this.tabControl1.SelectedTab;
                this.tabControl1.TabPages.Remove(tp);
                splitContainer1.Panel2.PerformLayout();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception removing memoryview tab:" + ex.Message);
            }
        }

        public void RemoveMemoryView(string title)
        {
            try
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Count:" + this.tabControl1.TabPages.Count.ToString());
                this.tabControl1.TabPages.RemoveByKey(title);
                //splitContainer1.Panel2.PerformLayout();
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Exception removing memoryview tab:" + ex.Message);
            }
        }

        private void splitContainer1_Panel1_Layout(object sender, LayoutEventArgs e)
        {
            this.stackPanel.Width = mStackView.ComputeWidthBasedOnFont();
            this.registersPanel.Width = mRegistersView.ComputeWidthBasedOnFont();
            this.registersPanel.Visible = mRegistersView.IsShowing;
            this.stackPanel.Visible = mStackView.IsShowing;

            bool anyVisible = mDataCacheView.IsShowing || mInstructionCacheView.IsShowing || mUnifiedCacheView.IsShowing;
            this.splitContainer2.Panel1Collapsed = !anyVisible;

        }

        public void BringToFront(Control view)
        {
            if (view == mWatchView.Control)
                mWatchViewTabPage.BringToFront();
            else if (view == mOutputView.Control)
                mOutputViewTabPage.BringToFront();
        }

        public void ShowContent(IContent content, bool show)
        {
            content.IsShowing = show;
            if (content == mWatchView)
            {
                if (show && !this.tabControl1.TabPages.Contains(mWatchViewTabPage))
                    this.tabControl1.TabPages.Add(mWatchViewTabPage);
                else if (!show && this.tabControl1.TabPages.Contains(mWatchViewTabPage))
                    this.tabControl1.TabPages.Remove(mWatchViewTabPage);
            }
            else if (content == mOutputView)
            {
                if (show && !this.tabControl1.TabPages.Contains(mOutputViewTabPage))
                    this.tabControl1.TabPages.Add(mOutputViewTabPage);
                else if (!show && this.tabControl1.TabPages.Contains(mOutputViewTabPage))
                    this.tabControl1.TabPages.Remove(mOutputViewTabPage);
            }
            else if (content == mDataCacheView)
            {
                if (show && !this.tabControl2.TabPages.Contains(mDataCacheTabPage))
                    this.tabControl2.TabPages.Add(mDataCacheTabPage);
                else if (!show && this.tabControl2.TabPages.Contains(mDataCacheTabPage))
                    this.tabControl2.TabPages.Remove(mDataCacheTabPage);
            }
            else if (content == mInstructionCacheView)
            {
                if (show && !this.tabControl2.TabPages.Contains(mInstructionCacheTabPage))
                    this.tabControl2.TabPages.Add(mInstructionCacheTabPage);
                else if (!show && this.tabControl2.TabPages.Contains(mInstructionCacheTabPage))
                    this.tabControl2.TabPages.Remove(mInstructionCacheTabPage);
            }
            else if (content == mUnifiedCacheView)
            {
                if (show && !this.tabControl2.TabPages.Contains(mUnifiedCacheTabPage))
                    this.tabControl2.TabPages.Add(mUnifiedCacheTabPage);
                else if (!show && this.tabControl2.TabPages.Contains(mUnifiedCacheTabPage))
                    this.tabControl2.TabPages.Remove(mUnifiedCacheTabPage);
            }
            else if (content == mPluginUIControlsView)
            {
                if (mPluginUIControlsForm == null)
                {
                    mPluginUIControlsForm = new Form();
                    mPluginUIControlsForm.SuspendLayout();
                    mPluginUIControlsForm.FormClosed += delegate(object sender, FormClosedEventArgs e) { mPluginUIControlsView.IsShowing = false; mPluginUIControlsView = null; };
                    mPluginUIControlsForm.ShowInTaskbar = false;
                    mPluginUIControlsForm.Text = "PluginUIView";
                    mPluginUIControlsForm.TopMost = true;
                    mPluginUIControlsView.Control.Dock = DockStyle.Fill;
                    mPluginUIControlsForm.Controls.Add(mPluginUIControlsView.Control);
                    mPluginUIControlsForm.ResumeLayout(false);
                }
                mPluginUIControlsForm.Visible = show;
            }

            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.PerformLayout();

        }

        private void splitContainer1_Panel2_Layout(object sender, LayoutEventArgs e)
        {
            bool anyTabs = (this.tabControl1.TabPages.Count > 0);
            this.splitContainer1.Panel2Collapsed = !anyTabs;

        }

        //private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        //{
        //    if (mStackView.Visible)
        //    {
        //        int stackViewWidthRequired = mStackView.GetWidth();
        //        stackPanel.Width = stackViewWidthRequired;
        //    }
        //    if (mRegistersView.Visible)
        //    {
        //        int registersViewWidthRequired = mRegistersView.GetWidth();
        //        mRegistersView.Control.Width = registersViewWidthRequired;
        //    }
        //    //int totalRequired = stackViewWidthRequired + registersViewWidthRequired;
        //    //int containerWidth = splitContainer1.Panel1.Width - totalRequired;
        //    //ARMPluginInterfaces.Utils.OutputDebugString(string.Format("Stack:{0} Registers:{1} Total:{2} Rest:{3}", stackViewWidthRequired, registersViewWidthRequired, totalRequired, containerWidth));

        //}

    }//class StaticMainForm
}
