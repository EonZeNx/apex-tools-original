using System;
using System.IO;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;

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
    public class AvaFileTypeManager
    {
        public string[] Paths { get; }
        
        public AvaFileTypeManager(string[] paths)
        {
            Paths = paths;
        }

        public int GetVersion(string path, EFourCc fourCc)
        {
            // Switch statement with version checks

            switch (fourCc)
            {
                case EFourCc.Rtpc:
                    break;
                case EFourCc.Irtpc:
                case EFourCc.Aaf:
                case EFourCc.Sarc:
                case EFourCc.Adf:
                case EFourCc.Tab:
                case EFourCc.Xml:
                default:
                    throw new NotImplementedException($"EFourCc not supported: '{fourCc}'");
            }
        }
        
        public IAvaFileManager GetAvaFileManager(string path, EFourCc fourCc, int version)
        {
            // Factory for AvaFileManager
            switch (fourCc)
            {
                case EFourCc.Rtpc:
                    return version switch
                    {
                        1 => new AvaRtpcV1Manager(path),
                        _ => throw new NotImplementedException($"Version not supported for this EFourCc: '{fourCc}'")
                    };
                case EFourCc.Irtpc:
                case EFourCc.Aaf:
                case EFourCc.Sarc:
                case EFourCc.Adf:
                case EFourCc.Tab:
                case EFourCc.Xml:
                default:
                    throw new NotImplementedException($"EFourCc not supported: '{fourCc}'");
            }
        }
        
        public void ProcessPaths()
        {
            for (var i = 0; i < Paths.Length; i++)
            {
                // Process file and determine file type
                var path = Paths[i];
                var fourCc = FourCCProcessor.GetCharacterCode(path);
                
                // If XML, perform steps
                if (fourCc == EFourCc.Xml) { /* Perform XML steps */ }
                
                // If AvaFile, determine version
                var version = GetVersion(path, fourCc);
                
                // Get AvaFileManager
                var avaFileManager = GetAvaFileManager(path, fourCc, version);
                
                // Process AvaFile, amending the steps taken to the output XML
                avaFileManager.Process();
                
                
                
                var dir = Path.GetDirectoryName(path) ?? path;
                
                var fnWoExt = Path.GetFileNameWithoutExtension(path) ?? path;
                var ext = Path.GetExtension(path) ?? path;
                
                Console.WriteLine($"[{i}/{Paths.Length}] Processing '{path}'");
                var step = new LaunchStep(path, "");

                var result = step.Execute();
                Console.Write($"{result.Result.ToString()}: {result.Message}");
            }
        }
    }
}