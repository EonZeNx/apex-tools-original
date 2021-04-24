using System.IO;
using System.Xml.Serialization;
using A01.Interfaces;
using A01.Processors;

namespace A01.Models.IRTPC.V01.Variants
{
    public abstract class PropertyVariants: ISerializable
    {
        [XmlAttribute]
        public abstract int NameHash { get; set; }
        protected abstract EVariantType VariantType { get; set; }
        
        protected abstract long Offset { get; init; }
        public abstract void Serialize(BinaryWriter bw);
        public abstract void Deserialize(BinaryReader br);
    }
}