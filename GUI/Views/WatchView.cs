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
    /// This class manages the WatchView for the simulator user interface.
    /// It manages a list of watch items that the user creates. Each watch item
    /// is updated with its current value after the simulator stops executing instructions(updateView).
    /// </summary>
    public partial class WatchView : UserControl, IView, IViewXMLSettings
    {
        private List<WatchEntry> _loadedEntries;

        private GraphicElements _graphicElements;
        private Color _highlightColor;

        /// <summary>
        /// This enum defines the types of watch items that be loaded into the listview.
        /// </summary>
        public enum WatchType
        {
            Byte,
            HalfWord,
            Word,
            Character,
            String
        }

        /// <summary>
        /// This class defines one entry in the watchview listview
        /// The Value property is where the entry will evaluate itself
        /// to the type requested and return a string representation of the value.
        /// </summary>
        private class WatchEntry
        {
            //private uint _address = 0;

            //access to the items fields
            public uint Address { get; set; }

            private readonly string _label;
            private readonly WatchView.WatchType _wt;
            private readonly bool _signed;
            private readonly bool _displayHex;
            private ApplicationJimulator _JM;

            /// <summary>
            /// WatchEntry ctor
            /// </summary>
            /// <param name="label">the label name to be displayed</param>
            /// <param name="wt">the type of watch item</param>
            /// <param name="signed">true to display as a signed number</param>
            /// <param name="displayHex">true to display in hexadecimal</param>
            /// <param name="JM">a reference to the simulator</param>
            public WatchEntry(string label, WatchView.WatchType wt, bool signed, bool displayHex, ApplicationJimulator JM)
            {
                _label = label;
                _wt = wt;
                _signed = signed;
                _displayHex = displayHex;
                _JM = JM;
            }//WatchEntry ctor

            public string Label { get { return _label; } }
            public WatchView.WatchType WatchType { get { return _wt; } }
            public bool Signed { get { return _signed; } }
            public bool DisplayHex { get { return _displayHex; } }

            /// <summary>
            /// Property to evaluate itself and return a string representation.
            /// </summary>
            public override string ToString()
            {
                //evaluate based on its type
                switch (_wt)
                {
                    //item is a byte
                    case WatchView.WatchType.Byte:
                        {
                            //get byte from memory
                            uint data = _JM.GetMemoryNoSideEffect(this.Address, ARMPluginInterfaces.MemorySize.Byte);

                            //and convert to signed/unsigned, then to string
                            if (_signed)
                                return ((sbyte)data).ToString();
                            else
                                return _displayHex ? "0x" + ((byte)data).ToString("x2") : ((byte)data).ToString();
                        }
                    //item is a halfword(2 bytes)
                    case WatchView.WatchType.HalfWord:
                        {
                            uint data = _JM.GetMemoryNoSideEffect(this.Address, ARMPluginInterfaces.MemorySize.HalfWord);

                            //and convert to signed/unsigned, then to string
                            if (_signed)
                                return ((short)data).ToString();
                            else
                                return _displayHex ? "0x" + ((ushort)data).ToString("x4") : ((ushort)data).ToString();
                        }
                    //item is a word(4 bytes)
                    case WatchView.WatchType.Word:
                        {
                            uint data = _JM.GetMemoryNoSideEffect(this.Address, ARMPluginInterfaces.MemorySize.Word);

                            //and convert to signed/unsigned, then to string
                            if (_signed)
                                return ((int)data).ToString();
                            else
                                return _displayHex ? "0x" + data.ToString("x8") : data.ToString();
                        }
                    //item is a character
                    case WatchView.WatchType.Character:
                        {
                            uint data = _JM.GetMemoryNoSideEffect(this.Address, ARMPluginInterfaces.MemorySize.Byte);

                            //if the character is normal ascii, return it, otherwise indicate a dot(.)
                            char cdata = '.';
                            if (data >= 32 && data <= 127)
                            {
                                cdata = (char)data;
                            }
                            return "'" + cdata + "'";
                        }
                    //item is a string
                    case WatchView.WatchType.String:
                        {
                            //load string for simulator memory, no side effects
                            string str = Utils.loadStringFromMemory(_JM.GetMemoryNoSideEffect, this.Address, 64);
                            return "\"" + str + "\"";
                        }
                    default: return "";
                }//switch
            }//ToString
        }//class WatchEntry

        //public event ARMSimWindowManager.OnRecalLayout OnRecalLayout;

        private ApplicationJimulator _JM;
        private CodeLabelsDelegate _codeLabelsHandler;
        private ResolveSymbolDelegate _resolveSymbolHandler;
        /// <summary>
        /// WatchView ctor.
        /// </summary>
        /// <param name="jm">A reference to the simulator</param>
        /// <param name="codeLabelsHandler">a delegate to resolve codelabels to address</param>
        /// <param name="resolveSymbolHandler">a delegate to resolve symbols to address</param>
        public WatchView(ApplicationJimulator jm, CodeLabelsDelegate codeLabelsHandler, ResolveSymbolDelegate resolveSymbolHandler)
        {
            _JM = jm;
            _codeLabelsHandler = codeLabelsHandler;
            _resolveSymbolHandler = resolveSymbolHandler;

            //create the watchentry list
            _loadedEntries = new List<WatchEntry>();

            this.Text = WatchView.ViewName;

            InitializeComponent();

            //setup the views graphic elements
            _graphicElements = new GraphicElements(this);

            //set view to default settings
            this.defaultSettings();

        }

        /// <summary>
        /// The font to use for the text in the watchentry list box
        /// </summary>
        public Font CurrentFont
        {
            get { return listView1.Font; }
            set { listView1.Font = value; }
        }

        /// <summary>
        /// The font color to use for the text in the watchentry list box
        /// </summary>
        public Color CurrentTextColour
        {
            get { return listView1.ForeColor; }
            set { listView1.ForeColor = value; }
        }

        /// <summary>
        /// The highlight color to use for the text in the watchentry list box
        /// </summary>
        public Color CurrentHighlightColour
        {
            get { return this._highlightColor; }
            set { this._highlightColor = value; }
        }

        /// <summary>
        /// The background color to use for the text in the watchentry list box
        /// </summary>
        public Color CurrentBackgroundColour
        {
            get { return listView1.BackColor; }
            set { listView1.BackColor = value; }
        }

        /// <summary>
        /// Return a reference to the watchview control
        /// </summary>
        public Control ViewControl { get { return this; } }

        /// <summary>
        /// This is called when the view is reset. This can happen when the user restarts a program
        /// or the program is loaded for the first time.
        /// Check if the entries to load array is > 0, if so, then this array was
        /// loaded from the xml document.
        /// Simply create the watch entries in the listview from this array. If the symbol
        /// cannot be resolved, then it may be for another program, so ignore it.
        /// </summary>
        public void resetView()
        {
            if (_loadedEntries.Count > 0)
            {
                listView1.Items.Clear();
                foreach (WatchEntry we in _loadedEntries)
                {
                    uint address = 0;
                    if (_resolveSymbolHandler(we.Label, ref address))
                    {
                        we.Address = address;
                        ListViewItem lvi = new ListViewItem(new string[2] { we.Label, we.ToString() });
                        lvi.Tag = we;
                        listView1.Items.Add(lvi);
                    }
                }
                _loadedEntries.Clear();
                return;
            }

            //else if the entries to load is zero, then this is possibly a new program loaded.
            //we must scan the listview and make sure the watch items are still valid. If not, remove
            //it, otherwise recompute the labels address. (It may have changed)
            foreach (ListViewItem lvi in listView1.Items)
            {
                uint address = 0;
                if (_resolveSymbolHandler((lvi.Tag as WatchEntry).Label, ref address))
                {
                    (lvi.Tag as WatchEntry).Address = address;
                }
                else
                {
                    listView1.Items.Remove(lvi);
                }
            }//foreach
        }//resetView

        /// <summary>
        /// This is called when the view needs to be updated. When the simulator hits a breakpoint or
        /// the program terminates are examples.
        /// Evaluate all the watch entries and update their values in the listview.
        /// </summary>
        public void updateView()
        {
            if (_JM == null || !_JM.ValidLoadedProgram) return;
            foreach (ListViewItem lvi in listView1.Items)
            {
                WatchEntry we = (lvi.Tag as WatchEntry);
                lvi.SubItems[1].Text = we.ToString();
            }
        }//updateView

        /// <summary>
        /// Returns the string name of this view.
        /// </summary>
        public static string ViewName { get { return "WatchView"; } }

        /// <summary>
        /// Returns the index of the watch entry currently selected in the listview.
        /// Returns -1 if none selected
        /// </summary>
        public int SelectedWatchItem
        {
            get
            {
                ListView.SelectedIndexCollection indexes = this.listView1.SelectedIndices;
                return (indexes.Count == 1) ? indexes[0] : -1;
            }
        }//SelectedWatchItem

        /// <summary>
        /// Removes any watch entries that are selected
        /// </summary>
        public void DeleteCurrentWatchItem()
        {
            int selectedWatchItem = this.SelectedWatchItem;
            if (selectedWatchItem >= 0)
            {
                this.listView1.Items.RemoveAt(selectedWatchItem);
            }
        }//DeleteCurrentWatchItem

        /// <summary>
        /// Removes all watch entries from the listview
        /// </summary>
        public void ClearAllWatchItems()
        {
            _loadedEntries.Clear();
            this.listView1.Items.Clear();
        }//ClearAllWatchItems

        public void stepStart() { }
        public void stepEnd() { }

        /// <summary>
        /// Adds a watch item to the listview. Displays a dialog allowing the user to
        /// specify the watchentry to create.
        /// </summary>
        public void AddWatchItem()
        {
            //create an instance of the add watch dialog
            AddWatch aw = new AddWatch();

            //set the code label resolver delegate
            aw.CodeLabels = _codeLabelsHandler();

            //show the dialog. If OK was pressed, then add watch entry
            if (aw.ShowDialog(this) == DialogResult.OK)
            {
                string lab = aw.Label;
                if (!string.IsNullOrEmpty(lab))
                {
                    //resolve the label name to an address in memory. If we cannot resolve it, just exit
                    uint address = 0;
                    if (_resolveSymbolHandler(lab, ref address))
                    {
                        //create a new watch entry and set the resolved address
                        WatchEntry we = new WatchEntry(lab, aw.WatchType, aw.Signed, aw.DisplayHex, _JM);
                        we.Address = address;

                        //add watch entry to listview
                        ListViewItem lvi = new ListViewItem(new string[2] { we.Label, we.ToString() });
                        lvi.Tag = we;
                        listView1.Items.Add(lvi);
                    }
                }
            }
        }//AddWatchItem

        /// <summary>
        /// Setup view settings to default values
        /// </summary>
        public void defaultSettings()
        {
            _highlightColor = Color.LightBlue;

        }//defaultSettings

        /// <summary>
        /// Given a screen coordinate, determine the watch entry at that point.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private ListViewItem ItemFromPoint(Point pt)
        {
            return (listView1.HitTest(listView1.PointToClient(pt))).Item;
        }//ItemFromPoint

        /// <summary>
        /// Add a watchItem. Invoked from the view context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_AddWatchItem(object sender, System.EventArgs e)
        {
            this.AddWatchItem();
        }

        /// <summary>
        /// Remove a watchItem. Invoked from the view context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_RemoveItem(object sender, System.EventArgs e)
        {
            DeleteCurrentWatchItem();
        }

        /// <summary>
        /// Called when a context menu is being created for the listview.
        /// Add some entries to manipulate the listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //get the menu and clear it out
            ContextMenuStrip cms = (ContextMenuStrip)sender;
            cms.Items.Clear();

            //add the 2 items for adding and removing items
            ToolStripMenuItem addItem = new ToolStripMenuItem("&Add Watch", null, this.menuItem_AddWatchItem);
            ToolStripMenuItem removeItem = new ToolStripMenuItem("&Remove Watch", null, this.menuItem_RemoveItem);

            //assume both are not valid for now
            addItem.Enabled = false;
            removeItem.Enabled = false;

            //the simulator engine must be valid
            if (_JM.ValidLoadedProgram)
            {
                //so adding is valid now
                addItem.Enabled = true;

                //cjeck if the mouse was over a watchentry. If so, enable the remove command.
                if (ItemFromPoint(Control.MousePosition) != null)
                {
                    removeItem.Enabled = true;
                }
            }

            //add to menu
            cms.Items.Add(addItem);
            cms.Items.Add(removeItem);

            //popup context menu
            _graphicElements.Popup(cms, true);
        }//contextMenuStrip1_Opening

        /// <summary>
        /// Called when a keypress is detected in the listview.
        /// Check if it is the delete key and remove the current watch entry if it is.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                DeleteCurrentWatchItem();
        }

        /// <summary>
        /// Called when the view is loading. Load any settings saved in the xml document.
        /// </summary>
        /// <param name="xmlIn"></param>
        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                xmlIn.MoveToContent();
                _graphicElements.loadFromXML(xmlIn);
                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement) break;
                    if (xmlIn.NodeType == XmlNodeType.Element && xmlIn.Name == "WatchItem")
                    {
                        string label = xmlIn.GetAttribute("Label");
                        WatchType wt = (WatchType)Enum.Parse(typeof(WatchType), xmlIn.GetAttribute("WatchType"), true);
                        bool signed = bool.Parse(xmlIn.GetAttribute("Signed"));
                        bool displayHex = bool.Parse(xmlIn.GetAttribute("DisplayHex"));
                        _loadedEntries.Add(new WatchEntry(label, wt, signed, displayHex, _JM));
                    }
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.defaultSettings();
            }
        }//LoadFromXML

        /// <summary>
        /// Called when the application is shutting down. Save all the view settings
        /// to the xml document.
        /// </summary>
        /// <param name="xmlOut"></param>
        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(WatchView.ViewName);
            //_graphicElements.savetoXML(xmlOut);

            foreach (ListViewItem lvi in listView1.Items)
            {
                WatchEntry we = (lvi.Tag as WatchEntry);
                xmlOut.WriteStartElement("WatchItem");
                xmlOut.WriteAttributeString("Label", we.Label);
                xmlOut.WriteAttributeString("WatchType", Enum.GetName(typeof(WatchType), we.WatchType));
                xmlOut.WriteAttributeString("Signed", we.Signed.ToString());
                xmlOut.WriteAttributeString("DisplayHex", we.DisplayHex.ToString());
                xmlOut.WriteEndElement();
            }//foreach

            _graphicElements.SaveToXML(xmlOut);
            xmlOut.WriteEndElement();

        }

        public void TerminateInput() { }

    }//class WatchView
}
