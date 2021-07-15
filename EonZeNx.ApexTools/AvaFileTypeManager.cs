using System;
using System.IO;
using EonZeNx.ApexTools.Core.Refresh;

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
    
    public class AvaFileTypeManager
    {
        public string[] Paths { get; }
        
        public AvaFileTypeManager(string[] paths)
        {
            Paths = paths;
        }
        
        public void ProcessPaths()
        {
            for (var i = 0; i < Paths.Length; i++)
            {
                var path = Paths[i];
                var dir = Path.GetDirectoryName(path) ?? path;
                
                var fnWoExt = Path.GetFileNameWithoutExtension(path) ?? path;
                var ext = Path.GetExtension(path) ?? path;
                
                Console.WriteLine($"[{i}/{Paths.Length}] Processing '{path}'");
                var step = new MoveStep(path, Path.Combine(dir, $"{fnWoExt}_test{ext}"));

                var result = step.Execute();
                Console.Write($"{result.Result.ToString()}: {result.Message}");
            }
        }
    }
}