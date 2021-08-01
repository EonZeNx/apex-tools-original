using System.IO;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileBare : IAvaFileSimple
    {
        void Deserialize(string path);
    }
}