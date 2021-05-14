using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public abstract class PropertyVariants: IBinarySerializable, IXmlSerializable
    {
        public abstract SQLiteConnection DbConnection { get; set; }
        public abstract int NameHash { get; set; }
        public abstract string Name { get; set; }
        public string HexNameHash => ByteUtils.IntToHex(NameHash);
        protected abstract EVariantType VariantType { get; set; }
        
        protected abstract long Offset { get; set; }
        public abstract void BinarySerialize(BinaryWriter bw);
        public abstract void BinaryDeserialize(BinaryReader br);
        public abstract void XmlSerialize(XmlWriter xw);
        public abstract void XmlDeserialize(XmlReader xr);
    }
}