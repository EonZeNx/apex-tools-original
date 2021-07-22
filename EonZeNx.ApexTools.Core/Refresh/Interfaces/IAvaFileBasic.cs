using System.IO;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileBasic
    {
        void Deserialize(string path);
        void Deserialize(Stream contents);

        Stream Export(HistoryInstance[] history = null);
    }
}