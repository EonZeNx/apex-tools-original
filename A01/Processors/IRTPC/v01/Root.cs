using System.Collections.Generic;
using System.IO;
using A01.Utils;

namespace A01.Processors.IRTPC.v01
{
    public class Root : IClassIO
    {
        /* Structure : Root
        * Version 01 : u8
        * Version 02 : u16
        * Object count : u16
        */
        
        protected long Offset { get; private set; }
        protected ushort ObjectCount { get; private set; }
        public byte Version01 { get; private set; }
        public ushort Version02 { get; private set; }
        public Container[] Containers { get; private set; }


        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Version01);
            bw.Write(Version02);
            bw.Write((ushort) Containers.Length);
            foreach (var container in Containers)
            {
                container.Serialize(bw);
            }
        }

        public void Deserialize(BinaryReader br)
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