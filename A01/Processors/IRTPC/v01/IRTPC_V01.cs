using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace A01.Processors.IRTPC.v01
{
    public class IRTPC_V01 : IFileProcessor
    {
        /* Structure : Root
         * Version 01 : u8
         * Version 02 : u16
         * Object count : u16
         *
         * Structure : Container
         * Name hash : s32
         * Version 01 : u8
         * Version 02 : u16
         * Object count : u16
         * NOTE: Containers only contain properties
         *
         * Structure : Property
         * Name hash : s32
         * Type : u8
         * Value : VARIOUS (Use <T> on property and call like an Unreal array)
         */

        public static readonly string FILETYPE = "IRTPC";
        public static readonly int VERSION = 01;
        public static string EXTENSION = "bin";
        public Root Root;
        
        public void LoadBinary(string path)
        {
            EXTENSION = Path.GetExtension(path);
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                Root = new Root();
                Root.Deserialize(br);
            }
        }

        public void ExportConverted(string path)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(this);
            
            using (var sw = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                sw.WriteLine(yaml);
            }
        }

        public void LoadConverted(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            
            string yaml;
            using (var sr = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                yaml = sr.ReadToEnd();
            }

            Root = deserializer.Deserialize<Root>(yaml);
        }

        public void ExportBinary(string path)
        {
            using (var bw = new BinaryWriter(new FileStream(path, FileMode.Open)))
            {
                Root.Serialize(bw);
            }
        }

        public void ProcessPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}