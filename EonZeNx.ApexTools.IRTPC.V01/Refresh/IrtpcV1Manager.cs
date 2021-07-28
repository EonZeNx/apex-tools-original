using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.IRTPC.V01.Models;

namespace EonZeNx.ApexTools.IRTPC.V01.Refresh
{
    /// <summary>
    /// An <see cref="IrtpcV1Manager"/> file
    /// <br/> Structure:
    /// <br/> Version 01 - <see cref="byte"/>
    /// <br/> Version 02 - <see cref="ushort"/>
    /// <br/> Object count - <see cref="ushort"/>
    /// <br/> Root container - <see cref="IRTPC.V01.Models.Container"/>
    /// </summary>
    public class IrtpcV1Manager : GenericAvaFileManager, IDatabaseConnection
    {
        #region Variables

        public override EFourCc FourCc { get; } = EFourCc.Irtpc;
        public override int Version { get; } = 1;
        public ushort Version02 { get; } = 4;
        public ushort ObjectCount { get; set; }
        public SQLiteConnection DbConnection { get; set; }
        private Container[] Containers { get; set; }

        #endregion


        #region Helpers

        private void SortContainers()
        {
            if (ConfigData.SortFiles)
            {
                Array.Sort(Containers, new ContainerComparer());
            }
        }

        private void OpenDatabaseConnection()
        {
            if (!File.Exists($"{ConfigData.AbsolutePathToDatabase}")) return;
            
            var dataSource = @$"Data Source={ConfigData.AbsolutePathToDatabase}";
            DbConnection = new SQLiteConnection(dataSource);
            DbConnection.Open();
        }

        private void BinaryDeserialize(BinaryReader br)
        {
            if (br.ReadByte() != Version) throw new InvalidFileVersion();
            if (br.ReadUInt16() != Version02) throw new InvalidFileVersion();
            ObjectCount = br.ReadUInt16();
            
            Containers = new Container[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                Containers[i] = new Container(DbConnection);
                Containers[i].BinaryDeserialize(br);
            }

            SortContainers();
        }
        
        private void XmlDeserialize(XmlReader xr)
        {
            xr.ReadToDescendant("Container");
            
            var containers = new List<Container>();
            xr.ReadToDescendant("Container");
            while (xr.NodeType == XmlNodeType.Element)
            {
                if (xr.NodeType != XmlNodeType.Element || xr.Name != "Container") break;
                
                var container = new Container();
                container.XmlDeserialize(xr);
                containers.Add(container);
                
                xr.ReadToNextSibling("Container");
                if (xr.NodeType == XmlNodeType.EndElement) xr.ReadToNextSibling("Container");
            }
            xr.Close();

            Containers = containers.ToArray();
            ObjectCount = (ushort) Containers.Length;
        }

        #endregion
        
        #region Public Functions

        public override void Deserialize(string path)
        {
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
            ms.Seek(0, SeekOrigin.Begin);
            var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);

            OpenDatabaseConnection();

            if (fourCc == EFourCc.Irtpc)
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
            
            bw.Write(Version);
            bw.Write(Version02);
            bw.Write(ObjectCount);
            foreach (var container in Containers)
            {
                container.BinarySerialize(bw);
            }

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
            XmlUtils.WriteHistory(xw, history);
            
            foreach (var container in Containers)
            {
                container.XmlSerialize(xw);
            }
            
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