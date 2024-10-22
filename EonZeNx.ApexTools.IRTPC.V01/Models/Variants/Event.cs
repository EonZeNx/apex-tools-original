﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.IRTPC.V01.Models.Variants
{
    public class Event : PropertyVariants
    {
        public override SQLiteConnection DbConnection { get; set; }
        public override int NameHash { get; set; }
        public override string Name { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.Event;
        protected override long Offset { get; set; }
        public (uint, uint)[] Value;
        
        public Event() { }
        public Event(Property prop)
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
            bw.Write((uint) Value.Length);
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
            
            // If valid connection, attempt to dehash
            if (DbConnection != null) Name = HashUtils.Lookup(DbConnection, NameHash);
        }

        #endregion

        #region XML Serialization

        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);
            
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

        public override void XmlDeserialize(XmlReader xr)
        {
            NameHash = XmlUtils.ReadNameIfValid(xr);

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

        #endregion
    }
}