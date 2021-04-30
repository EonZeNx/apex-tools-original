using System;
using EonZeNx.ApexTools.Core.Interfaces;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Models.Managers;

namespace EonZeNx.ApexTools
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
            var fourCC = new FourCCProcessor().GetFourCC(fullPath);
            return fourCC switch
            {
                EFoucCC.IRTPC =>  new IRTPC_Manager(),
                EFoucCC.RTPC =>   new RTPC_Manager(),
                _ =>              new IRTPC_Manager()
            };
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