using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Event : IPropertyVariants
    {
        public int NameHash { get; set; }
        public EVariantType VariantType => EVariantType.Event;
        public byte[] RawData { get; }
        public long Offset { get; set; }
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
            if (Value == null)
            {
                bw.Write((uint) 0);
                return;
            }
            
            bw.Write(Value.Length);
            for (int i = 0; i < Value.Length; i++)
            {
                bw.Write(Value[i].Item1);
                bw.Write(Value[i].Item2);
            }
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
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = ByteUtils.HexToInt(nameHash);

            var value = xr.ReadString();
            if (value.Length == 0)
            {
                Value = Array.Empty<(uint, uint)>();
                return;
            }

            string[] eventStringArray = {value};
            if (value.Contains(","))
            {
                eventStringArray = value.Split(", ");
            }
            

            var events = new List<(uint, uint)>();
            foreach (var eventString in eventStringArray)
            {
                var eventStrings = eventString.Split("=");
                var eventsArray = Array.ConvertAll(eventStrings, ByteUtils.HexToUint);
                var eventsTuple = (eventsArray[0], eventsArray[1]);
                
                events.Add(eventsTuple);
            }

            Value = events.ToArray();
        }

        public long MemorySerializeData(MemoryStream ms, long offset)
        {
            var coffset = ByteUtils.Align(ms, offset, Alignment);
            Offset = coffset;
            
            ms.Write(BitConverter.GetBytes((uint) Value.Length));
            foreach (var (item1, item2) in Value)
            {
                ms.Write(BitConverter.GetBytes(item1));
                ms.Write(BitConverter.GetBytes(item2));
            }

            return coffset + 4 + (Value.Length * 2 * 4);
        }

        public void MemorySerializeHeader(MemoryStream ms)
        {
            ms.Write(BitConverter.GetBytes(NameHash));
            ms.Write(BitConverter.GetBytes((uint) Offset));
            ms.WriteByte((byte) VariantType);
        }
    }
}