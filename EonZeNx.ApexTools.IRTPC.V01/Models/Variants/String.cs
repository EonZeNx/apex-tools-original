using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public class String : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override int NameHash { get; set; }
        public override string Name { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.String;
        protected override long Offset { get; set; }
        public string Value;
        
        public String() { }
        public String(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            DbConnection = prop.DbConnection;
        }

        #region Binary Serialization

        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            bw.Write((ushort) Value.Length);
            bw.Write(Encoding.UTF8.GetBytes(Value));
        }
        
        public override void BinaryDeserialize(BinaryReader br)
        {
            var length = br.ReadUInt16();
            byte[] byteString = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteString[i] = br.ReadByte();
            }
            Value = Encoding.UTF8.GetString(byteString);
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        #endregion

        #region XML Serialization

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameIfValid(xw, NameHash, Name);
            
            xw.WriteValue(Value);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);
            Value = xr.ReadString();
        }

        #endregion
    }
}