using System.IO;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public record HistoryInstance
    {
        public EFourCc FourCc;
        public int Version;

        public HistoryInstance(EFourCc fourCc, int version)
        {
            FourCc = fourCc;
            Version = version;
        }
    }
    
    public abstract class GenericAvaFileManager : IAvaFileManager
    {
        #region Interface Functions

        public abstract void Deserialize(string path);
        
        public abstract void Deserialize(byte[] contents);
        
        public abstract void Export(string path, HistoryInstance[] history);
        
        public abstract byte[] Export();

        #endregion
        
        #region Abstract Functions

        protected abstract void DeserializeBinary();
        protected abstract void DeserializeConverted();
        
        protected abstract void ExportBinary();
        protected abstract void ExportConverted();

        #endregion
    }
}