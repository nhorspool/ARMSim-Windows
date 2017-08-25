using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace ARMSim.GUI.Views
{
    /// <summary>
    /// This class handles the common graphic features that are common to each of the views.
    /// Text Font/Colour, Background Colour, Hightlight Colour are handled now.
    /// Includes persisting these settings to the save xml file, and loading.
    /// </summary>
    public class GraphicElements
    {
        private readonly Font _defaultFont;
        private readonly Color _defaultBackgroundColour;
        private readonly Color _defaultHighlightColour;
        private readonly Color _defaultTextColour;

        private Font _currentFont;
        private Color _currentBackgroundColour;
        private Color _currentHighlightColour;
        private Color _currentTextColour;

        private readonly IView _parent;

        //various labels used in xml file processing
        private const string _UIAttributesLabel = "UIAttributes";
        private const string _TypeLabel = "Type";
        private const string _FontLabel = "Font";
        private const string _BackgroundColourLabel = "BackgroundColour";
        private const string _HighlightColourLabel = "HighlightColour";
        private const string _TextColourLabel = "TextColour";

        /// <summary>
        /// GraphicElements ctor
        /// Constructor takes an IView to the parent
        /// </summary>
        /// <param name="parent"></param>
        public GraphicElements(IView parent)
        {
            //save parent interface
            _parent = parent;

            //get the current settings so they can be saved as the defaults
            _defaultFont = _parent.CurrentFont;
            _defaultBackgroundColour = _parent.CurrentBackgroundColour;
            _defaultHighlightColour = _parent.CurrentHighlightColour;
            _defaultTextColour = _parent.CurrentTextColour;

            //save default settings as current settings
            this.setDefault();
        }

        /// <summary>
        /// restore the default settings to be the current settings
        /// </summary>
        public void setDefault()
        {
            _currentFont = _defaultFont;
            _currentBackgroundColour = _defaultBackgroundColour;
            _currentHighlightColour = _defaultHighlightColour;
            _currentTextColour = _defaultTextColour;
        }//setDefault

        private bool FontChanged { get { return (_currentFont != _defaultFont); } }
        private bool BackgroundColourChanged { get { return (_currentBackgroundColour != _defaultBackgroundColour); } }
        private bool HighlightColourChanged { get { return (_currentHighlightColour != _defaultHighlightColour); } }
        private bool TextColourChanged { get { return (_currentTextColour != _defaultTextColour); } }

        /// <summary>
        /// checks if any of the settings are different than the default ones.
        /// if any are different, return true
        /// </summary>
        public bool Changed
        {
            get
            {
                return (FontChanged || BackgroundColourChanged || HighlightColourChanged || TextColourChanged);
            }//get
        }//Changed

        //properties return the current settings
        ///<summary>Access the font property</summary>
        public Font CurrentFont { get { return _currentFont; } }
        ///<summary>Access the background color property</summary>
        public Color CurrentBackgroundColour { get { return _currentBackgroundColour; } }
        ///<summary>Access the highlight color property</summary>
        public Color CurrentHighlightColour { get { return _currentHighlightColour; } }
        ///<summary>Access the text color color property</summary>
        public Color CurrentTextColour { get { return _currentTextColour; } }

        /// <summary>
        /// Save the view settings to the save xml document
        /// first check if any have changed, if false, then save nothing
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public void SaveToXML(XmlWriter xmlOut)
        {
            //if nothing has changed from default, save nothing
            if (!this.Changed) return;

            try
            {
                xmlOut.WriteAttributeString(_TypeLabel, _UIAttributesLabel);

                if (FontChanged)
                {
                    //convert Font to a memory stream
                    using (MemoryStream mem = new MemoryStream())
                    {
                        new BinaryFormatter().Serialize(mem, _currentFont);
                        string str = Convert.ToBase64String(mem.ToArray());

                        //and save each of the settings
                        xmlOut.WriteAttributeString(_FontLabel, str);
                    }
                }

                if (BackgroundColourChanged)
                {
                    xmlOut.WriteAttributeString(_BackgroundColourLabel, _currentBackgroundColour.ToArgb().ToString());
                }

                if (HighlightColourChanged)
                {
                    xmlOut.WriteAttributeString(_HighlightColourLabel, _currentHighlightColour.ToArgb().ToString());
                }

                if (TextColourChanged)
                {
                    xmlOut.WriteAttributeString(_TextColourLabel, _currentTextColour.ToArgb().ToString());
                }
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
            }
        }//savetoXML

        /// <summary>
        /// Load the view settings from the xml document
        /// </summary>
        /// <param name="xmlIn"></param>
        public void loadFromXML(XmlReader xmlIn)
        {
            try
            {
                string str = xmlIn.GetAttribute(_TypeLabel);
                if (str != _UIAttributesLabel) return;

                str = xmlIn.GetAttribute(_FontLabel);
                if (str != null)
                {
                    using (MemoryStream mem = new MemoryStream(Convert.FromBase64String(str)))
                    {
                        _parent.CurrentFont = _currentFont = (new BinaryFormatter().Deserialize(mem) as Font);
                    }
                }
                str = xmlIn.GetAttribute(_BackgroundColourLabel);
                if (str != null)
                {
                    _parent.CurrentBackgroundColour = _currentBackgroundColour = Color.FromArgb(int.Parse(str));
                }

                str = xmlIn.GetAttribute(_HighlightColourLabel);
                if (str != null)
                {
                    _parent.CurrentHighlightColour = _currentHighlightColour = Color.FromArgb(int.Parse(str));
                }

                str = xmlIn.GetAttribute(_TextColourLabel);
                if (str != null)
                {
                    _parent.CurrentTextColour = _currentTextColour = Color.FromArgb(int.Parse(str));
                }
                return;
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.setDefault();
            }
        }//loadFromXML

        // The user wishes to change the text Font. Display the Font selection dialog
        // and let the user select a font. The font colour can be selected from the same dialog.
        // Set this as current and notify the parent of the change.
        private void menuItem_Font(Object sender, System.EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.Font = _currentFont;
                fontDialog.ShowColor = true;
                fontDialog.Color = _currentTextColour;

                if (fontDialog.ShowDialog(_parent.ViewControl) == DialogResult.OK)
                {
                    _currentFont = fontDialog.Font;
                    _currentTextColour = fontDialog.Color;
                    _parent.CurrentFont = _currentFont;
                    _parent.CurrentTextColour = _currentTextColour;
                }
            }
        }

        //The user wishes to change the background colour. Display the Colour selection dialog
        //and let the user select a colour.
        //Set this as current and notify the parent of the change.
        private void menuItem_Background_Colour(Object sender, System.EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = _currentBackgroundColour;
                if (colorDialog.ShowDialog(_parent.ViewControl) == DialogResult.OK)
                {
                    _currentBackgroundColour = colorDialog.Color;
                    _parent.CurrentBackgroundColour = _currentBackgroundColour;
                }
            }
        }

        //The user wishes to change the hightlight colour. Display the Colour selection dialog
        //and let the user select a colour.
        //Set this as current and notify the parent of the change.
        private void menuItem_Highlight_Colour(Object sender, System.EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = _currentBackgroundColour;
                if (colorDialog.ShowDialog(_parent.ViewControl) == DialogResult.OK)
                {
                    _currentHighlightColour = colorDialog.Color;
                    _parent.CurrentHighlightColour = _currentHighlightColour;
                }
            }
        }

        //Restores the settings back to their program start defaults.
        private void menuItem_Restore_Defaults(Object sender, System.EventArgs e)
        {
            //set the defaults back into current
            this.setDefault();

            //notify the parent that they display items have changed by setting the values
            _parent.CurrentFont = _currentFont;
            _parent.CurrentBackgroundColour = _currentBackgroundColour;
            _parent.CurrentHighlightColour = _currentHighlightColour;
            _parent.CurrentTextColour = _currentTextColour;
        }

        /// <summary>
        /// Adds the menu items to the given pop menu. This is called by the owner of this class
        /// in response to a context menu activation.
        /// </summary>
        /// <param name="menu">menu being use for popup menu</param>
        /// <param name="seperator">true - add a seperator to top of menu</param>
        public void Popup(ToolStrip menu, bool separator)
        {
            //if the separator flag is set, put one in
            if (separator)
            {
                menu.Items.Add(new ToolStripSeparator());
            }

            //Add the common menu options.
            menu.Items.Add(new ToolStripMenuItem("&Font", null, menuItem_Font));
            menu.Items.Add(new ToolStripMenuItem("&Background Colour", null, menuItem_Background_Colour));
            menu.Items.Add(new ToolStripMenuItem("&Highlight Colour", null, menuItem_Highlight_Colour));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem("&Restore Defaults", null, menuItem_Restore_Defaults));
        }//Popup
    }//class GraphicElements
}
