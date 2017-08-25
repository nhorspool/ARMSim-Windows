using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    public interface IAddressLabelPair
    {
        uint Address { get; }
        string Label { get; }
    }
    
    public interface ICodeLabels
    {
        IAddressLabelPair LabelToAddress(string label);
        IAddressLabelPair[] GetCodeLabelList();
    }

}
