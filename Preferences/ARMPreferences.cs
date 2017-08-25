using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using ARMPluginInterfaces.Preferences;

namespace ARMSim.Preferences
{
    /// <summary>
    /// This class represents all the preferences for ARMSim. At application load time this
    /// class is populated by reading the application preferences file.
    /// Additionally, it provides a read-only interface to each of the preferences modules to interested code.
    /// </summary>
    public class ARMPreferences : IViewXMLSettings, IARMPreferences
    {
        /// <summary>
        /// All the cache preferences
        /// </summary>
        public CachePreferences CachePreferences { get; set; }

        /// <summary>
        /// All simulator engine specific preferences
        /// </summary>
        public SimulatorPreferences SimulatorPreferences { get; set; }

        /// <summary>
        /// General ARMSim preferences
        /// </summary>
        public GeneralPreferences GeneralPreferences { get; set; }

        /// <summary>
        /// All plugin related preferences
        /// </summary>
        public PluginPreferences PluginPreferences { get; set; }

        /// <summary>
        /// The last tab selected on the preferences form
        /// </summary>
        public string LastTab { get; set; }

        /// <summary>
        /// Read-only version of the general preferences
        /// </summary>
        public IGeneralPreferences IGeneralPreferences { get { return GeneralPreferences; } }

        /// <summary>
        /// Read-only version of the cache preferences
        /// </summary>
        public ICachePreferences ICachePreferences { get { return CachePreferences; } }

        /// <summary>
        /// Read-only version of the simulator preferences
        /// </summary>
        public ISimulatorPreferences ISimulatorPreferences { get { return SimulatorPreferences; } }

        /// <summary>
        /// Read-only version of the plugin preferences
        /// </summary>
        public IPluginPreferences IPluginPreferences { get { return PluginPreferences; } }

        /// <summary>
        /// ctor for this class, simply instantiate the encapsulated preferences
        /// </summary>
        public ARMPreferences()
        {
            this.CachePreferences = new CachePreferences();
            this.SimulatorPreferences = new SimulatorPreferences();
            this.GeneralPreferences = new GeneralPreferences();
            this.PluginPreferences = new PluginPreferences();
        }

        /// <summary>
        /// This tag is used as the xml element name for the ARMSim preferences
        /// </summary>
        public static string TagName
        {
            get { return "ARMPreferences"; }
        }

        /// <summary>
        /// Load preferences from the application preferences file
        /// If anything go es wrong we will revert to default preferences
        /// </summary>
        /// <param name="xmlIn"></param>
        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                //clear out data structures
                this.defaultSettings();

                xmlIn.MoveToContent();

                this.LastTab = xmlIn.GetAttribute("LastTab");

                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement)
                        break;

                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        if (xmlIn.Name == CachePreferences.TagName)
                        {
                            this.CachePreferences.LoadFromXML(xmlIn.ReadSubtree());
                        }//if
                        else if (xmlIn.Name == SimulatorPreferences.TagName)
                        {
                            this.SimulatorPreferences.LoadFromXML(xmlIn.ReadSubtree());
                        }
                        else if (xmlIn.Name == GeneralPreferences.TagName)
                        {
                            this.GeneralPreferences.LoadFromXML(xmlIn.ReadSubtree());
                        }
                        else if (xmlIn.Name == PluginPreferences.TagName)
                        {
                            this.PluginPreferences.LoadFromXML(xmlIn.ReadSubtree());
                        }
                    }//if
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }
            catch (Exception ex)
            {
                //if an exception occurs, simply accept the OS defaults
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                defaultSettings();
            }
        }//LoadFromXML

        /// <summary>
        /// Set all preferences to their default state
        /// </summary>
        public void defaultSettings()
        {
            this.CachePreferences.defaultSettings();
            this.SimulatorPreferences.defaultSettings();
            this.GeneralPreferences.defaultSettings();
            this.PluginPreferences.defaultSettings();
            this.LastTab = string.Empty;
        }

        /// <summary>
        /// Save the preferences to the application preferences file
        /// </summary>
        /// <param name="xmlOut"></param>
        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(ARMPreferences.TagName);
            if (!String.IsNullOrEmpty(this.LastTab))
            {
                xmlOut.WriteAttributeString("LastTab", this.LastTab);
            }//if

            this.SimulatorPreferences.saveState(xmlOut);
            this.CachePreferences.saveState(xmlOut);
            this.GeneralPreferences.saveState(xmlOut);
            this.PluginPreferences.saveState(xmlOut);
            xmlOut.WriteEndElement();
        }//saveState

    }//class ARMPreferences
}