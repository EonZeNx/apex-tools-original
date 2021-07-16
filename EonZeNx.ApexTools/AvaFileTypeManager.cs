using System;
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
                // Process file and determine file type
                var path = Paths[i];
                
                // Get AvaFileManager
                var factory = new AvaFileManagerFactory(path);
                var avaFileManager = factory.GetAvaFileManager();
                
                // Process AvaFile, amending the steps taken to the output XML
                avaFileManager.Process();
                
                
                Console.WriteLine($"[{i}/{Paths.Length}] Processing '{path}'");
            }
        }
    }
}