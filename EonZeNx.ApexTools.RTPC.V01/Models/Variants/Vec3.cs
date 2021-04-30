using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Vec3 : FloatArrayVariant
    {
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Vec3()
        {
            NUM = 3;
            VariantType = EVariantType.Vec3;
        }
        public Vec3(Property prop) : base(prop)
        {
            NUM = 3;
            VariantType = EVariantType.Vec3;
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