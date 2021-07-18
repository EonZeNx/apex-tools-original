using System;
using System.Collections.Generic;
using System.IO;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
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
        private List<HistoryInstance> History { get; }  = new();

        public void ProcessFile(string path)
        {
            // Pre-process file
            var fourCc = FilePreProcessor.GetCharacterCode(path);
            var version = FilePreProcessor.GetVersion(path, fourCc);
            var fileManager = new AvaFileManagerFactory(fourCc, version).Build();
            
            History.Add(new HistoryInstance(fourCc, version));
            
            // Deserialize file
            fileManager.Deserialize(path);
            
            // If result FourCC is AvaFile, repeat
            if (fourCc == EFourCc.Aaf)
            {
                ProcessFile(fileManager.Export());
            }
            
            fileManager.Export("final path", History.ToArray());
        }

        private void ProcessFile(byte[] contents)
        {
            while (true)
            {
                // Pre-process file
                using var br = new BinaryReader(new MemoryStream(contents));
                
                var firstBlock = br.ReadBytes(16);
                var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);
                var version = FilePreProcessor.GetVersion(firstBlock, fourCc);
                var fileManager = new AvaFileManagerFactory(fourCc, version).Build();

                History.Add(new HistoryInstance(fourCc, version));

                // Deserialize file
                fileManager.Deserialize(contents);

                // If FourCC is File-in-a-File, repeat
                if (fourCc == EFourCc.Aaf)
                {
                    contents = fileManager.Export();
                    continue;
                }

                break;
            }
        }
    }
}