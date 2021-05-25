using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Utils;
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
            // If path is a file
            if (File.Exists(fullPath))
            {
                var fourCc = FourCCProcessor.GetCharacterCode(fullPath);
                return fourCc switch
                {
                    EFourCc.IRTPC =>  new IRTPC_Manager(),
                    EFourCc.RTPC =>   new RTPC_Manager(),
                    EFourCc.XML =>    GetXmlProcessor(fullPath),
                    EFourCc.AAF =>    new AAF_Manager(),
                    EFourCc.SARC =>   new AAF_Manager(),
                    _ =>              new IRTPC_Manager()
                };
            }

            // And a directory
            if (!Directory.Exists(fullPath))
                throw new FileNotFoundException($"Invalid path at GetFileProcessor: '{fullPath}'");
            
            // Load the @files.xml
            var fileListPath = $@"{fullPath}\@files.xml";
            if (!File.Exists(fileListPath))
                throw new FileNotFoundException($"@files.xml not found in '{fullPath}'");

            // And figure out which type it belongs to
            var xr = XmlReader.Create(fileListPath);
            xr.ReadToDescendant("MetaInfo");
            var filetype = XmlUtils.GetAttribute(xr, "FileType");
            var version = XmlUtils.GetAttribute(xr, "Version");
            xr.Close();

            return filetype switch
            {
                "SARC" => new SARC_Manager(int.Parse(version)),
                "ARC" => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };
        }

        private static FileProcessor GetXmlProcessor(string fullPath)
        {
            var path = @$"{fullPath}";
            var xr = XmlReader.Create(path);
            xr.MoveToContent();
            
            var fileType = XmlUtils.GetAttribute(xr, "FileType");
            xr.Close();
            
            // TODO: Create function to filter random files from potential IRTPC files
            return fileType switch
            {
                "IRTPC" => new IRTPC_Manager(),
                "RTPC" => new RTPC_Manager(),
                _ => new IRTPC_Manager()
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