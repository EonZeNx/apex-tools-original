using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Xml;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.RTPC.V01.Models;

namespace EonZeNx.ApexTools.Models.Managers
{
    public class RTPC_Manager : FileProcessor
    {
        public override string FullPath { get; protected set; }
        public override string ParentPath { get; protected set; }
        public override string PathName { get; protected set; }
        public override string Extension { get; protected set; }
        public override string[] ConvertedExtensions { get; protected set; } = {".xml", ".yaml"};

        public override string FileType { get; protected set; }
        public override int Version { get; protected set; }
        
        private IXmlClassIO rtpc { get; set; }
        
        public override void GetClassIO(string path)
        {
            FullPath = path;
            (ParentPath, PathName, Extension) = PathUtils.SplitPath(path);

            int version;
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                version = br.ReadByte();
            }

            rtpc = version switch
            {
                1 => new RTPC_V01(),
                _ => new RTPC_V01()
            };
        }

        public override bool FileIsBinary()
        {
            return !ConvertedExtensions.Contains(Extension);
        }

        private void LoadBinaryDeserialize()
        {
            var dataSource = @$"Data Source={ConfigData.AbsolutePathToDatabase}";
            if (File.Exists($"{ConfigData.AbsolutePathToDatabase}"))
            {
                using (var connection = new SQLiteConnection(dataSource))
                {
                    connection.Open();
                    
                    rtpc.DbConnection = connection;
                    using (var fr = new FileStream(FullPath, FileMode.Open))
                    {
                        rtpc.StreamDeserialize(fr);
                    }
                }

                return;
            }
            
            using (var fr = new FileStream(FullPath, FileMode.Open))
            {
                rtpc.StreamDeserialize(fr);
            }
        }

        public override void LoadBinary()
        {
            rtpc.GetMetaInfo().Extension = Extension;
            LoadBinaryDeserialize();
        }

        public override void ExportConverted()
        {
            var extension = "xml";
            var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
            var xw = XmlWriter.Create(@$"{ParentPath}\{PathName}.{extension}", settings);
            
            rtpc.XmlSerialize(xw);
            xw.Close();
        }

        public override void LoadConverted()
        {
            if (Extension != ".xml") throw new IOException($"'{FullPath}' was not a valid IRTPC file (XML or YAML)");

            var path = @$"{ParentPath}\{PathName}{Extension}";
            var xr = XmlReader.Create(path);
            xr.MoveToContent();

            var versionStr = XmlUtils.GetAttribute(xr, "Version");
            int versionInt;
            try
            {
                versionInt = int.Parse(versionStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); throw;
            }

            rtpc = versionInt switch
            {
                1 => new RTPC_V01(),
                _ => new RTPC_V01()
            };
            
            rtpc.XmlDeserialize(xr);
        }

        public override void ExportBinary()
        {
            using (var fw = new FileStream(@$"{ParentPath}\{PathName}{rtpc.GetMetaInfo().Extension}", FileMode.Create))
            {
                rtpc.StreamSerialize(fw);
            }
        }
    }
}