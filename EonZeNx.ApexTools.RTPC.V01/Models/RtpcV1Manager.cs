using System;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models
{
    /// <summary>
    /// An <see cref="RTPC_V01"/> file
    /// <br/> Structure:
    /// <br/> FourCc - <see cref="T:EFourCC"/>
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Root container - <see cref="RTPC.V01.Models.Container"/>
    /// </summary>
    public class RtpcV1Manager : GenericAvaFileManager, IDatabaseConnection
    {
        #region Variables

        public override EFourCc FourCc { get; } = EFourCc.Rtpc;
        public override int Version { get; } = 1;
        public override string Extension { get; set; }
        public override string DefaultExtension { get; set; } = ".rtpc";
        public SQLiteConnection DbConnection { get; set; }
        private Container Root { get; set; }

        #endregion


        #region Helpers

        private void OpenDatabaseConnection()
        {
            if (!File.Exists($"{ConfigData.AbsolutePathToDatabase}")) return;
            
            var dataSource = @$"Data Source={ConfigData.AbsolutePathToDatabase}";
            DbConnection = new SQLiteConnection(dataSource);
            DbConnection.Open();
        }

        private void BinaryDeserialize(BinaryReader br)
        {
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            var block = br.ReadBytes(16);
            br.BaseStream.Seek(4 + 4, SeekOrigin.Begin);
            
            if (FilePreProcessor.ValidCharacterCode(block) != FourCc) throw new InvalidFileVersion();
            if (FilePreProcessor.GetVersion(block, FourCc) != Version) throw new InvalidFileVersion();
            
            Root.BinaryDeserialize(br);
        }

        private void XmlDeserialize(XmlReader xr)
        {
            xr.ReadToDescendant(Root.GetType().Name);
            Root.XmlDeserialize(xr);
        }

        #endregion
        
        #region Public Functions

        public override void Deserialize(string path)
        {
            Extension = Path.GetExtension(path);
            using var fs = new FileStream(path, FileMode.Open);
            Deserialize(BinaryReaderUtils.StreamToBytes(fs));
        }

        public override void Deserialize(byte[] contents)
        {
            // Pre-process file
            using var ms = new MemoryStream(contents);
            ms.Seek(0, SeekOrigin.Begin);
            using var br = new BinaryReader(ms);
            
            var firstBlock = br.ReadBytes(16);
            var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);

            OpenDatabaseConnection();
            
            Root = new Container(DbConnection);

            if (fourCc == EFourCc.Rtpc)
            {
                BinaryDeserialize(br);
            }
            else if (fourCc == EFourCc.Xml)
            {
                ms.Seek(0, SeekOrigin.Begin);
                
                var xr = XmlReader.Create(ms);
                XmlDeserialize(xr);
            }
            else
            {
                throw new NotSupportedException();
            }
            
            DbConnection.Close();
        }

        public override byte[] Export(HistoryInstance[] history = null)
        {
            throw new NotImplementedException();
        }

        public override byte[] ExportBinary()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            bw.Write(ByteUtils.ReverseBytes((uint) FourCc));
            bw.Write(Version);

            var originalOffset = bw.BaseStream.Position;
            bw.Seek(Container.ContainerHeaderSize, SeekOrigin.Current);
            Root.BinarySerializeData(bw);
            
            bw.Seek((int) originalOffset, SeekOrigin.Begin);
            Root.BinarySerialize(bw);

            return ms.ToArray();
        }

        public override byte[] ExportConverted(HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        public override void Export(string path, HistoryInstance[] history = null)
        {
            var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
            using var xw = XmlWriter.Create(path, settings);
            
            xw.WriteStartElement("AvaFile");
            XmlUtils.WriteHistory(xw, history, Extension);
            Root.XmlSerialize(xw);
            xw.WriteEndElement();
        }

        public override void ExportBinary(string path)
        {
            throw new NotImplementedException();
        }

        public override void ExportConverted(string path, HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}