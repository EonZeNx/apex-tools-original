using System;
using System.Collections.Generic;
using System.IO;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.Refresh;

namespace EonZeNx.ApexTools
{
    
    /// <summary>
    /// <see cref="sbyte"/>: int8<br/>
    /// <see cref="byte"/>: uint8<br/>
    /// <see cref="short"/>: int16<br/>
    /// <see cref="ushort"/>: uint16<br/>
    /// <see cref="int"/>: int32<br/>
    /// <see cref="uint"/>: uint32<br/>
    /// <see cref="long"/>: int64<br/>
    /// <see cref="ulong"/>: uint64<br/>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">Integral numeric types</a><br/>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">Floating point numeric types</a>
    /// </summary>
    public class AvaSingleFileProcessor
    {
        private static string FilelistName { get; } = "@files.xml";
        private List<HistoryInstance> History { get; } = new();

        #region Helpers

        private static string SetValidPath(string path)
        {
            if (File.Exists(path)) return path;
            
            if (!Directory.Exists(path)) throw new FileNotFoundException($"Neither file nor directory found: '{path}'");

            var atFiles = Path.Combine(path, FilelistName);
            if (!File.Exists(atFiles)) throw new FileNotFoundException($"Directory missing '{FilelistName}'");

            return atFiles;
        }

        #endregion

        public void ProcessXml(string path)
        {
            GenericAvaFileBare manager = new XmlManager();
            
            manager.Deserialize(path);
            
            if (path.Contains(FilelistName)) path = Path.GetDirectoryName(path);
            
            var fnWoExt = Path.GetFileNameWithoutExtension(path);
            var finalPath = Path.Combine(Path.GetDirectoryName(path) ?? "./", $"{fnWoExt}{manager.Extension}");

            using var bw = new BinaryWriter(new FileStream(finalPath, FileMode.Create));
            bw.Write(manager.Export());
        }
        
        public void ProcessFile(string path)
        {
            // File or directory?
            path = SetValidPath(path);
            
            // Pre-process file
            var fourCc = FilePreProcessor.GetCharacterCode(path);
            if (fourCc == EFourCc.Xml)
            {
                ProcessXml(path);
                return;
            }
            
            var fileManager = new AvaFileManagerFactory(path).Build();
            var version = fileManager.Version;
            
            History.Add(new HistoryInstance(fourCc, version));
            
            // Deserialize file
            fileManager.Deserialize(path);
            
            // If result FourCC is AvaFile, repeat
            if (fourCc == EFourCc.Aaf)
            {
                var newFm = ProcessFile(fileManager.ExportConverted());
                newFm.Extension = fileManager.Extension;
                fileManager = newFm;
            }

            var fnWoExt = Path.GetFileNameWithoutExtension(path);
            var finalPath = Path.Combine(Path.GetDirectoryName(path) ?? "./", $"{fnWoExt}{fileManager.Extension}");
            fileManager.Export(finalPath, History.ToArray());
        }

        private GenericAvaFileManager ProcessFile(byte[] contents)
        {
            while (true)
            {
                using var ms = new MemoryStream(contents);
                ms.Seek(0, SeekOrigin.Begin);
                
                // Pre-process file
                using var br = new BinaryReader(ms);
                
                var firstBlock = br.ReadBytes(16);
                var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);
                var version = FilePreProcessor.GetVersion(firstBlock, fourCc);
                var fileManager = new AvaFileManagerFactory(fourCc, version).Build();

                History.Add(new HistoryInstance(fourCc, version));

                // Deserialize file
                fileManager.Deserialize(ms.ToArray());

                // If FourCC is File-in-a-File, repeat
                if (fourCc == EFourCc.Aaf)
                {
                    contents = fileManager.Export();
                    continue;
                }

                return fileManager;
            }
        }
    }
}