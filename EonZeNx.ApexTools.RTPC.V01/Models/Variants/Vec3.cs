using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Vec3 : FloatArrayVariant
    {
        public override uint Alignment => 4;
        public override EVariantType VariantType => EVariantType.Vec3;
        public override int NUM => 3;

        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Vec3() { }
        public Vec3(Property prop) : base(prop) { }
        
        
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