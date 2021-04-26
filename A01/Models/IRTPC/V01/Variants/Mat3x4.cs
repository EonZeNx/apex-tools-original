using System;
using System.Collections.Generic;
using System.Xml;
using A01.Utils;

namespace A01.Models.IRTPC.V01.Variants
{
    public class Mat3X4 : FloatArrayVariant
    {
        public Mat3X4()
        {
            NUM = 12;
            VariantType = EVariantType.Mat3X4;
        }
        
        public Mat3X4(Property prop) : base(prop)
        {
            NUM = 12;
            VariantType = EVariantType.Mat3X4;
        }

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{HexUtils.IntToHex(NameHash)}");

            string[] strArray = new string[3];
            for (int i = 0; i < strArray.Length; i++)
            {
                var startIndex = i * 4;
                var endIndex = (i + 1) * 4;
                var values = Value[startIndex..endIndex];
                strArray[i] = string.Join(",", values);
            }
            xw.WriteValue(string.Join(", ", strArray));
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = HexUtils.HexToInt(nameHash);
            
            var floatString = xr.ReadString();
            var vectorString = floatString.Split(", ");

            var floats = new List<float>();
            foreach (var vector in vectorString)
            {
                var vecStr = vector.Split(",");
                var vecFloats = Array.ConvertAll(vecStr, float.Parse);
                
                floats.AddRange(vecFloats);
            }
            
            Value = floats.ToArray();
        }
    }
}