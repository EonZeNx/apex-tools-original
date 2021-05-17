using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.SARC.V02.Models
{
    /// <summary>
    /// A <see cref="SARC_V02"/> file.
    /// <br/> Structure:
    /// <br/> Header Length - <see cref="uint"/>
    /// <br/> FourCC
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Data offset - <see cref="uint"/>
    /// <br/> ENTRIES[]
    /// </summary>
    public class SARC_V02 : IFolderClassIO
    {
        public uint HeaderLength { get; set; }
        public uint Version { get; set; }
        public uint DataOffset { get; set; }
        public Entry[] Entries { get; set; }

        public MetaInfo Minfo = new() {FileType = "SARC", Version = 2};
        
        
        public MetaInfo GetMetaInfo()
        {
            return Minfo;
        }


        #region Binary Serialization

        public void BinarySerialize(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            HeaderLength = br.ReadUInt32();
            var fourCc = br.ReadUInt32();
            Version = br.ReadUInt32();
            DataOffset = br.ReadUInt32();

            var entries = new List<Entry>();
            while (br.BaseStream.Position < 4 + DataOffset)
            {
                var entry = new Entry();
                entry.BinaryDeserialize(br);
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
            var xw = XmlWriter.Create(@$"{basePath}\@files.xml", settings);
            
            xw.WriteStartElement($"{Minfo.GetType().Name}");
            xw.WriteAttributeString("FileType", Minfo.FileType);
            xw.WriteAttributeString("Version", $"{Minfo.Version}");
            xw.WriteAttributeString("Extension", Minfo.Extension);
            
            xw.WriteStartElement("Entries");
            
            foreach (var entry in Entries)
            {
                xw.WriteStartElement("Entry");
                xw.WriteAttributeString("Ref", $"{entry.IsReference}");
                xw.WriteAttributeString("Size", $"{entry.Size}");
                xw.WriteValue(entry.Path);
                xw.WriteEndElement();
                
                entry.FolderSerialize(basePath);
            }
            
            xw.Close();
        }

        public void FolderDeserialize(string basePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}