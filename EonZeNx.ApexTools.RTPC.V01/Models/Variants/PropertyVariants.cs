using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public abstract class PropertyVariants : IStreamSerializable, IXmlSerializable, IDeferredSerializable
    {
        public abstract SQLiteConnection DbConnection { get; set; }
        public abstract string Name { get; set; }
        public abstract int NameHash { get; set; }
        public abstract string HexNameHash { get; }
        public abstract EVariantType VariantType { get; set; }
        public abstract byte[] RawData { get; set; }
        public abstract long Offset { get; set; }
        public abstract uint Alignment { get; }
        public abstract bool Primitive { get; }

        #region Binary Serialization

        public abstract void StreamSerializeData(Stream s);

        public abstract void StreamSerialize(Stream s);

        public abstract void StreamDeserialize(Stream s);

        #endregion

        #region XML Serialization

        public abstract void XmlSerialize(XmlWriter xw);

        public abstract void XmlDeserialize(XmlReader xr);

        #endregion
    }
}