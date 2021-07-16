using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.RTPC.V01.Models;

namespace EonZeNx.ApexTools.RTPC.V01.Refresh
{
    /// <summary>
    /// An <see cref="RTPC_V01"/> file
    /// <br/> Structure:
    /// <br/> FourCC
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Root container - <see cref="Container"/>
    /// </summary>
    public class AvaRtpcV1Manager : GenericAvaFileManager, IDatabaseConnection
    {
        #region Variables

        public SQLiteConnection DbConnection { get; set; }
        private Container Root { get; set; }

        #endregion
        
        
        public AvaRtpcV1Manager(string path = "") : base(EFourCc.Rtpc, 1, path) { }


        #region Functions

        private bool TryConnectToDatabase()
        {
            var dataSource = @$"Data Source={ConfigData.AbsolutePathToDatabase}";
            if (File.Exists($"{ConfigData.AbsolutePathToDatabase}"))
            {
                using var connection = new SQLiteConnection(dataSource);
                DbConnection = connection.OpenAndReturn();
            }

            return DbConnection is {State: ConnectionState.Open};
        }

        #endregion


        #region Override Functions
        
        public override StepResult Process()
        {
            return new ();
        }

        protected override StepResult BinaryDeserialize()
        {
            TryConnectToDatabase();
            
            using var br = new BinaryReader(new FileStream(Path, FileMode.Open));
            
            var offset = BinaryReaderUtils.Position(br);
            var fourCc = br.ReadInt32();
            var version = br.ReadInt32();
            
            Root = new Container(DbConnection);
            Root.BinaryDeserialize(br);
            
            return new ();
        }
        protected override StepResult ConvertedSerialize() { return new (); }
        protected override StepResult ConvertedDeserialize() { return new (); }

        protected override StepResult BinarySerialize()
        {
            TryConnectToDatabase();
            
            using var bw = new BinaryWriter(new FileStream(Path, FileMode.Create));
            
            bw.Write(Encoding.UTF8.GetBytes(Minfo.FileType));
            bw.Write(Version);

            var originalOffset = bw.BaseStream.Position;
            bw.Seek(Container.ContainerHeaderSize, SeekOrigin.Current);
            Root.BinarySerializeData(bw);
            
            bw.Seek((int) originalOffset, SeekOrigin.Begin);
            Root.BinarySerialize(bw);
            
            return new ();
        }
        
        #endregion
    }
}