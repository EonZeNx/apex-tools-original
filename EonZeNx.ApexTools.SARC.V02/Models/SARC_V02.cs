using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.SARC.V02.Models
{
    /// <summary>
    /// A <see cref="SARC_V02"/> file.
    /// <br/> Structure:
    /// <br/> Header Length - <see cref="uint"/>
    /// <br/> FourCC
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Data offset - <see cref="uint"/>
    /// <br/> Entries[]
    /// </summary>
    public class SARC_V02 : IFolderClassIO
    {
        public uint HeaderLength { get; set; }
        public uint Version { get; set; }
        public uint DataOffset { get; set; }
        public Entry[] Entries { get; set; }
        
        public string FileListName { get; set; } = "@files";
        public string[] Ignore { get; set; }

        public MetaInfo Minfo = new() {FileType = "SARC", Version = 2};
        
        
        public MetaInfo GetMetaInfo()
        {
            return Minfo;
        }

        
        #region Xml Load Helpers

        private void XmlLoadExternalReferences(XmlReader xr)
        {
            var entries = new List<Entry>();
            xr.ReadToDescendant("Entry");

            do
            {
                var tag = xr.Name;
                var nodeType = xr.NodeType;

                if (nodeType == XmlNodeType.Whitespace) continue;
                if (tag == "Ignore" && nodeType == XmlNodeType.Element) break;

                if (!xr.HasAttributes) throw new XmlException("Property missing attributes");

                var entry = new Entry();
                entry.XmlLoadExternalReference(xr);
                entries.Add(entry);
            } while (xr.Read());

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
        
        #endregion

        #region Folder Load Helpers

        private void FolderLoad(string basePath)
        {
            var files = Directory.GetFiles(basePath, "", SearchOption.AllDirectories);

            /* For each filepath in files...
             * Where !(Filepath contains any in Ignore)...
             * Keep filepath... */
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

        #endregion

        #region Binary Serialization

        public void StreamSerialize(BinaryWriter bw)
        {
            bw.Write((uint) 4);
            bw.Write(ByteUtils.ReverseBytes((uint) EFourCc.SARC));
            bw.Write((uint) Minfo.Version);

            DataOffset = (uint) Entries.Sum(item => item.HeaderSize);
            bw.Write(DataOffset);
            
            bw.Seek((int) DataOffset, SeekOrigin.Current);

            foreach (var entry in Entries)
            {
                entry.StreamSerializeData(bw);
            }

            bw.Seek(16, SeekOrigin.Begin);
            foreach (var entry in Entries)
            {
                entry.StreamSerialize(bw);
            }
        }

        public void StreamDeserialize(BinaryReader br)
        {
            HeaderLength = br.ReadUInt32();
            var fourCc = br.ReadUInt32();
            Version = br.ReadUInt32();
            DataOffset = br.ReadUInt32();

            var entries = new List<Entry>();
            while (br.BaseStream.Position < 4 + DataOffset)
            {
                var entry = new Entry();
                entry.StreamDeserialize(br);
                entries.Add(entry);
            }

            Entries = entries.ToArray();
        }

        #endregion

        #region Folder Serialization

        public void FolderSerialize(string basePath)
        {
            Directory.CreateDirectory(basePath);
            
            var settings = new XmlWriterSettings{Indent = true, IndentChars = "\t"};

            using (var xw = XmlWriter.Create(@$"{basePath}\@files.xml", settings))
            {
                xw.WriteStartElement(Minfo.GetType().Name);
                xw.WriteAttributeString("FileType", Minfo.FileType);
                xw.WriteAttributeString("Version", $"{Minfo.Version}");
                xw.WriteAttributeString("Extension", Minfo.Extension);
            
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
                
                    entry.FolderSerialize(basePath);
                }
            
                // Ignore these extensions when deserializing the folder
                xw.WriteStartElement("Ignore");
                xw.WriteElementString("Extension", ".xml");
                xw.WriteElementString("Extension", ".bak");
                xw.WriteEndElement();
                
                xw.Close();
            }
        }

        public void FolderDeserialize(string basePath)
        {
            var fileListPath = $@"{basePath}\{FileListName}.xml";
            var xr = XmlReader.Create(fileListPath);
            xr.ReadToDescendant(Minfo.GetType().Name);
            
            Minfo.Extension = XmlUtils.GetAttribute(xr, "Extension");
            Version = byte.Parse(XmlUtils.GetAttribute(xr, "Version"));

            XmlLoadExternalReferences(xr);
            XmlLoadIgnore(xr);
            FolderLoad(basePath);
        
            xr.Close();
        }

        #endregion
    }
}