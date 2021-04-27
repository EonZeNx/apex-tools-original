using EonZeNx.ApexTools.Core.Interfaces;

namespace EonZeNx.ApexTools.Core.Processors
{
    public abstract class FileProcessor : IFileProcessor
    {
        public abstract string FullPath { get; protected set; }
        public abstract string ParentPath { get; protected set; }
        public abstract string PathName { get; protected set; }
        public abstract string Extension { get; protected set; }
        
        public abstract string[] ConvertedExtensions { get; protected set; }

        public abstract string FileType { get; protected set; }
        public abstract int Version { get; protected set; }
        
        public abstract void GetClassIO(string path);
        public abstract bool FileIsBinary();
        
        public abstract void LoadBinary();
        public abstract void ExportConverted();
        public abstract void LoadConverted();
        public abstract void ExportBinary();
    }
}