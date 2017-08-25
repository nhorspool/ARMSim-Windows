using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    public interface IScriptingInterface
    {
        void Restart();
        ICodeLabels GetCodeLabels { get; }
        IMemoryBlock MainMemory { get; }
        //bool Stopped { get; }
        void Execute();

        IGPR GPR { get; }

        bool Load(string[] fileNames);

        IDictionary<string, IList<IErrorReport>> GetErrorReports { get; }
    }
}
