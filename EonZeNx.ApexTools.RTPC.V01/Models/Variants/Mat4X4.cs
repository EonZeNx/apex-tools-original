using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Mat4X4 : FloatArrayVariant
    {
        public override uint Alignment => 16;
        public override EVariantType VariantType => EVariantType.Mat4X4;
        public override int NUM => 16;
        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Mat4X4() { }
        public Mat4X4(Property prop) : base(prop) { }
        
        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            string[] strArray = new string[4];
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
    }
}