using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using ARMPluginInterfaces.Preferences;

namespace ARMSim.Preferences
{
    public class PluginPreferences : IViewXMLSettings, ARMPluginInterfaces.Preferences.IPluginPreferences
    {
        private const string mActivatedPluginAttribute = "ActivatedPlugin";
        private const string mNameAttribute = "Name";
        private const string mAssemblyAttribute = "Assembly";

        private List<PluginSettingsItem> mSettingsPlugins = new List<PluginSettingsItem>();

        /// <summary>
        /// pluginPreferences ctor
        /// Sets all preferences to a default value
        /// </summary>
        public PluginPreferences()
        {
            this.defaultSettings();
        }

        public ICollection<PluginSettingsItem> SettingsPlugins { get { return mSettingsPlugins; } }

        public void ClearPlugins()
        {
            mSettingsPlugins.Clear();
        }
		
		/*
		private PluginSettingsItem FindPlugin(string name, string assembly)
		{
			foreach (PluginSettingsItem p in mSettingsPlugins)
			{
				//TODO resolve paths of assembly before matching (so that ./assembly.dll and ././assembly.dll are treated the same)
				if (p.Name.Equals(name) && p.Assembly.Equals(assembly))
					return p;
			}
			return null;
		}
		*/
        
		public void AddPlugin(PluginSettingsItem item)
        {
			//BB - 08/20/2014 - Only add the plugin to the list if it is not already present.
			if (!mSettingsPlugins.Contains(item))
				mSettingsPlugins.Add(item);
        }


        public void EnableSWIExtendedInstructions()
        {
            AddPlugin(new PluginSettingsItem("AngelSWIInstructions", "ARMSim.exe"));
        }

        public void EnableSWIExtendedInstructions( string which )
        {
            if (which == null)
                return;
            if (which.StartsWith("A"))
                AddPlugin(new PluginSettingsItem("AngelSWIInstructions", "ARMSim.exe"));
            else if (which.StartsWith("L"))
                AddPlugin(new PluginSettingsItem("LegacySWIInstructions", "ARMSim.exe"));
        }

        /// <summary>
        /// Tagname of these settings. Used to write tags in output xml document
        /// </summary>
        public static string TagName{ get { return "Plugins"; } }

        /// <summary>
        /// Set all preferences to a default value
        /// </summary>
        public void defaultSettings()
        {
            mSettingsPlugins.Clear();
        }

        /// <summary>
        /// Load general preferences from an xml document
        /// </summary>
        /// <param name="xmlIn">xml reader to use</param>
        public void LoadFromXML(XmlReader xmlIn)
        {
            try
            {
                this.defaultSettings();
                xmlIn.MoveToContent();
                xmlIn.Read();

                do
                {
                    if (xmlIn.NodeType == XmlNodeType.EndElement)
                        break;

                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        //if the MRU list , hand off
                        if (xmlIn.Name == mActivatedPluginAttribute)
                        {
                            string name = xmlIn.GetAttribute(mNameAttribute);
                            string assembly = xmlIn.GetAttribute(mAssemblyAttribute);

                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(assembly))
                            {
                                AddPlugin(new PluginSettingsItem(name, assembly));

                            }

                        }//if

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
        /// Save general preferences to an xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(PluginPreferences.TagName);

            foreach (PluginSettingsItem item in mSettingsPlugins)
            {
                xmlOut.WriteStartElement(mActivatedPluginAttribute);
                //xmlOut.WriteStartElement(pluginItem.armPlugin.Name);
                xmlOut.WriteAttributeString(mNameAttribute, item.Name);
                xmlOut.WriteAttributeString(mAssemblyAttribute, item.Assembly);
                xmlOut.WriteEndElement();
            }//foreach
            xmlOut.WriteEndElement();

        }//saveState

    }//class pluginPreferences
}