using System;
using System.IO;
using A01.Processors.IRTPC.v01;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace A01.Processors.IRTPC
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

        public string Filetype = "IRTPC";
        public int Version = 01;
        public string Extension = "bin";
        public Root Root;
        
        public void LoadBinary(string path)
        {
            Extension = Path.GetExtension(path);
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
            throw new System.NotImplementedException();
        }

        public void ExportBinary(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}