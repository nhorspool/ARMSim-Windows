using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using ARMSim.Preferences;
using ARMSim.Simulator;

namespace ARMSim.GUI.Views
{
    public partial class PluginsUIView : UserControl, IView, IViewXMLSettings
    {
        //private ApplicationJimulator _JM;
        private GraphicElements _graphicElements;
        private Color _highlightColor;

        private Color _currentBackgroundColour;

        //public event ARMSimWindowManager.OnRecalLayout OnRecalLayout;

        /// <summary>
        /// Plugins view ctor
        /// </summary>
        /// <param name="jm"></param>
        public PluginsUIView()
        {
            //_JM = jm;
            this.Text = PluginsUIView.ViewName;
            InitializeComponent();

            this.defaultSettings();
            _graphicElements = new GraphicElements(this);
        }

        public void defaultSettings()
        {
            _highlightColor = Color.LightBlue;
        }

        public Font CurrentFont
        {
            get { return tabControl1.Font; }
            set { tabControl1.Font = value; }
        }

        public Color CurrentTextColour
        {
            get { return tabControl1.ForeColor; }
            set { tabControl1.ForeColor = value; }
        }

        public Color CurrentBackgroundColour
        {
            //get { return tabControl1.BackColor; }
            //set { tabControl1.BackColor = value; }
            get { return _currentBackgroundColour; }
            set
            {
                _currentBackgroundColour = value;
                foreach (TabPage tab in tabControl1.TabPages)
                {
                    tab.BackColor = value;
                }
            }
        }
        //public Control ViewControl { get { return this.tabControl1; } }
        public Control ViewControl
        {
            get { return this.tabControl1; }
        }

        public void resetView() { }
        public void updateView() { }

        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(PluginsUIView.ViewName);
            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();
        }//saveState

        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                _graphicElements.loadFromXML(xmlIn);
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.defaultSettings();
            }
        }//LoadFromXML

        public static string ViewName { get { return "PluginUIView"; } }

        public Color CurrentHighlightColour
        {
            get { return this._highlightColor; }
            set { this._highlightColor = value; }
        }

        public void stepStart() { }
        public void stepEnd() { }

        //public Panel RequestPanel(string title)
        public TabPage RequestPanel(string title)
        {
            TabPage tabPage = new TabPage(title);
            this.tabControl1.TabPages.Add(tabPage);
            return tabPage;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();
            _graphicElements.Popup(cms, false);
        }

        public void ResetTabs()
        {
            this.tabControl1.TabPages.Clear();
        }

        public void TerminateInput() { }

    }//class PluginsUIView
}
