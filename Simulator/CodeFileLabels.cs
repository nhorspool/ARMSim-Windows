using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Simulator
{
    /// <summary>
    /// This class maintains information about a single file of a program.
    /// It keeps track of data and text sections labels.
    /// </summary>
    public class CodeFileLabels
    {
        //provides a label to memory offset table
        private IDictionary<string, uint> _dataLabelsToAddress = new Dictionary<string, uint>();
        private IDictionary<string, uint> _codeLabelsToAddress = new Dictionary<string, uint>();
        private IDictionary<uint, ArmAssembly.SyEntry> _lineToLabel = new Dictionary<uint, ArmAssembly.SyEntry>();

        /// <summary>
        /// readonly properties allow access to clients needing data section info
        /// </summary>
        public IDictionary<string, uint> DataLabelsToAddress { get { return _dataLabelsToAddress; } }
        public IDictionary<string, uint> CodeLabelsToAddress { get { return _codeLabelsToAddress; } }
        public IDictionary<uint, ArmAssembly.SyEntry> LineNumberToLabel { get { return _lineToLabel; } }

        /// <summary>
        /// Translates a label to a memory offset
        /// if a label ends with a ":", simply strip it off
        /// </summary>
        /// <param name="label">label to translate</param>
        /// <param name="address">returned address</param>
        /// <returns>true if successful</returns>
        public bool LabelToAddress(string label, ref uint address)
        {
            //get a lower case version(all labels are stored in lower case)
            string str = label.Trim().ToLower();

            //if it has a : on the end, strip it off
            if (str.EndsWith(":"))
            {
                str = str.Substring(0, str.Length - 1);
            }

            //check if the key exists, if not, return invalid
            if (_dataLabelsToAddress.ContainsKey(str))
            {
                //otherwise look up the address and return it
                address = (uint)_dataLabelsToAddress[str];
                return true;
            }
            //check if the key exists, if not, return invalid
            if (_codeLabelsToAddress.ContainsKey(str))
            {
                //otherwise lookup the address and return it
                address = (uint)_codeLabelsToAddress[str];
                return true;
            }
            return false;
        }//LabelToAddress

        /// <summary>
        /// Loads the assembler symbols into the internal tables
        /// Only interested in the code and data section labels.
        /// Store labels as lower case
        /// [Modified by NH so that duplicate keys in the hashtables don't crash]
        /// </summary>
        /// <param name="afi">arm assembler file to load</param>
        public void Load(ArmAssembly.ArmFileInfo armFileInfo)
        {
            //ARMPluginInterfaces.Utils.OutputDebugString("");
            foreach (ArmAssembly.SyEntry se in armFileInfo.LocalSymTable.Values)
            {
                switch (se.Section)
                {
                    case ArmAssembly.SectionType.Text:
                        //ARMPluginInterfaces.Utils.OutputDebugString(String.Format(
                        //    "Text: Name:{0} Kind:{1} Line:{2} SubSection:{3}"+
                        //    " Section:{4} Value:0x{5:X}",
                        //    se.Name,se.Kind,se.LineNumber,
                        //    se.SubSection,se.Section,se.Value));

                        if (se.Kind == ArmAssembly.SymbolKind.Label)
                        {
                            _codeLabelsToAddress[se.Name.ToLower()] = (uint)se.SymValue;
                            _lineToLabel[(uint)se.LineNumber] = se;
                        }//if
                        break;

                    case ArmAssembly.SectionType.Data:
                    case ArmAssembly.SectionType.Bss:
                        //ARMPluginInterfaces.Utils.OutputDebugString(String.Format(
                        //    "Data: Name:{0} Kind:{1} Line:{2} SubSection:{3}"+
                        //    " Section:{4} Value:0x{5:X}",
                        //    se.Name,se.Kind,se.LineNumber,
                        //    se.SubSection,se.Section,se.Value));

                        if (se.Kind == ArmAssembly.SymbolKind.Label)
                        {
                            _dataLabelsToAddress[se.Name.ToLower()] = (uint)se.SymValue;
                            _lineToLabel[(uint)se.LineNumber] = se;
                        }//if
                        break;

                    default: break;
                }//switch
            }//foreach
        }//Load
    }//class CodeFileLabels

}
