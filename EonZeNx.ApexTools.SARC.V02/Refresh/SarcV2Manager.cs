using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Xml;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.SARC.V02.Models;

namespace EonZeNx.ApexTools.SARC.V02.Refresh
{
    /// <summary>
    /// A <see cref="SarcV2Manager"/> file.
    /// <br/> Structure:
    /// <br/> Header Length - <see cref="uint"/>
    /// <br/> FourCC
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Data offset - <see cref="uint"/>
    /// <br/> Entries[]
    /// </summary>
    public class SarcV2Manager : GenericAvaFileManager
    {
        #region Variables

        public override EFourCc FourCc { get; } = EFourCc.Sarc;
        public override int Version { get; } = 2;
        public override string Extension { get; set; }
        public override string DefaultExtension { get; set; } = ".sarc";
        
        public string FileListName { get; } = "@files";
        public uint HeaderLength { get; } = 4;
        
        private Entry[] Entries { get; set; }

        #endregion


        #region Helpers

        private void BinaryDeserialize(BinaryReader br)
        {
            br.BaseStream.Seek(0, SeekOrigin.Begin);

            var block = br.ReadBytes(16);
            var fourCc = FilePreProcessor.ValidCharacterCode(block);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            
            if (br.ReadUInt32() != HeaderLength) throw new InvalidFileVersion();
            if (fourCc != FourCc) throw new InvalidFileVersion();
            br.ReadUInt32();
            if (br.ReadUInt32() != Version) throw new InvalidFileVersion();
            
            var dataOffset = br.ReadUInt32();
            var entries = new List<Entry>();
            while (br.BaseStream.Position < 4 + dataOffset)
            {
                var entry = new Entry();
                entry.BinaryDeserialize(br);
                entries.Add(entry);
            }

            Entries = entries.ToArray();
        }
        
        private void XmlDeserialize(XmlReader xr)
        {
            
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
            ms.Seek(0, SeekOrigin.Begin);
            var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);

            if (fourCc == EFourCc.Sarc)
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
        }

        public override byte[] Export(HistoryInstance[] history = null)
        {
            throw new NotImplementedException();
        }

        public override byte[] ExportBinary()
        {
            throw new NotImplementedException();
        }

        public override byte[] ExportConverted(HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        public override void Export(string path, HistoryInstance[] history = null)
        {
            var directoryPath = Path.Combine(Path.GetDirectoryName(path) ?? "./", Path.GetFileNameWithoutExtension(path));
            Directory.CreateDirectory(directoryPath);
            
            var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
            using var xw = XmlWriter.Create(Path.Combine(directoryPath, $"{FileListName}.xml"), settings);
            
            xw.WriteStartElement("AvaFile");
            XmlUtils.WriteHistory(xw, history, Extension);
        
            foreach (var entry in Entries)
            {
                // Only write references, can just gather contents of folder for internal files
                if (entry.IsReference)
                {
                    xw.WriteStartElement("Entry");
                    xw.WriteAttributeString("Size", $"{entry.Size}");
                    xw.WriteValue(entry.Path);
                    xw.WriteEndElement();
                }
            
                entry.FolderSerialize(directoryPath);
            }
        
            // Ignore these extensions when deserializing the folder
            xw.WriteStartElement("Ignore");
            xw.WriteElementString("Extension", ".xml");
            xw.WriteElementString("Extension", ".bak");
            xw.WriteEndElement();
            
            xw.Close();
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