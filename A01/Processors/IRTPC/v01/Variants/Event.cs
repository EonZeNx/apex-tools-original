using System.Collections.Generic;
using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class Event : PropertyVariants
    {
        public (uint, uint)[] Value;
        
        public Event(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
            VariantType = EVariantType.Event;
        }

        public override void Serialize(BinaryWriter bw)
        {
            //
        }
        
        public override void Deserialize(BinaryReader br)
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
    }
}