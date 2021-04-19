using System;
using System.IO;
using A01.Processors.IRTPC;
using A01.Processors.IRTPC.v01;

namespace A01
{
    /*
     * REFERENCE: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
     * REFERENCE: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
     * sbyte : int8
     * byte : uint8
     * short : int16
     * ushort : uint16
     * int : int32
     * uint : uint32
     * long : int64
     * ulong : uint64
     */
    
    public class FileManager : IFileProcessor
    {
        public IClassIO FileProcessor { get; private set; }
        public string[] Paths { get; }

        public FileManager(string[] paths)
        {
            Paths = paths;
        }

        public void ProcessPaths()
        {
            for (int i = 0; i < Paths.Length; i++)
            {
                var path = Paths[i];
                
                Console.WriteLine($"[{i}/{Paths.Length}] Processing '{path}'");
                ProcessPath(path);
            }
        }

        public void ProcessPath(string fullPath)
        {
            var parentPath = Path.GetDirectoryName(fullPath);
            // Also gets final directory name if path is directory
            var pathName = Path.GetFileNameWithoutExtension(fullPath);
            
            IFileProcessor manager;
            var fourCC = new FourCCProcessor().GetFourCC(fullPath);
            switch (fourCC)
            {
                case EFoucCC.IRTPC:
                {
                    manager = new IRTPC_Manager(fullPath, parentPath, pathName); break;
                }
                default:
                    manager = new IRTPC_Manager(fullPath, parentPath, pathName); break;
            }
            
            manager.ProcessPath(fullPath);
        }
    }
}