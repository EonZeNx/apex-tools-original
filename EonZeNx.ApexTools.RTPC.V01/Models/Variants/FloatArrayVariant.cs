using System;
using System.IO;
using System.Xml;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class FloatArrayVariant : IPropertyVariants
    {
        public int NameHash { get; }
        public EVariantType VariantType { get; set; }
        public byte[] RawData { get; }
        public uint Offset { get; }
        public uint Alignment => 0;
        public bool Primitive => false;

        public int NUM = 2;
        public float[] Value;
        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public FloatArrayVariant() { }
        public FloatArrayVariant(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }
        
        
        public void BinarySerialize(BinaryWriter bw)
        {
            //
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            Value = new float[NUM];
            for (int i = 0; i < NUM; i++)
            {
                Value[i] = br.ReadSingle();
            }
        }

        public virtual void XmlSerialize(XmlWriter xw)
        {
            //
        }

        public virtual void XmlDeserialize(XmlReader xr)
        {
            //
        }
    }
}