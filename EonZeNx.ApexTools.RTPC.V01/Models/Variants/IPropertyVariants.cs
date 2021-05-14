using System;
using System.Data.SQLite;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    // TODO: Convert to abstract class
    public interface IPropertyVariants : IBinarySerializable, IXmlSerializable, IDeferredSerializable
    {
        SQLiteConnection DbConnection { get; set; }
        string Name { get; set; }
        int NameHash { get; }
        string HexNameHash => ByteUtils.IntToHex(NameHash);
        EVariantType VariantType { get; }
        byte[] RawData { get; }
        long Offset { get; }
        uint Alignment { get; }
        bool Primitive { get; }
    }
}