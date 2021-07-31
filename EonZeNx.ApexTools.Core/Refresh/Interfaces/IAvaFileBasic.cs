using System.IO;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileBasic
    {
        void Deserialize(string path);
        void Deserialize(byte[] contents);

        byte[] Export(HistoryInstance[] history = null);
    }
}