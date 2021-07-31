using System.IO;
using EonZeNx.ApexTools.Core.Processors;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public abstract class GenericAvaFileBare : IAvaFileBare
    {
        #region Abstract Variables

        public abstract EFourCc FourCc { get; }
        public abstract int Version { get; }
        public abstract string Extension { get; set; }

        #endregion

        
        #region Interface Functions

        public abstract void Deserialize(string path);
        public abstract void Deserialize(byte[] contents);

        public abstract byte[] Export();

        #endregion
    }
}