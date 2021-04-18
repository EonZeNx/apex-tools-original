using System.IO;

namespace A01.Processors.IRTPC
{
    public interface IClassIO
    {
        public void Serialize(BinaryWriter bw);
        public void Deserialize(BinaryReader br);
    }
}