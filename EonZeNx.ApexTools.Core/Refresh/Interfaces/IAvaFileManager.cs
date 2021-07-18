using System.IO;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileManager
    {
        void Deserialize(string path);
        void Deserialize(byte[] contents);

        void Export(string path, HistoryInstance[] history);
        byte[] Export();
    }
}