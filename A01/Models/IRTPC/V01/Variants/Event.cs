using System.IO;

namespace A01.Models.IRTPC.V01.Variants
{
    public class Event : PropertyVariants
    {
        public override int NameHash { get; set; }
        protected override EVariantType VariantType { get; set; } = EVariantType.Event;
        protected override long Offset { get; init; }
        public (uint, uint)[] Value;
        
        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="Event"></see>
        /// </summary>
        public Event()
        {
            
        }
        
        public Event(Property prop)
        {
            Offset = prop.Offset;
            NameHash = prop.NameHash;
        }

        public override void Serialize(BinaryWriter bw)
        {
            bw.Write(NameHash);
            bw.Write((byte) VariantType);
            for (int i = 0; i < Value.Length; i++)
            {
                bw.Write(Value[i].Item1);
                bw.Write(Value[i].Item2);
            }
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