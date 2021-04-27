using System.IO;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class PathUtils
    {
        /// <summary>
        /// Splits a path into 3, Parent, Name, Extension
        /// </summary>
        /// <param name="path">Full path, either file or directory</param>
        /// <returns>Tuple of ParentPath, PathName, Extension</returns>
        public static (string, string, string) SplitPath(string path)
        {
            string parentPath = Path.GetDirectoryName(path);
            // Also gets final directory name if path is directory
            string pathName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            
            return (parentPath, pathName, extension);
        }
    }
}