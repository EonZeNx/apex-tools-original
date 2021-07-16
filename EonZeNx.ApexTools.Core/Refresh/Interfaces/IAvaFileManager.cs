using System;
using System.Collections.Generic;
using EonZeNx.ApexTools.Core.Processors;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileManager
    {
        EFourCc FourCc { get; }
        int Version { get; }
        
        StepResult Process();
        Stack<Tuple<EFourCc, int>> GetManagerInfo();
    }
}