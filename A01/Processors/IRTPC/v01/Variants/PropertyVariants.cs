using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public abstract class PropertyVariants: IClassIO
    {
        public int NameHash { get; protected init; }
        public EVariantType VariantType { get; protected init; }
        protected long Offset { get; init; }
        public virtual void Serialize(BinaryWriter bw) {}
        public virtual void Deserialize(BinaryReader br) {}
    }
}