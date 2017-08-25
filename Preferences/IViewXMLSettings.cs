using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

namespace ARMSim.Preferences
{
    /// <summary>
    /// This interface is implemented by any control that needs to load/save settings
    /// to/from an xml document. Views, Preferences are 2 eamples.
    /// </summary>
    public interface IViewXMLSettings
    {
        /// <summary>
        ///<summary>Save the objects settings to an xml document</summary>
        /// </summary>
        /// <param name="xmlOut">xml writer too use</param>
        void saveState(XmlWriter xmlOut);

        /// <summary>
        ///<summary>Load the objects settings from an xml document</summary>
        /// </summary>
        /// <param name="xmlIn">xml reader to use</param>
        void LoadFromXML(XmlReader xmlIn);

        /// <summary>
        /// Set all settings to a default state
        /// </summary>
        void defaultSettings();
    }//interface IViewXMLSettings
}
