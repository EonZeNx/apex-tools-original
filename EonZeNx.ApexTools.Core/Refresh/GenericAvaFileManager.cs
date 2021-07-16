using System;
using System.Collections.Generic;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public abstract class GenericAvaFileManager : IAvaFileManager, IAvaFileManagerStackable
    {
        public EFourCc FourCc { get; }
        public int Version { get; }
        public string Path { get; set; }
        public IAvaFileManagerStackable ParentManager { get; set; }


        public GenericAvaFileManager(EFourCc fourCc, int version, string path = "", IAvaFileManagerStackable parentManager = null)
        {
            FourCc = fourCc;
            Version = version;
            Path = path;
            ParentManager = parentManager;
        }


        public abstract StepResult Process();
        
        public Stack<Tuple<EFourCc, int>> GetManagerInfo(Stack<Tuple<EFourCc, int>> priorInfo)
        {
            priorInfo.Push(new Tuple<EFourCc, int>(FourCc, Version));

            return ParentManager == null ? priorInfo : ParentManager.GetManagerInfo(priorInfo);
        }
        
        public Stack<Tuple<EFourCc, int>> GetManagerInfo()
        {
            var priorInfo = new Stack<Tuple<EFourCc, int>>();

            return GetManagerInfo(priorInfo);
        }

        protected abstract StepResult BinaryDeserialize();
        protected abstract StepResult ConvertedSerialize();
        protected abstract StepResult ConvertedDeserialize();
        protected abstract StepResult BinarySerialize();
    }
}