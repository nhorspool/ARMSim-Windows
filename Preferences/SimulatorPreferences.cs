using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces.Preferences;

namespace ARMSim.Preferences
{
    /// <summary>
    /// This class represents the preferences for the simulator engine.
    /// </summary>
    public class SimulatorPreferences : IViewXMLSettings, ISimulatorPreferences
    {
        /// <summary>
        /// Address of start of main memory
        /// </summary>
        public uint MemoryStart { get; set; }

        /// <summary>
        /// How much stack memory is there(words)
        /// </summary>
        public uint StackAreaSize { get; set; }

        /// <summary>
        /// How much heap memory is there(words)
        /// </summary>
        public uint HeapAreaSize { get; set; }

        /// <summary>
        /// Fill pattern to initialize main memory with
        /// </summary>
        public uint FillPattern { get; set; }

        /// <summary>
        /// Flag indicating simulator should stop on unaligned word or halfword access
        /// </summary>
        public bool StopOnMisaligned { get; set; }

        /// <summary>
        /// Flag indicating if the ARM sp is going to grow upwards or downwards.
        /// used to set initial stack area
        /// </summary>
        public bool StackGrowsDown { get; set; }

        /// <summary>
        /// Flag to indicate if the Text (program) area should be write protected.
        /// </summary>
        public bool ProtectTextArea { get; set; }

        /// <summary>
        /// simulatorPreferences ctor
        /// Sets all simulator preferences to default values
        /// </summary>
        public SimulatorPreferences()
        {
            this.defaultSettings();
        }

        /// <summary>
        /// Set all simulator preferences to default values
        /// </summary>
        public void defaultSettings()
        {
            this.MemoryStart = 0x1000;
            this.StackAreaSize = 32;
            this.HeapAreaSize = 32;
            this.FillPattern = 0x81818181;
            this.StopOnMisaligned = true;
            this.StackGrowsDown = true;
            this.ProtectTextArea = false;
        }

        /// <summary>
        /// Tagname of these settings. Used to write tags in output xml document
        /// </summary>
        public static string TagName { get { return "SimulatorPreferences"; }  }

        /// <summary>
        /// Load simulator preferences from an xml document
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
                        if (xmlIn.Name == "MainMemory")
                        {
                            this.MemoryStart = Convert.ToUInt32(xmlIn.GetAttribute("StartAddress"), 16);
                            this.StackAreaSize = Convert.ToUInt32(xmlIn.GetAttribute("StackArea"), 16);
                            this.HeapAreaSize = Convert.ToUInt32(xmlIn.GetAttribute("HeapArea"), 16);
                            this.FillPattern = Convert.ToUInt32(xmlIn.GetAttribute("FillPattern"), 16);
                            this.StopOnMisaligned = bool.Parse(xmlIn.GetAttribute("StopOnMisaligned"));

                            //new attribute added for 1.92, make sure we dont break old configs
                            string str = xmlIn.GetAttribute("StackGrowsDown") as string;
                            if (!string.IsNullOrEmpty(str))
                            {
                                this.StackGrowsDown = bool.Parse(str);
                            }
                            
                            //new attribute added for 1.92, make sure we dont break old configs
                            str = xmlIn.GetAttribute("ProtectTextArea") as string;
                            if (!string.IsNullOrEmpty(str))
                            {
                                this.ProtectTextArea = bool.Parse(str);
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
        }

        /// <summary>
        /// Save simulator preferences to an xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public void saveState(XmlWriter xmlOut)
        {
            //Main Memory Tab
            xmlOut.WriteStartElement(SimulatorPreferences.TagName);
            xmlOut.WriteStartElement("MainMemory");

            xmlOut.WriteAttributeString("StartAddress", this.MemoryStart.ToString("x8"));
            xmlOut.WriteAttributeString("StackArea", this.StackAreaSize.ToString("x4"));
            xmlOut.WriteAttributeString("HeapArea", this.HeapAreaSize.ToString("x4"));
            xmlOut.WriteAttributeString("FillPattern", this.FillPattern.ToString("x8"));
            xmlOut.WriteAttributeString("StopOnMisaligned", this.StopOnMisaligned.ToString());
            xmlOut.WriteAttributeString("StackGrowsDown", this.StackGrowsDown.ToString());
            xmlOut.WriteAttributeString("ProtectTextArea", this.ProtectTextArea.ToString());

            xmlOut.WriteEndElement();
            xmlOut.WriteEndElement();
        }

    }//class simulatorPreferences
}