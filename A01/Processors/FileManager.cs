using System;
using A01.Interfaces;
using A01.Models.IRTPC;

namespace A01.Processors
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
    
    public class FileManager : IGetFileProcessor
    {
        public string[] Paths { get; }
        
        public FileManager(string[] paths)
        {
            Paths = paths;
        }
        
        public FileProcessor GetFileProcessor(string fullPath)
        {
            FileProcessor manager;
            var fourCC = new FourCCProcessor().GetFourCC(fullPath);
            switch (fourCC)
            {
                case EFoucCC.IRTPC:
                {
                    manager = new IRTPC_Manager(); break;
                }
                default:
                    manager = new IRTPC_Manager(); break;
            }
            
            return manager;
        }
        
        public void ProcessPaths()
        {
            for (int i = 0; i < Paths.Length; i++)
            {
                var path = Paths[i];
                
                Console.WriteLine($"[{i}/{Paths.Length}] Processing '{path}'");
                var manager = GetFileProcessor(path);
                manager.GetClassIO(path);

                if (manager.FileIsBinary())
                {
                    manager.LoadBinary();
                    manager.ExportConverted();
                }
                else
                {
                    manager.LoadConverted();
                    manager.ExportBinary();
                }
            }
        }
    }
}