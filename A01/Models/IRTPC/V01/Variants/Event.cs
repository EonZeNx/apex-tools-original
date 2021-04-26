using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using A01.Utils;
using ExtendedXmlSerializer;

namespace A01.Models.IRTPC.V01.Variants
{
    public class Event : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.Event;
        protected override long Offset { get; set; }
        public (uint, uint)[] Value;
        
        public Event() { }
        public Event(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            for (int i = 0; i < Value.Length; i++)
            {
                bw.Write(Value[i].Item1);
                bw.Write(Value[i].Item2);
            }
        }
        
        public override void BinaryDeserialize(BinaryReader br)
        {
            var length = br.ReadUInt32();
            Value = new (uint, uint)[length];
            for (int i = 0; i < length; i++)
            {
                var firstEventHalf = br.ReadUInt32();
                var secondEventHalf = br.ReadUInt32();
                Value[i] = (firstEventHalf, secondEventHalf);
            }
        }

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            xw.WriteAttributeString("NameHash", $"{HexUtils.IntToHex(NameHash)}");
            
            string[] strArray = new string[Value.Length];
            for (int i = 0; i < Value.Length; i++)
            {
                var item1 = HexUtils.UintToHex(Value[i].Item1);
                var item2 = HexUtils.UintToHex(Value[i].Item2);
                strArray[i] = $"{item1}={item2}";
            }

            var array = string.Join(", ", strArray);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }

        public override void XmlDeserialize(XmlReader xr)
        {
            var nameHash = XmlUtils.GetAttribute(xr, "NameHash");
            NameHash = HexUtils.HexToInt(nameHash);

            var value = xr.ReadString();
            if (value.Length == 0)
            {
                Value = new (uint, uint)[0];
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
                var eventsArray = Array.ConvertAll(eventStrings, HexUtils.HexToUint);
                var eventsTuple = (eventsArray[0], eventsArray[1]);
                
                events.Add(eventsTuple);
            }

            Value = events.ToArray();
        }
    }
}