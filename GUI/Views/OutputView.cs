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
    /// <summary>
    /// This class implements the OutputView of the simulator.
    /// This view holds the console output and the standard consoles.
    /// </summary>
    public partial class OutputView : UserControl, IView, IViewXMLSettings, IOutputView
    {
        ///<summary>List of standard consoles</summary>
        private Dictionary<uint, StandardIOConsole> mStandardIOConsoles;

        private GraphicElements _graphicElements;
        private Color _highlightColor;

        private bool mNewLine = true;
        private int mCurrentLine = 0;
        private uint _nextHandle = 0;

        //private ApplicationJimulator _JM;

        /// <summary>
        /// OutputView ctor
        /// </summary>
        /// <param name="jm">reference to simulator engine</param>
        /// <param name="codeSelectionHandler">delegate that can select a line of code in the code view</param>
        public OutputView(ApplicationJimulator jm)
        {
            //_JM = jm;
            //_codeSelectionHandler = codeSelectionHandler;
            mStandardIOConsoles = new Dictionary<uint, StandardIOConsole>();

            this.Text = OutputView.ViewName;

            InitializeComponent();

            this.defaultSettings();
            _graphicElements = new GraphicElements(this);
        }

        /// <summary>
        /// set the properties of the outputview to default values
        /// </summary>
        public void defaultSettings()
        {
            mNewLine = true;
            mCurrentLine = 0;
            _highlightColor = Color.LightBlue;
        }

        /// <summary>
        /// Access the current font of the consoles.
        /// </summary>
        public Font CurrentFont
        {
            get { return listBox1.Font; }
            set { listBox1.Font = value; }
        }

        /// <summary>
        /// Access the text color of the consoles.
        /// </summary>
        public Color CurrentTextColour
        {
            get { return listBox1.ForeColor; }
            set { listBox1.ForeColor = value; }
        }

        /// <summary>
        /// Access the highlight color of the consoles.
        /// </summary>
        public Color CurrentHighlightColour
        {
            get { return this._highlightColor; }
            set { this._highlightColor = value; }
        }

        /// <summary>
        /// Access the background color of the consoles.
        /// </summary>
        public Color CurrentBackgroundColour
        {
            get { return listBox1.BackColor; }
            set { listBox1.BackColor = value; }
        }

        /// <summary>
        /// Returns a reference to the views control
        /// </summary>
        public Control ViewControl { get { return this; } }

        /// <summary>
        /// Reset the view.
        /// Remove all standard consoles, create the stdout/stdin/stderr console
        /// </summary>
        public void resetView()
        {
        }//resetView

        ///<summary>Update view</summary>
        public void updateView() { }

        /// <summary>
        /// Save the view settings to the save xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(OutputView.ViewName);
            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();
        }//saveState

        /// <summary>
        /// Load the view settings from the xml document
        /// </summary>
        /// <param name="xmlIn"></param>
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
            }//catch
        }//LoadFromXML

        /// <summary>
        /// Clear text from the output console
        /// </summary>
        public void ClearText()
        {
            listBox1.Items.Clear();
            mCurrentLine = 0;
            mNewLine = true;
        }//ClearText

        /// <summary>
        /// Write a character to the output console.
        /// </summary>
        /// <param name="ch">character to write</param>
        public void WriteConsole(char chr)
        {
            if (mNewLine)
            {
                mCurrentLine = listBox1.Items.Add("");
                mNewLine = false;
            }//if

            if (chr == '\n')
            {
                mNewLine = true;
                return;
            }//if

            string line = listBox1.Items[mCurrentLine] as string;
            line += chr;
            listBox1.Items[mCurrentLine] = line;

            int bottomIndex = listBox1.IndexFromPoint(0, listBox1.ClientSize.Height - 1);
            if (bottomIndex > 0 && mCurrentLine > bottomIndex)
            {
                int top = listBox1.TopIndex;
                top += ((mCurrentLine - bottomIndex) + 1);
                listBox1.TopIndex = top;
                //this.Invalidate();
            }//if
        }//WriteConsole

        /// <summary>
        /// Write a string to the output console
        /// </summary>
        /// <param name="str">string to write</param>
        public void WriteLine(string str)
        {
            mCurrentLine = listBox1.Items.Add(str);
            mNewLine = true;
        }//WriteLine

        ///<summary>Name of this view. used to write tags to xml document</summary>
        public static string ViewName { get { return "OutputView"; } }

        ///<summary>called when a single step is starting</summary>
        public void stepStart() { }
        ///<summary>called when a single step has ended</summary>
        public void stepEnd() { }

        /// <summary>
        /// Creates a new standard console. A tab is created in the output view 
        /// with the title set to the one given.
        /// The console is added to the list of standard consoles and a handle
        /// returned to the caller.
        /// </summary>
        /// <param name="title">title of the new standard console</param>
        /// <returns>handle of console</returns>
        public uint CreateStandardConsole(string title)
        {
            StandardIOConsole console = new StandardIOConsole();
            console.ConsoleHandle = _nextHandle++; ;
            console.CurrentFont = this.CurrentFont;
            console.CurrentBackgroundColour = this.CurrentBackgroundColour;
            console.CurrentTextColour = this.CurrentTextColour;
            console.Dock = DockStyle.Fill;
            mStandardIOConsoles.Add(console.ConsoleHandle, console);

            TabPage tabPage = new TabPage();
            tabPage.Text = title;
            tabPage.Tag = console;
            tabPage.Controls.Add(console);

            this.tabControl1.TabPages.Add(tabPage);
            this.tabControl1.SelectedTab = tabPage;
            return console.ConsoleHandle;
        }//CreateStandardConsole

        /// <summary>
        /// Close a standard console. Removes it from the tab control and remove from the list
        /// of consoles.
        /// </summary>
        /// <param name="handle">handle of console to close</param>
        public void CloseStandardConsole(uint handle)
        {
            if (mStandardIOConsoles.ContainsKey(handle))
            {
                foreach (TabPage tabPage in this.tabControl1.TabPages)
                {
                    StandardIOConsole console = tabPage.Tag as StandardIOConsole;
                    if (console != null && console.ConsoleHandle == handle)
                    {
                        this.tabControl1.TabPages.Remove(tabPage);
                        mStandardIOConsoles.Remove(handle);
                        break;
                    }//if
                }//foreach
            }//if
        }//CloseStandardConsole

        /// <summary>
        /// Write a character to a standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <param name="ch">character to write</param>
        public void WriteStandardConsole(uint handle, char chr)
        {
            if (mStandardIOConsoles.ContainsKey(handle))
            {
                mStandardIOConsoles[handle].Write(chr);
            }//if
        }//WriteStandardConsole

        /// <summary>
        /// Read a character from the standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        public char ReadStandardConsole(uint handle)
        {
            if (mStandardIOConsoles.ContainsKey(handle))
            {
                return mStandardIOConsoles[handle].Read();
            }//if
            return (char)0;
        }//ReadStandardConsole

        /// <summary>
        /// Peek at the next character in the specified console.
        /// If none is available, the console waits for a keystroke.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        public char PeekStandardConsole(uint handle)
        {
            if (mStandardIOConsoles.ContainsKey(handle))
            {
                return mStandardIOConsoles[handle].Peek();
            }//if
            return (char)0;
        }//PeekStandardConsole

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();
            _graphicElements.Popup(cms, false);
        }//contextMenuStrip1_Opening

        public void TerminateInput()
        {
            foreach (StandardIOConsole std in mStandardIOConsoles.Values)
            {
                std.SetAbort();
            }
        }
    }//class OutputView
}
