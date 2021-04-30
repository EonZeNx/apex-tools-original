using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Vec4 : FloatArrayVariant
    {
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
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
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }
    }
}