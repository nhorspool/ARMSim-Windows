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

//using System.Windows.Forms.Design;
//using System.ComponentModel.Design;

namespace ARMSim.GUI.Views
{
    //[Designer(typeof(ARMSim.GUI.Views.CodeViewDesigner))]
    public partial class CodeView : UserControl, IView, IViewXMLSettings
    {
        //the simulator engine
        private ApplicationJimulator _JM;

        //true if there were compile errors and we are displaying the original code text
        //with compile errors in the code area
        private bool _errors;

        //public event ARMSimWindowManager.OnRecalLayout OnRecalLayout;

        //graphic elements for this view
        private GraphicElements _graphicElements;

        //Breakpoints moved into sim engine
        ////breakpoints loaded from settings file
        //private List<uint> _loadedBreakpoints;

        //callback to the main forms character output handler to OutputView
        private CharacterOutputDelegate _characterOutputHandler;

        //current graphic elements.
        private Color _currentHighlightColour;
        private Font _currentFont;
        private Color _currentTextColour;
        private Color _currentBackgroundColour;

        ////the context menu
        //private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;

        public CodeView(ApplicationJimulator jm)
        {
            _JM = jm;
            this.Text = CodeView.ViewName;
            InitializeComponent();

            this.defaultSettings();
            _graphicElements = new GraphicElements(this);

        }

        //Property for the character output callback
        public CharacterOutputDelegate CharOutputHandler
        {
            get { return _characterOutputHandler; }
            set { _characterOutputHandler += value; }
        }

        //public ApplicationJimulator Jimulator { set { _JM = value; } }

        public static string ViewName { get { return "CodeView"; } }

        public void defaultSettings()
        {
            this.CurrentHighlightColour = Color.LightBlue;
            this.CurrentTextColour = Color.Black;
            this.CurrentFont = new Font("Courier New", 9.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.CurrentBackgroundColour = Color.Gray;
        }

        //load the CodeView settings from the save document
        public void LoadFromXML(XmlReader xmlIn)
        {
            //ALl breakpoint logic moved to sim engine.
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

            //try
            //{
            //    xmlIn.MoveToContent();
            //    //load any of the standard graphic elements
            //    _graphicElements.loadFromXML(xmlIn);
            //    xmlIn.Read();
            //    do
            //    {
            //        if (xmlIn.NodeType == XmlNodeType.EndElement) break;
            //        if (xmlIn.NodeType == XmlNodeType.Element && xmlIn.Name == "Breakpoint")
            //        {
            //            //get the breakpoint address
            //            string str = xmlIn.GetAttribute("Position");
            //            //allocate a loaded breakpoint collection if needed
            //            if (_loadedBreakpoints == null)
            //            {
            //                _loadedBreakpoints = new List<uint>();
            //            }//if
            //            //and add this breakpoint
            //            _loadedBreakpoints.Add(Convert.ToUInt32(str, 16));
            //        }
            //        xmlIn.Skip();
            //    } while (!xmlIn.EOF);
            //}
            //catch (Exception ex)
            //{
            //    ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
            //    this.defaultSettings();
            //}

        }//LoadFromXML

        //Save the CodeView settings to the save document
        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(CodeView.ViewName);
            //save the graphic settings, regardless of error mode
            _graphicElements.SaveToXML(xmlOut);

            //Breakpoint logic moved to sim engine
            ////we are in error mode, all done
            //if (!_errors)
            //{
            //    foreach (CodeViewTab tab in this.Controls)
            //    {
            //        List<uint> al = tab.BreakPoints;
            //        foreach (uint address in al)
            //        {
            //            xmlOut.WriteStartElement("Breakpoint");
            //            xmlOut.WriteAttributeString("Position", address.ToString("x8"));
            //            xmlOut.WriteEndElement();
            //        }//foreach
            //    }//foreach
            //}

            xmlOut.WriteEndElement();
        }//saveState

        //All breakpoint logic moved to sim engine
        ////Loads the breakpoints that were saved in the save settings document at application load time.
        ////To make this simple and robust, the saved breakpoint must have the same address and opcode
        ////to be valid. This means that if the user edits the code that changes the line of an opcode, that
        ////is ok. However if the code changes at the address of the breakpoint, it will be rejected.
        //// _loadedBreakpoints contains the breakpoints from the saved document, and is set to null
        ////when this function completes
        //private void loadBreakpoints()
        //{
        //    //if not valid, then there were no breakpoints in the save document
        //    if (_loadedBreakpoints == null) return;
        //    //the try is used to catch an exception in ToUInt32 in case there are invalid
        //    //numbers in the address:opcode section
        //    try
        //    {
        //        //iterate thru the tabs setting the breakpoints foreach tab
        //        foreach (CodeViewTab tab in this.Controls)
        //        {
        //            tab.BreakPoints = _loadedBreakpoints;
        //        }
        //    }//try
        //    catch (Exception ex)
        //    {
        //        ARMPluginInterfaces.Utils.OutputDebugString("error loading breakpoints:" + ex.Message);
        //    }//catch
        //    finally
        //    {
        //        _loadedBreakpoints = null;
        //    }
        //}

        //[Browsable(true)]
        //[Category("Wizard")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[Description("Gets the collection of wizard pages in this tab control.")]
        public Font CurrentFont
        {
            get { return _currentFont; }
            set
            {
                _currentFont = value;
                foreach (CodeViewTab tab in tabControl1.TabPages)
                {
                    tab.CurrentFont = value;
                }
            }
        }

        public Color CurrentTextColour
        {
            get { return _currentTextColour; }
            set
            {
                _currentTextColour = value;
                foreach (CodeViewTab tab in tabControl1.TabPages)
                {
                    tab.CurrentTextColour = value;
                }
            }
        }

        public Color CurrentBackgroundColour
        {
            get { return _currentBackgroundColour; }
            set
            {
                _currentBackgroundColour = value;
                foreach (CodeViewTab tab in tabControl1.TabPages)
                {
                    tab.CurrentBackgroundColour = value;
                }
            }
        }
        public Color CurrentHighlightColour
        {
            get { return _currentHighlightColour; }
            set
            {
                _currentHighlightColour = value;
                foreach (CodeViewTab tab in tabControl1.TabPages)
                {
                    tab.CurrentHighlightColour = value;
                }
            }
        }

        public Control ViewControl { get { return this; } }

        public void resetView() { }

        public void updateView()
        {
            if (_JM == null || !_JM.ValidLoadedProgram)
                return;
            if (!_JM.InRange(_JM.GPR.PC, ARMPluginInterfaces.MemorySize.Word))
                return;
            if (_errors)
                return;

            foreach (CodeViewTab tab in tabControl1.TabPages)
            {
                if (tab.HasAddress(_JM.GPR.PC))
                {
                    tabControl1.SelectedTab = tab;
                    tab.SelectCodeAddress(_JM.GPR.PC);
                    break;  // no need to continue searching
                }
            }//foreach
        }//updateView

        public void stepStart() { }
        public void stepEnd() { }


        public void InitErrors(IList<ArmAssembly.ArmFileInfo> progInfo)
        {
            _errors = true;

            this.tabControl1.TabPages.Clear();

            foreach (ArmAssembly.ArmFileInfo pp in progInfo)
            {
                if (pp == null) continue;
                CodeViewTab tabPage = new CodeViewTab(pp, _JM);
                tabPage.CurrentFont = this.CurrentFont;
                tabPage.CurrentHighlightColour = this.CurrentHighlightColour;
                tabPage.CurrentTextColour = this.CurrentTextColour;
                tabPage.CurrentBackgroundColour = this.CurrentBackgroundColour;
                tabPage.initErrors();

                this.tabControl1.Controls.Add(tabPage);
            }
        }

        public bool InitNormal(IList<ArmAssembly.ArmFileInfo> progInfo)
        {
            _errors = false;

            this.tabControl1.TabPages.Clear();

            foreach (ArmAssembly.ArmFileInfo pp in progInfo)
            {
                CodeViewTab tabPage = new CodeViewTab(pp, _JM);
                tabPage.CurrentFont = this.CurrentFont;
                tabPage.CurrentHighlightColour = this.CurrentHighlightColour;
                tabPage.CurrentTextColour = this.CurrentTextColour;
                tabPage.CurrentBackgroundColour = this.CurrentBackgroundColour;
                //tabPage.init();

                this.tabControl1.TabPages.Add(tabPage);
            }
            //All breakpoint logic moved to sim engine
            //this.loadBreakpoints();
            return true;
        }

        public void SelectCodeLine(string fileName, int line)
        {
            foreach (CodeViewTab tab in this.Controls)
            {
                if (tab.Name == fileName)
                {
                    this.tabControl1.SelectedTab = tab;
                    tab.SelectCodeLine(line);
                    return;
                }
            }//foreach
        }//SelectCodeLine

        //All breakpoint logic moved to sim engine
        //public bool AtBreakpoint(uint address)
        //{
        //    foreach (CodeViewTab tab in this.Controls)
        //    {
        //        if (tab.AtBreakpoint(address)) return true;
        //    }//foreach
        //    return false;
        //}//AtBreakpoint
        //public void ClearAllBreakpoints()
        //{
        //    foreach (CodeViewTab tab in this.Controls)
        //    {
        //        tab.ClearAllBreakpoints();
        //    }//foreach
        //}//ClearAllBreakpoints
        //public void ToggleBreakpoint()
        //{
        //    (this.SelectedTab as CodeViewTab).ToggleBreakpoint();
        //}

        private CodeViewTab.ListLine ItemFromPoint(Point pt)
        {
            if (this.tabControl1.SelectedTab == null)
                return null;
            
            return (this.tabControl1.SelectedTab as CodeViewTab).ItemFromPoint(pt);
        }

        private void menuItem_ToggleBreakpoint(object sender, System.EventArgs e)
        {
            ToolStripDropDownItem tsdi = (sender as ToolStripDropDownItem);
            CodeViewTab.ListLine lst = ItemFromPoint(tsdi.Owner.Location);

            if (lst != null)
            {
                _JM.Breakpoints.Toggle(lst.Address);
                this.Invalidate();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();

            ToolStripMenuItem toggleItem = new ToolStripMenuItem("&Toggle Breakpoint", null, this.menuItem_ToggleBreakpoint);
            toggleItem.Enabled = false;

            CodeViewTab.ListLine lst = ItemFromPoint(Control.MousePosition);
            if (lst != null && lst.Valid)
                toggleItem.Enabled = true;

            cms.Items.Add(toggleItem);
            _graphicElements.Popup(cms, true);
        }

        //Called when the application wants to know if any files have changed since being loaded.
        //Query each of the tabs and get the changed flag. Build a response list from the tabs reporting
        //a change. Returns a null reference if no tabs report changed.
        public List<string> Activated()
        {
            //start with a null list, no need to create if no tabs changed.
            List<string> changedFiles = null;

            //iterate over the tabs, query each one for change
            foreach (CodeViewTab tab in this.Controls)
            {
                if (tab.Changed)
                {
                    //tab has a change, so build the list if needed
                    if (changedFiles == null)
                    {
                        changedFiles = new List<string>();
                    }

                    //add the filename to the response list and clear the tab change flag.
                    changedFiles.Add(tab.Name);
                    tab.Changed = false;
                }
            }//foreach
            return changedFiles;
        }

        private void tabControl1_Resize(object sender, EventArgs e)
        {
            this.updateView();
        }

        public void TerminateInput() { }

    }//class CodeView

    //internal class CodeViewDesigner : ParentControlDesigner
    //{
    //}//class WizardDesigner

}
