using System.Buffers.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Vec2 : FloatArrayVariant
    {
        public override uint Alignment => 4;
        public override EVariantType VariantType => EVariantType.Vec2;
        public override int NUM => 2;
        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Vec2() { }
        public Vec2(Property prop) : base(prop) { }
        
        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }
    }
}