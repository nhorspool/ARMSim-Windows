using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    public interface IARMSimArguments
    {
        string StdoutFileName { get; }
        string StdinFileName { get; }
        string StderrFileName { get; }
        string StdoutAppendMode { get; }
        string StderrAppendMode { get; }
    }
}
