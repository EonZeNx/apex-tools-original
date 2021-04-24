using System.IO;
using System.Xml.Serialization;
using A01.Processors;
using A01.Utils;

namespace A01.Models.IRTPC.V01
{
    public class IRTPC_V01 : ClassIO
    {
        /* ROOT
        * Version 01 : u8
        * Version 02 : u16
        * Object count : u16
        */
        
        [XmlAttribute]
        public override string FileType { get; set; } = "IRTPC";
        [XmlAttribute]
        public override int Version { get; set; } = 01;
        [XmlAttribute]
        public override string Extension { get; set; }
        
        protected long Offset { get; private set; }
        protected ushort ObjectCount { get; private set; }
        
        [XmlAttribute]
        public byte Version01 { get; set; }
        
        [XmlAttribute]
        public ushort Version02 { get; set; }
        public Container[] Containers { get; set; }


        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(Version01);
            bw.Write(Version02);
            bw.Write((ushort) Containers.Length);
            foreach (var container in Containers)
            {
                container.Serialize(bw);
            }
        }

        public override void Deserialize(BinaryReader br)
        {
            Offset = BinaryReaderUtils.Position(br);
            Version01 = br.ReadByte();
            Version02 = br.ReadUInt16();
            ObjectCount = br.ReadUInt16();

            Containers = new Container[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                Containers[i] = new Container();
                Containers[i].Deserialize(br);
            }
        }
    }
}