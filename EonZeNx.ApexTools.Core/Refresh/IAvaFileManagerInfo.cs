using System;
using System.Collections.Generic;
using EonZeNx.ApexTools.Core.Processors;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public interface IAvaFileManagerStackable
    {
        Stack<Tuple<EFourCc, int>> GetManagerInfo(Stack<Tuple<EFourCc, int>> priorInfo);
    }
}