using System.Data.SQLite;
using System.IO;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.RTPC.V01.Models;

namespace EonZeNx.ApexTools.RTPC.V01.Refresh
{
    /// <summary>
    /// An <see cref="RTPC_V01"/> file
    /// <br/> Structure:
    /// <br/> FourCC - <see cref="EFourCC"/>
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Root container - <see cref="Container"/>
    /// </summary>
    public class RtpcV1Manager : GenericAvaFileManager, IDatabaseConnection
    {
        #region Variables

        public SQLiteConnection DbConnection { get; set; }
        private Container Root { get; set; }

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

        public override void Export(string path, HistoryInstance[] history)
        {
            throw new System.NotImplementedException();
        }

        public override BinaryReader Export()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Internal Functions

        protected override void DeserializeBinary()
        {
            throw new System.NotImplementedException();
        }

        protected override void DeserializeConverted()
        {
            throw new System.NotImplementedException();
        }

        protected override void ExportBinary()
        {
            throw new System.NotImplementedException();
        }

        protected override void ExportConverted()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}