using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Mat3X3 : FloatArrayVariant
    {
        public override uint Alignment => 4;
        public override EVariantType VariantType => EVariantType.Mat4X4;
        public override int NUM => 9;
        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Mat3X3() { }
        public Mat3X3(Property prop) : base(prop) { }
        
        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            string[] strArray = new string[3];
            for (int i = 0; i < strArray.Length; i++)
            {
                var startIndex = i * 3;
                var endIndex = (i + 1) * 3;
                var values = Value[startIndex..endIndex];
                strArray[i] = string.Join(",", values);
            }
            xw.WriteValue(string.Join(", ", strArray));
            xw.WriteEndElement();
        }
    }
}