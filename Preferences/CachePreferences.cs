using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Preferences
{
    /// <summary>
    /// This class represents all the cache preferences. This includes all data cache and instruction
    /// cache preference settings as well as global cache settings.
    /// </summary>
    public class CachePreferences : IViewXMLSettings, ARMPluginInterfaces.Preferences.ICachePreferences
    {
        /// <summary>
        /// This flag indicates if the cache is unified or not
        /// </summary>
        public bool UnifiedCache { get; set; }

        /// <summary>Access to the data cache preferences object</summary>
        public DataCachePreferences DataCachePreferences { get; set; }

        /// <summary>Access to the instruction cache preferences object</summary>
        public InstructionCachePreferences InstructionCachePreferences { get; set; }

        /// <summary>
        /// The data cache settings. If the cache is unified, the data cache settings are used
        /// </summary>
        //private DataCachePreferences _data;

        /// <summary>
        /// Cache settings for the Instruction cache. If the cache is unified, this is not used
        /// </summary>
        //private InstructionCachePreferences _instruction;
        public ARMPluginInterfaces.Preferences.IDataCachePreferences IDataCachePreferences { get { return DataCachePreferences; } }
        public ARMPluginInterfaces.Preferences.IInstructionCachePreferences IInstructionCachePreferences { get { return InstructionCachePreferences; } }

        /// <summary>
        /// cachePreferences ctor
        /// create the preferences objects and set all fields to a default value
        /// </summary>
        public CachePreferences()
        {
            DataCachePreferences = new DataCachePreferences();
            InstructionCachePreferences = new InstructionCachePreferences();
            this.defaultSettings();
        }

        /// <summary>
        /// Name of cache settings. used when writing/reading tags to xml document
        /// </summary>
        public static string TagName { get { return "CachePreferences"; } }

        /// <summary>
        /// Load the cache settings from an xml document.
        /// Loads the global settings, data cache and instruction cache settings
        /// </summary>
        /// <param name="xmlIn">xml reader to read from</param>
		public void LoadFromXML(XmlReader xmlIn)
		{
            try
            {
                this.defaultSettings();
                xmlIn.MoveToContent();

                this.UnifiedCache = bool.Parse(xmlIn.GetAttribute("UnifiedCache"));

                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement) break;
                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        if (xmlIn.Name == DataCachePreferences.TagName)
                        {
                            DataCachePreferences.LoadFromXML(xmlIn.ReadSubtree());
                        }//if
                        else if (xmlIn.Name == InstructionCachePreferences.TagName)
                        {
                            InstructionCachePreferences.LoadFromXML(xmlIn.ReadSubtree());
                        }
                    }//if
                    xmlIn.Skip();
                } while (!xmlIn.EOF);
            }//try
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString(ex.Message);
                this.defaultSettings();
            }
        }//LoadFromXML

        /// <summary>
        /// Sets all cache settings to a default value
        /// </summary>
        public void defaultSettings()
        {
            DataCachePreferences.defaultSettings();
            InstructionCachePreferences.defaultSettings();
            this.UnifiedCache = false;
        }

        /// <summary>
        /// Save the cache settings to an xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
		public void saveState(XmlWriter xmlOut)
		{
            xmlOut.WriteStartElement(CachePreferences.TagName);
            xmlOut.WriteAttributeString("UnifiedCache", this.UnifiedCache.ToString());
            DataCachePreferences.saveState(xmlOut);
            InstructionCachePreferences.saveState(xmlOut);
            xmlOut.WriteEndElement();
		}
    }//class cachePreferences
}
