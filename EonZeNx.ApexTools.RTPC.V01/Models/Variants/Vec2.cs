using System.Buffers.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Vec2 : FloatArrayVariant
    {
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Vec2()
        {
            NUM = 2;
            VariantType = EVariantType.Vec2;
        }
        public Vec2(Property prop) : base(prop)
        {
            NUM = 2;
            VariantType = EVariantType.Vec2;
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