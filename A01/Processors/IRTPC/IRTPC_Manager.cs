namespace A01.Processors.IRTPC
{
    public class IRTPC_Manager : IFileProcessor
    {
        public string FullPath { get; }
        public string ParentPath { get; }
        public string PathName { get; }
        
        public IRTPC_Manager(string fullPath, string parentPath, string pathName)
        {
            FullPath = fullPath;
            ParentPath = parentPath;
            PathName = pathName;
        }
        
        public void ProcessPath(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}