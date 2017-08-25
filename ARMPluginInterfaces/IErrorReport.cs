using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    public interface IErrorReport
    {
        int Line { get; }
        int Col { get; }
        string ErrorMsg { get; }
    }
}
