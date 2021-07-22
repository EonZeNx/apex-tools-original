using System.IO;
using System.Text.RegularExpressions;
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

        #endregion
        
        
        #region Interface Functions

        public abstract void Deserialize(string path);
        
        public abstract void Deserialize(Stream contents);

        public abstract void Export(string path, HistoryInstance[] history = null);
        public abstract void ExportBinary(string path);
        public abstract void ExportConverted(string path, HistoryInstance[] history);

        public abstract Stream Export(HistoryInstance[] history = null);
        public abstract Stream ExportBinary();
        public abstract Stream ExportConverted(HistoryInstance[] history);

        #endregion
    }
}