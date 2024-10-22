﻿using System.Xml;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Variants
{
    public class Vec4 : FloatArrayVariant
    {
        public override uint Alignment => 16;
        public override EVariantType VariantType => EVariantType.Vec4;
        public override int NUM => 4;

        
        /// <summary>
        /// Blank constructor for XML processing.
        /// </summary>
        public Vec4() { }
        public Vec4(Property prop) : base(prop) { }
        
        
        public override void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartElement($"{GetType().Name}");
            
            // Write Name if valid
            XmlUtils.WriteNameOrNameHash(xw, NameHash, Name);

            string array = string.Join(",", Value);
            xw.WriteValue(array);
            xw.WriteEndElement();
        }
    }
}