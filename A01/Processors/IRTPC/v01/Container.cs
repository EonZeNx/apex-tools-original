using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using A01.Processors.IRTPC.v01.Variants;
using A01.Utils;
using String = A01.Processors.IRTPC.v01.Variants.String;
using UInt32 = A01.Processors.IRTPC.v01.Variants.UInt32;

namespace A01.Processors.IRTPC.v01
{
    public class Container : IClassIO
    {
        /* Structure : Container
         * Name hash : s32
         * Version 01 : u8
         * Version 02 : u16
         * Object count : u16
         * NOTE: Containers only contain properties
         */

        public int NameHash { get; private set; }
        public ushort ObjectCount { get; private set; }
        private byte Version01 { get; set; }
        private ushort Version02 { get; set; }
        private long Offset { get; set; }
        public PropertyVariants[] Properties { get; private set; }


        public void Serialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write(Version01);
            bw.Write(Version02);
            bw.Write((ushort) Properties.Length);
            foreach (var property in Properties)
            {
                property.Serialize(bw);
            }
        }

        public void Deserialize(BinaryReader br)
        {
            Offset = BinaryReaderUtils.Position(br);
            NameHash = br.ReadInt32();
            Version01 = br.ReadByte();
            Version02 = br.ReadUInt16();
            ObjectCount = br.ReadUInt16();

            Properties = new PropertyVariants[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                var prop = new Property(br);
                switch (prop.Type)
                {
                    case EVariantType.UInteger32:
                        Properties[i] = new UInt32(prop); break;
                    case EVariantType.Float32:
                        Properties[i] = new F32(prop); break;
                    case EVariantType.String:
                        Properties[i] = new String(prop); break;
                    case EVariantType.Vec2:
                        Properties[i] = new Vec2(prop); break;
                    case EVariantType.Vec3:
                        Properties[i] = new Vec3(prop); break;
                    case EVariantType.Vec4:
                        Properties[i] = new Vec4(prop); break;
                    case EVariantType.Mat3X4:
                        Properties[i] = new Mat3X4(prop); break;
                    case EVariantType.Event:
                        Properties[i] = new Event(prop); break;
                    default:
                        throw new InvalidEnumArgumentException("Property type was not a valid variant.");
                }
                Properties[i].Deserialize(br);
            }
        }
    }
}













