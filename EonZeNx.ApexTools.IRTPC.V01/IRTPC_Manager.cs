using System;
using System.IO;
using System.Linq;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.IRTPC.V01.Models;

namespace EonZeNx.ApexTools.IRTPC.V01
{
    public class IRTPC_Manager : FileProcessor
    {
        public override string FullPath { get; protected set; }
        public override string ParentPath { get; protected set; }
        public override string PathName { get; protected set; }
        public override string Extension { get; protected set; }
        public override string[] ConvertedExtensions { get; protected set; } = {".xml", ".yaml"};

        public override string FileType { get; protected set; }
        public override int Version { get; protected set; }
        
        private IXmlClassIO irtpc { get; set; }
        
        public override void GetClassIO(string path)
        {
            FullPath = path;
            (ParentPath, PathName, Extension) = PathUtils.SplitPath(path);

            int version;
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                version = br.ReadByte();
            }

            irtpc = version switch
            {
                1 => new IRTPC_V01(),
                _ => new IRTPC_V01()
            };
        }

        public override bool FileIsBinary()
        {
            return !ConvertedExtensions.Contains(Extension);
        }

        public override void LoadBinary()
        {
            irtpc.GetMetaInfo().Extension = Extension;
            using (var br = new BinaryReader(new FileStream(FullPath, FileMode.Open)))
            {
                irtpc.BinaryDeserialize(br);
            }
        }

        public override void ExportConverted()
        {
            string extension;
            
            extension = "xml";
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t"
            };
            XmlWriter xw = XmlWriter.Create(@$"{ParentPath}\{PathName}.{extension}", settings);
            irtpc.XmlSerialize(xw);
            xw.Close();
        }

        public void TempXmlDeserialize(XmlReader xr)
        {
            while (xr.Read())
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        Console.WriteLine("Start Element {0}", xr.Name);
                        break;
                    case XmlNodeType.Text:
                        Console.WriteLine("Text Node: {0}", xr.Value);
                        break;
                    case XmlNodeType.EndElement:
                        Console.WriteLine("End Element {0}", xr.Name);
                        break;
                    default:
                        Console.WriteLine("Other node {0} with value {1}", xr.NodeType, xr.Value);
                        break;
                }

                ConsoleUtils.GetInput("Waiting...");
            }
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

            switch (versionInt)
            {
                case 1:
                    irtpc = new IRTPC_V01(); break;
                default:
                    irtpc = new IRTPC_V01(); break;
            }
            
            irtpc.XmlDeserialize(xr);
        }

        public override void ExportBinary()
        {
            using (var bw = new BinaryWriter(new FileStream(@$"{ParentPath}\{PathName}{irtpc.GetMetaInfo().Extension}", FileMode.Create)))
            {
                irtpc.BinarySerialize(bw);
            }
        }
    }
}