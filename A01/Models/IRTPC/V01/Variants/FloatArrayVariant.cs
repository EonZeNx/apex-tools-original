using System;
using System.IO;
using System.Linq;
using System.Xml;
using A01.Utils;

namespace A01.Models.IRTPC.V01.Variants
{
    public class FloatArrayVariant : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; }
        protected override long Offset { get; set; }
        
        protected static int NUM = 2;
        public float[] Value;

        public FloatArrayVariant()
        {
            NUM = 2;
        }
        
        public FloatArrayVariant(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            foreach (var val in Value)
            {
                bw.Write(val);
            }
        }
    
        public override void BinaryDeserialize(BinaryReader br)
        {
            Value = new float[NUM];
            for (int i = 0; i < NUM; i++)
            {
                Value[i] = br.ReadSingle();
            }
        }

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{HexUtils.IntToHex(NameHash)}");

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = HexUtils.HexToInt(nameHash);
            var floatString = xr.ReadString();
            var floats = floatString.Split(",");
            Value = Array.ConvertAll(floats, input => float.Parse(input));
        }
    }
}