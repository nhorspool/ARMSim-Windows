using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Preferences
{
    /// <summary>
    /// This class represents the general preferences for the ARMSim.
    /// </summary>
    public class GeneralPreferences : IViewXMLSettings, ARMPluginInterfaces.Preferences.IGeneralPreferences
    {
        ///Access properties to general preferences
        ///<summary>Syncronise cache with memory when simulation ends</summary>
        public bool SyncCacheOnExit { get; set; }
        ///<summary>Close all files when simulation ends</summary>
        public bool CloseFilesOnExit { get; set; }
        ///<summary>Filename of file to map to stdin stream</summary>
        public string StdinFileName { get; set; }
        ///<summary>Filename of file to map to stdout stream</summary>
        public string StdoutFileName { get; set; }
        ///<summary>Filename of file to map to stderr stream</summary>
        public string StderrFileName { get; set; }
        ///<summary>Flag indicating to overwrite stdout file</summary>
        public bool StdoutOverwrite { get; set; }
        ///<summary>Flag indicating to overwrite stderr file</summary>
        public bool StderrOverwrite { get; set; }

        /// <summary>
        /// generalPreferences ctor
        /// Sets all preferences to a default value
        /// </summary>
        public GeneralPreferences()
        {
            this.defaultSettings();
        }

        /// <summary>
        /// Set all preferences to a default value
        /// </summary>
        public void defaultSettings()
        {
            this.SyncCacheOnExit=false;
            this.CloseFilesOnExit = false;
            this.StdinFileName = null;
            this.StdoutFileName = null;
            this.StderrFileName = null;
            this.StdoutOverwrite = false;
            this.StderrOverwrite = false;
        }

        /// <summary>
        /// Tagname of these settings. Used to write tags in output xml document
        /// </summary>
        public static string TagName { get { return "GeneralPreferences"; } }

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
                    if (xmlIn.NodeType == XmlNodeType.EndElement) break;
                    if (xmlIn.NodeType == XmlNodeType.Element)
                    {
                        if (xmlIn.Name == "ExitSettings")
                        {
                            this.SyncCacheOnExit = bool.Parse(xmlIn.GetAttribute("SyncCache"));
                            this.CloseFilesOnExit = bool.Parse(xmlIn.GetAttribute("CloseFiles"));
                        }//if
                        else if (xmlIn.Name == "StandardFiles")
                        {
                            this.StdinFileName = xmlIn.GetAttribute("StdinFilename");
                            this.StdoutFileName = xmlIn.GetAttribute("StdoutFilename");
                            this.StderrFileName = xmlIn.GetAttribute("StderrFilename");
                            this.StdoutOverwrite = bool.Parse(xmlIn.GetAttribute("StdoutOverwrite"));
                            this.StderrOverwrite = bool.Parse(xmlIn.GetAttribute("StderrOverwrite"));
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
        /// Save general preferences to an xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public void saveState(XmlWriter xmlOut)
        {
            //Main Memory Tab
            xmlOut.WriteStartElement(GeneralPreferences.TagName);

            xmlOut.WriteStartElement("ExitSettings");
            xmlOut.WriteAttributeString("SyncCache", this.SyncCacheOnExit.ToString());
            xmlOut.WriteAttributeString("CloseFiles", this.CloseFilesOnExit.ToString());
            xmlOut.WriteEndElement();

            xmlOut.WriteStartElement("StandardFiles");
            if (this.StdinFileName != null && this.StdinFileName.Length > 0)
            {
                xmlOut.WriteAttributeString("StdinFilename", this.StdinFileName);
            }
            if (this.StdoutFileName != null && this.StdoutFileName.Length > 0)
            {
                xmlOut.WriteAttributeString("StdoutFilename", this.StdoutFileName);
            }
            xmlOut.WriteAttributeString("StdoutOverwrite", this.StdoutOverwrite.ToString());
            if (this.StderrFileName != null && this.StderrFileName.Length > 0)
            {
                xmlOut.WriteAttributeString("StderrFilename", this.StderrFileName);
            }
            xmlOut.WriteAttributeString("StderrOverwrite", this.StderrOverwrite.ToString());
            xmlOut.WriteEndElement();

            xmlOut.WriteEndElement();
        }//saveState

    }//class generalPreferences
}