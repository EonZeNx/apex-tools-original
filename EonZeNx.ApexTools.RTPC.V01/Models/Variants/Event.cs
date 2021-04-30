using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Event : IPropertyVariants
    {
        public int NameHash { get; }
        public EVariantType VariantType => EVariantType.Event;
        public byte[] RawData { get; }
        public uint Offset { get; }
        public uint Alignment => 4;
        public bool Primitive => false;
        
        public (uint, uint)[] Value;

        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Event() { }
        public Event(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            RawData = prop.RawData;
        }


        public void BinarySerialize(BinaryWriter bw)
        {
            //
        }
        
        public void BinaryDeserialize(BinaryReader br)
        {
            var dataOffset = BitConverter.ToUInt32(RawData);
            
            br.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            
            var length = br.ReadUInt32();
            Value = new (uint, uint)[length];
            for (int i = 0; i < length; i++)
            {
                var firstEventHalf = br.ReadUInt32();
                var secondEventHalf = br.ReadUInt32();
                Value[i] = (firstEventHalf, secondEventHalf);
            }
        }

        public void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{ByteUtils.IntToHex(NameHash)}");
            
            string[] strArray = new string[Value.Length];
            for (int i = 0; i < Value.Length; i++)
            {
                var item1 = ByteUtils.UintToHex(Value[i].Item1);
                var item2 = ByteUtils.UintToHex(Value[i].Item2);
                strArray[i] = $"{item1}={item2}";
            }

            var array = string.Join(", ", strArray);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public void XmlDeserialize(XmlReader xr)
        {
            //
        }
    }
}