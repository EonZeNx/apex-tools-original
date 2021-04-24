using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using A01.Configuration;
using A01.Models.IRTPC.V01;
using A01.Processors;
using A01.Utils;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static A01.Program;


namespace A01.Models.IRTPC
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
        
        private ClassIO irtpc { get; set; }
        
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
            irtpc.Extension = Extension;
            using (var br = new BinaryReader(new FileStream(FullPath, FileMode.Open)))
            {
                irtpc.Deserialize(br);
            }
        }

        public override void ExportConverted()
        {
            string data;
            string extension;
            
            if (Config.GetPreferXmlOverYaml())
            {
                extension = "xml";
                var serializer = new ConfigurationContainer()
                    .Create();
                data = serializer.Serialize(new XmlWriterSettings {Indent = true}, irtpc);
            }
            else
            {
                extension = "yaml";
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                data = serializer.Serialize(irtpc);
            }
            
            using (var sw = new StreamWriter(new FileStream(@$"{ParentPath}\{PathName}.{extension}", FileMode.Create)))
            {
                sw.WriteLine(data);
            }
        }

        public override void LoadConverted()
        {
            if (Extension == ".xml")
            {
                var deserializer = new XmlSerializer(irtpc.GetType());
                using (var sr = new StreamReader(new FileStream(@$"{ParentPath}\{PathName}.xml", FileMode.Open)))
                {
                    switch (Version)
                    {
                        case 1:
                            irtpc = (IRTPC_V01) deserializer.Deserialize(sr); break;
                        default:
                            irtpc = (IRTPC_V01) deserializer.Deserialize(sr); break;
                    }
                }
            }
            else if (Extension == ".yaml")
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
            
                string yaml;
                using (var sr = new StreamReader(new FileStream(FullPath, FileMode.Open)))
                {
                    yaml = sr.ReadToEnd();
                }
            
                switch (Version)
                {
                    case 1:
                        irtpc = deserializer.Deserialize<IRTPC_V01>(yaml); break;
                    default:
                        irtpc = deserializer.Deserialize<IRTPC_V01>(yaml); break;
                }
            }
            else
            {
                throw new IOException($"'{FullPath}' was not a valid IRTPC file (XML or YAML)");
            }
        }

        public override void ExportBinary()
        {
            using (var bw = new BinaryWriter(new FileStream(@$"{ParentPath}\{PathName}.{Extension}", FileMode.Create)))
            {
                irtpc.Serialize(bw);
            }
        }
    }
}