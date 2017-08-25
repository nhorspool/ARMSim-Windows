using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces.Preferences;

namespace ARMSim.Preferences
{
    /// <summary>
    /// dataCachePreferences ctor
    /// Extends the instructionCachePreferences class adding write properties to the cache
    /// </summary>
    public class DataCachePreferences : InstructionCachePreferences, IDataCachePreferences
    {
        /// <summary>
        /// Access to cache write policy
        /// </summary>
        public WritePolicyEnum WritePolicy { get; set; }

        /// <summary>
        /// Access to cache allocate policy
        /// </summary>
        public AllocatePolicyEnum AllocatePolicy { get; set; }

        /// <summary>
        /// dataCachePreferences ctor
        /// Set all preferences to a default state
        /// </summary>
        public DataCachePreferences()
        {
            this.defaultSettings();
        }

        /// <summary>
        /// String name of this object. used when writing tags to xml
        /// </summary>
        public override string TagName { get { return "DataCache"; } }

        /// <summary>
        /// Save the preferences to an xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public override void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(this.TagName);

            base.saveSettings(xmlOut);

            xmlOut.WriteAttributeString("WritePolicy", this.WritePolicy.ToString());
            xmlOut.WriteAttributeString("AllocatePolicy", this.AllocatePolicy.ToString());

            xmlOut.WriteEndElement();
        }//saveState

        /// <summary>
        /// Set preferences to a default state
        /// </summary>
        public override void defaultSettings()
        {
            base.defaultSettings();
            this.WritePolicy = WritePolicyEnum.WriteThrough;
            this.AllocatePolicy = AllocatePolicyEnum.Both;
        }

        /// <summary>
        /// Load preference settings from xml document
        /// </summary>
        /// <param name="xmlIn">xml reader to use</param>
        public override void LoadFromXML(XmlReader xmlIn)
        {
            xmlIn.MoveToContent();
            base.LoadFromXML(xmlIn);

            string wpString = xmlIn.GetAttribute("WritePolicy");
            if (!string.IsNullOrEmpty(wpString))
            {
                object obj = Enum.Parse(typeof(WritePolicyEnum), wpString);
                if (obj != null)
                    this.WritePolicy = (WritePolicyEnum)obj;
            }//if

            string apString = xmlIn.GetAttribute("AllocatePolicy");
            if (!string.IsNullOrEmpty(apString))
            {
                object obj = Enum.Parse(typeof(AllocatePolicyEnum), apString);
                if (obj != null)
                    this.AllocatePolicy = (AllocatePolicyEnum)obj;
            }//if

        }//LoadFromXML

    }//class dataCachePreference
}
