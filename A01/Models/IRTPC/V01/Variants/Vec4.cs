using System;
using System.Xml;
using A01.Utils;

namespace A01.Models.IRTPC.V01.Variants
{
    public class Vec4 : FloatArrayVariant
    {
        public Vec4()
        {
            NUM = 4;
            VariantType = EVariantType.Vec4;
        }
        
        public Vec4(Property prop) : base(prop)
        {
            NUM = 4;
            VariantType = EVariantType.Vec4;
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