﻿using System;
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
        public uint Offset { get; }
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
            var data = Value.Item1 | Value.Item2;
            bw.Write(ByteUtils.ReverseBytes((uint) (data >> 32)));
            bw.Write(ByteUtils.ReverseBytes((uint) (data & uint.MaxValue)));
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

            var upper = ByteUtils.ReverseBytes(br.ReadUInt32());
            var lower = ByteUtils.ReverseBytes(br.ReadUInt32());
            
            var oid = ((ulong)upper) << 32 | lower;
            var udata = (byte)(oid & byte.MaxValue);
            Value = (oid, udata);
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");

            var oid = $"{ByteUtils.UlongToHex(Value.Item1)}={Value.Item2}";
            xw.WriteValue(oid);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);
            var strValue = xr.ReadString();
            var strArray = strValue.Split("=");
            Value = (ulong.Parse(strArray[0], NumberStyles.AllowHexSpecifier), byte.Parse(strArray[1], NumberStyles.AllowHexSpecifier));
        }
    }
}