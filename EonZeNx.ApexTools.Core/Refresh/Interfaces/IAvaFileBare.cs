using System.IO;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileBare
    {
        void Deserialize(string path);
        void Deserialize(byte[] contents);

        byte[] Export();
    }
}