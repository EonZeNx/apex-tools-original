using EonZeNx.ApexTools.Core.Interfaces.Serializable;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public interface IPropertyVariants : IBinarySerializable, IXmlSerializable
    {
        int NameHash { get; }
        EVariantType VariantType { get; }
        byte[] RawData { get; }
        uint Offset { get; }
        uint Alignment { get; }
        bool Primitive { get; }
    }
}