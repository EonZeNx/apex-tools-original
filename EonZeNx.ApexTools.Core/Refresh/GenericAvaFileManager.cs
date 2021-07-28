using System.IO;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public record HistoryInstance
    {
        public readonly EFourCc FourCc;
        public readonly int Version;

        public HistoryInstance(EFourCc fourCc, int version)
        {
            FourCc = fourCc;
            Version = version;
        }
    }
    
    public abstract class GenericAvaFileManager : IAvaFileBasic
    {
        #region Abstract Variables

        public abstract EFourCc FourCc { get; }
        public abstract int Version { get; }
        public abstract string Extension { get; set; }
        public abstract string DefaultExtension { get; set; }

        #endregion
        
        
        #region Interface Functions

        public abstract void Deserialize(string path);
        
        public abstract void Deserialize(byte[] contents);

        public abstract void Export(string path, HistoryInstance[] history = null);
        public abstract void ExportBinary(string path);
        public abstract void ExportConverted(string path, HistoryInstance[] history);

        public abstract byte[] Export(HistoryInstance[] history = null);
        public abstract byte[] ExportBinary();
        public abstract byte[] ExportConverted(HistoryInstance[] history);

        #endregion
    }
}