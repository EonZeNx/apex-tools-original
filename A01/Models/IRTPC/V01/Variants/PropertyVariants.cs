using System.IO;
using System.Xml;
using A01.Interfaces.Serializable;

namespace A01.Models.IRTPC.V01.Variants
{
    public abstract class PropertyVariants: IBinarySerializable, IXmlSerializable
    {
        public abstract int NameHash { get; set; }
        protected abstract EVariantType VariantType { get; set; }
        
        protected abstract long Offset { get; set; }
        public abstract void BinarySerialize(BinaryWriter bw);
        public abstract void BinaryDeserialize(BinaryReader br);
        public abstract void XmlSerialize(XmlWriter xw);
        public abstract void XmlDeserialize(XmlReader xr);
    }
}