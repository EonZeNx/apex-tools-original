namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IFolderSerializable
    {
        public void FolderSerialize(string basePath);
        public void FolderDeserialize(string basePath);
    }
}