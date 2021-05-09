using EonZeNx.ApexTools.Core.Interfaces.Serializable;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public interface IPropertyVariants : IBinarySerializable, IXmlSerializable, IDeferredSerializable
    {
        int NameHash { get; }
        EVariantType VariantType { get; }
        byte[] RawData { get; }
        long Offset { get; }
        uint Alignment { get; }
        bool Primitive { get; }
    }
}