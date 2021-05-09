using System;
using System.Globalization;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class OID : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.ObjectID;
        public byte[] RawData { get; }
        public long Offset { get; set; }
        public uint Alignment => 4;
        public bool Primitive => false;
        
        public (ulong, byte) Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public OID() { }
        public OID(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }


        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((uint) Offset);
            bw.Write((byte) VariantType);
        }
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            ByteUtils.Align(bw, Alignment);
            Offset = bw.BaseStream.Position;
            
            var data = Value.Item1 | Value.Item2;
            var first = (uint) (data >> 32);
            var second = (uint) (data & uint.MaxValue);
            
            bw.Write(BitConverter.GetBytes(second));
            bw.Write(BitConverter.GetBytes(first));
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

            var upper = ByteUtils.ReverseBytes(br.ReadUInt32());
            var lower = ByteUtils.ReverseBytes(br.ReadUInt32());
            
            // Thanks UnknownMiscreant
            var oid = ((ulong)upper) << 32 | lower;
            var userData = (byte)(oid & byte.MaxValue);
            
            Value = (oid, userData);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            var reversedOid = ByteUtils.ReverseBytes(Value.Item1);
            var stringOid = ByteUtils.UlongToHex(reversedOid);
            
            var full = $"{stringOid}={Value.Item2}";
            xw.WriteValue(full);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            var strValue = xr.ReadString();
            var strArray = strValue.Split("=");

            var reversedOid = ulong.Parse(strArray[0], NumberStyles.AllowHexSpecifier);
            var oid = ByteUtils.ReverseBytes(reversedOid);

            var userData = byte.Parse(strArray[1], NumberStyles.AllowHexSpecifier);
            
            Value = (oid, userData);
        }
    }
}