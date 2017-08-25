#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;
using System.Collections.Generic;

namespace ARMSim.Simulator
{
    public class AddressLabelPair : ARMPluginInterfaces.IAddressLabelPair
    {
        public uint Address { get; set; }
        public string Label { get; set; }

        public AddressLabelPair(uint address, string label)
        {
            this.Address = address;
            this.Label = label;
        }

    }//class AddressLabelPair

	/// <summary>
	/// Maintains information about the code and data segment labels of the current program.
	/// Once loaded, the only service this class provides is to translate a label string
	///  to a memory offset. This class is used to resolve user entered labels into memory offsets.
	/// </summary>
    public class CodeLabels : ARMPluginInterfaces.ICodeLabels
	{
        /// <summary>
        /// A table of all file sections.
        /// </summary>
        private IDictionary<string, CodeFileLabels> _fileSections = new Dictionary<string, CodeFileLabels>();

        /// <summary>
        /// Return a table of the file sections
        /// </summary>
        public IDictionary<string, CodeFileLabels> FileSections { get { return _fileSections; } }

        /// <summary>
        /// Return the data sections of the program
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IDictionary<string, uint> DataSectionLabels(string fileName) { return (_fileSections[fileName] as CodeFileLabels).DataLabelsToAddress; }
        public IDictionary<string, uint> CodeSectionLabels(string fileName) { return (_fileSections[fileName] as CodeFileLabels).CodeLabelsToAddress; }
        public IDictionary<uint, ArmAssembly.SyEntry> LineNumberToLabel(string fileName) { return (_fileSections[fileName] as CodeFileLabels).LineNumberToLabel; }

        /// <summary>
        /// Load the complete assembled program
        /// Loads each file section of the program
        /// </summary>
        /// <param name="ar"></param>
        public CodeLabels(ArmAssembly.ArmAssembler ar)
        {
            _fileSections.Clear();
            foreach (ArmAssembly.ArmFileInfo afi in ar.FileInfoTable)
            {
                _fileSections[afi.FileName] = new CodeFileLabels();
                (_fileSections[afi.FileName] as CodeFileLabels).Load(afi);
            }//foreach
        }

        public ARMPluginInterfaces.IAddressLabelPair LabelToAddress(string label)
        {
            uint address = 0;
		    if(!LabelToAddress(label,ref address))
                return null;

            return new AddressLabelPair(address, label);
        }

        /// <summary>
        /// Search all the loaded file for a specific label.
        /// </summary>
        /// <param name="label">label to search for</param>
        /// <param name="address">returned address</param>
        /// <returns>true if successful</returns>
		public bool LabelToAddress(string label,ref uint address)
        {
			foreach( string fileName in _fileSections.Keys )
            {
                if (_fileSections.ContainsKey(fileName) &&
                        _fileSections[fileName].LabelToAddress(label, ref address))
                    return true;
			}//foreach
			return false;
        }//LabelToAddress

        /// <summary>
        /// Return a list of address-label pairs sorted by address
        /// </summary>
        public AddressLabelPair[] CodeLabelList()
        {
            int size = 0;
            IDictionary<string, uint> cl;
            foreach (string filename in _fileSections.Keys)
            {
                cl = CodeSectionLabels(filename);
                size += cl.Count;
                cl = DataSectionLabels(filename);
                size += cl.Count;
            }
            AddressLabelPair[] result = new AddressLabelPair[size];
            int len = 0;
            System.Collections.IComparer cmp = new AddressPairComparer();
            foreach (string filename in _fileSections.Keys)
            {
                cl = CodeSectionLabels(filename);
                foreach (string x in cl.Keys)
                {
                    uint y = cl[x];
                    result[len++] = new AddressLabelPair(y, x);
                }
                cl = DataSectionLabels(filename);
                foreach (string x in cl.Keys)
                {
                    uint y = cl[x];
                    result[len++] = new AddressLabelPair(y, x);
                }
            }
            Array.Sort(result, cmp);
            return result;
        }

        public ARMPluginInterfaces.IAddressLabelPair[] GetCodeLabelList()
        {
            return CodeLabelList();
        }

        private class AddressPairComparer : System.Collections.IComparer
        {
            public int Compare(object v1, object v2)
            {
                AddressLabelPair p1 = (AddressLabelPair)v1;
                AddressLabelPair p2 = (AddressLabelPair)v2;
                return (int)p1.Address - (int)p2.Address;
            }
        }//class AddressPairComparer

    }//class CodeLabels
}
