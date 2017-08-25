using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

using ARMSim.Simulator;
using ARMPluginInterfaces.Preferences;

namespace ARMSim.Preferences
{
    /// <summary>
    /// This class represents the instruction cache preferences.
    /// </summary>
    //public class InstructionCachePreferences : IViewXMLSettings
    public class InstructionCachePreferences : ARMPluginInterfaces.Preferences.IInstructionCachePreferences
    {
        private uint _blockSize;				//size of 1 line in bytes
        private uint _numberBlocks;			    //number of blocks
        private uint _blocksPerSet;			    //number of blocks per set

        /// <summary>
        /// Block replacement strategy to use
        /// </summary>
        public ReplaceStrategiesEnum ReplaceStrategy { get; set; }

        ///<summary>flag indicating cache enabled</summary>
        ///
        public bool Enabled { get; set; }


        ///<summary>specifies the minimum cache block size.</summary>
        public const uint _minBlockSize = 4;
        ///<summary>specifies the maximum cache block size.</summary>
        public const uint _maxBlockSize = 128;
        ///<summary>specifies the maximum number of cache blocks</summary>
        public const uint _maxNumberBlocks = 256;
        ///<summary>specifies the maximum cache size in bytes</summary>
        public const uint _maxCacheSize = (uint)(_maxBlockSize * _maxNumberBlocks);

        /// <summary>
        /// instructionCachePreferences ctor
        /// </summary>
        public InstructionCachePreferences()
        {
            defaultSettings();
        }

        ///<summary>string name of this object, used for writing tags to xml document</summary>
        public virtual string TagName { get { return "InstructionCache"; } }

        /// <summary>
        /// Size of a block in words. Make sure its a power of 2 when writing
        /// </summary>
        public uint BlockSize
        {
            get { return _blockSize; }
            set
            {
                _blockSize = Math.Max(Math.Min(Utils.powerOf2(Utils.logBase2(value)), _maxBlockSize), _minBlockSize);
            }
        }

        /// <summary>
        /// Number of blocks in the cache. Make sure its a power of 2 when writing
        /// </summary>
        public uint NumberBlocks
        {
            get { return _numberBlocks; }
            set
            {
                _numberBlocks = (uint)Math.Max((int)Math.Min(Utils.powerOf2(Utils.logBase2(value)), (int)_maxNumberBlocks), 1);
            }
        }

        /// <summary>
        /// Number of blocks in one set. Make sure its a power of 2 when writing
        /// </summary>
        public uint BlocksPerSet
        {
            get { return _blocksPerSet; }
            set
            {
                _blocksPerSet = (uint)Math.Max((int)Math.Min(Utils.powerOf2(Utils.logBase2(value)), (int)_maxNumberBlocks), 1);
            }
        }

        /// <summary>
        /// Save the preference settings to the xml document
        /// </summary>
        /// <param name="xmlOut">xml writer to use</param>
        public void saveSettings(XmlWriter xmlOut)
        {
            xmlOut.WriteAttributeString("Enabled", this.Enabled.ToString());
            xmlOut.WriteAttributeString("BlockSize", this.BlockSize.ToString());
            xmlOut.WriteAttributeString("NumberBlocks", this.NumberBlocks.ToString());
            xmlOut.WriteAttributeString("BlocksPerSet", this.BlocksPerSet.ToString());
            xmlOut.WriteAttributeString("ReplaceStrategy", this.ReplaceStrategy.ToString());
        }//saveSettings

        /// <summary>
        /// Create the xml tag and insert the preference settings
        /// </summary>
        /// <param name="xmlOut"></param>
        public virtual void saveState(XmlWriter xmlOut)
        {
            xmlOut.WriteStartElement(this.TagName);
            saveSettings(xmlOut);
            xmlOut.WriteEndElement();
        }

        /// <summary>
        /// Load the preferences from an xml document
        /// </summary>
        /// <param name="xmlIn">xml reader to use</param>
        public virtual void LoadFromXML(XmlReader xmlIn)
        {
            xmlIn.MoveToContent();
            this.Enabled = bool.Parse(xmlIn.GetAttribute("Enabled"));
            this.BlockSize = uint.Parse(xmlIn.GetAttribute("BlockSize"));
            this.NumberBlocks = uint.Parse(xmlIn.GetAttribute("NumberBlocks"));
            this.BlocksPerSet = uint.Parse(xmlIn.GetAttribute("BlocksPerSet"));

            string rsString = xmlIn.GetAttribute("ReplaceStrategy");
            if (!string.IsNullOrEmpty(rsString))
            {
                object obj = Enum.Parse(typeof(ReplaceStrategiesEnum), rsString);
                if (obj != null)
                    this.ReplaceStrategy = (ReplaceStrategiesEnum)obj;
            }//if

        }//LoadFromXML

        /// <summary>
        /// Set preferences to a default value
        /// </summary>
        public virtual void defaultSettings()
        {
            this.Enabled = false;
            _blockSize = 16;
            _blocksPerSet = 1;
            _numberBlocks = 16;
            this.ReplaceStrategy = ReplaceStrategiesEnum.RoundRobin;
        }//defaultSettings
    }//class instructionCachePreference
}