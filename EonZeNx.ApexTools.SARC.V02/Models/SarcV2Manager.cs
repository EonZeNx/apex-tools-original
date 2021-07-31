using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.SARC.V02.Models
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

        public override EFourCc FourCc => EFourCc.Sarc;
        public override int Version => 2;
        public override string Extension { get; set; }
        public override string DefaultExtension { get; set; } = ".sarc";

        private static string FileListName => "@files";
        private static uint HeaderLength => 4;
        public string FilePath { get; set; }

        private Entry[] Entries { get; set; }
        private string[] Ignore { get; set; }

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

        private void XmlWriteEntries(XmlWriter xw, string directoryPath)
        {
            xw.WriteStartElement("References");
            
            foreach (var entry in Entries)
            {
                XmlWriteEntry(xw, entry);
                entry.FolderSerialize(directoryPath);
            }
            
            xw.WriteEndElement();
        }
        
        private void XmlWriteEntry(XmlWriter xw, Entry entry)
        {
            // Only write references, can just gather contents of folder for internal files
            // if (!entry.IsReference) return;
            
            xw.WriteStartElement("Entry");
            xw.WriteAttributeString("Size", $"{entry.Size}");
            xw.WriteAttributeString("IsRef", $"{entry.IsReference}");
            xw.WriteValue(entry.Path);
            xw.WriteEndElement();
        }
        
        private void XmlWriteIgnores(XmlWriter xw)
        {
            // Ignore these extensions when deserializing the folder
            xw.WriteStartElement("Ignore");
            xw.WriteElementString("Extension", ".xml");
            xw.WriteElementString("Extension", ".bak");
            xw.WriteEndElement();
        }

        private void XmlLoadReferences(XmlReader xr)
        {
            var basePath = FilePath;
            if (Path.HasExtension(FilePath)) basePath = Path.GetDirectoryName(FilePath) ?? FilePath;
            
            var entries = new List<Entry>();
            xr.ReadToDescendant("References");
            xr.ReadToDescendant("Entry");

            do
            {
                if (xr.NodeType != XmlNodeType.Element) continue;
                if (xr.Name != "Entry") break;

                var entry = new Entry();
                entry.XmlLoadReference(xr);
                FolderLoadV2(entry, basePath);
                entries.Add(entry);
            } while (xr.ReadToNextSibling("Entry"));
            xr.ReadEndElement();
            
            Entries = entries.ToArray();
        }
        
        private void XmlLoadIgnore(XmlReader xr)
        {
            var ignore = new List<string>();
            xr.Read();

            do
            {
                var tag = xr.Name;
                var nodeType = xr.NodeType;

                if (nodeType == XmlNodeType.Whitespace) continue;
                if (tag == "Ignore" && nodeType == XmlNodeType.EndElement) break;

                ignore.Add(xr.ReadString());
            } while (xr.Read());

            Ignore = ignore.ToArray();
        }
        
        /// <summary>
        /// Loads the internal files
        /// </summary>
        private void FolderLoad()
        {
            var basePath = FilePath;
            if (Path.HasExtension(FilePath)) basePath = Path.GetDirectoryName(FilePath) ?? FilePath;
            
            var files = Directory.GetFiles(basePath, "", SearchOption.AllDirectories);

            // For each filepath in files...
            // Where !(Filepath contains any in Ignore)...
            // Keep filepath...
            files = (
                from filepath in files 
                where !Ignore.Any(filepath.Contains)
                select filepath
            ).ToArray();
            
            var newEntries = new List<Entry>();
            newEntries.AddRange(Entries);
            
            foreach (var filepath in files)
            {
                var localFilePath = filepath.Replace(@$"{basePath}\", "");
                var entry = new Entry(localFilePath);
                entry.FolderDeserialize(filepath);
                
                newEntries.Add(entry);
            }
            
            Entries = newEntries.ToArray();
        }

        private void FolderLoadV2(Entry entry, string basePath)
        {
            entry.FolderDeserialize(Path.Combine(basePath, entry.Path));
        }
        
        private void XmlDeserialize(XmlReader xr)
        {
            XmlLoadReferences(xr);
            // XmlLoadIgnore(xr);
            // FolderLoad();
        }

        #endregion
        
        #region Public Functions

        public override void Deserialize(string path)
        {
            if (Directory.Exists(path)) path = Path.Combine(path, $"{FileListName}.xml");
            FilePath = path;
            
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
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            bw.Write((uint) 4);
            bw.Write(ByteUtils.ReverseBytes((uint) EFourCc.Sarc));
            bw.Write((uint) Version);

            var dataOffset = (uint) Entries.Sum(item => item.HeaderSize);
            bw.Write(dataOffset);
            
            bw.Seek((int) dataOffset, SeekOrigin.Current);

            foreach (var entry in Entries)
            {
                entry.BinarySerializeData(bw);
            }

            bw.Seek(16, SeekOrigin.Begin);
            foreach (var entry in Entries)
            {
                entry.BinarySerialize(bw);
            }

            return ms.ToArray();
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

            XmlWriteEntries(xw, directoryPath);
            XmlWriteIgnores(xw);
            
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