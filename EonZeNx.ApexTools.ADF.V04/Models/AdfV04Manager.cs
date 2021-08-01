using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;

namespace EonZeNx.ApexTools.ADF.V04.Models
{
    /// <summary>
    /// An <see cref="AdfV04Manager"/> file
    /// <br/> Structure:
    /// <br/> FourCc - <see cref="EFourCC"/>
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Root container - <see cref="RTPC.V01.Models.Container"/>
    /// </summary>
    public class AdfV04Manager : GenericAvaFileManager
    {
        #region Variables

        public override EFourCc FourCc => EFourCc.Adf;
        public override int Version => 4;
        public override string Extension { get; set; }
        public override string DefaultExtension { get; set; } = ".adf";

        #endregion


        #region Public Functions

        public override void Deserialize(string path)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(byte[] contents)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Export(HistoryInstance[] history = null)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] ExportBinary()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] ExportConverted(HistoryInstance[] history = null)
        {
            throw new System.NotImplementedException();
        }

        public override void Export(string path, HistoryInstance[] history = null)
        {
            throw new System.NotImplementedException();
        }

        public override void ExportBinary(string path)
        {
            throw new System.NotImplementedException();
        }

        public override void ExportConverted(string path, HistoryInstance[] history)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}