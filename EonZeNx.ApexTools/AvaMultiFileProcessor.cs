using System;
using System.Collections.Generic;
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
    public class AvaMultiFileProcessor
    {
        private string[] Paths { get; }
        
        public AvaMultiFileProcessor(string[] paths)
        {
            Paths = paths;
        }
        
        public void ProcessPaths()
        {
            for (var i = 0; i < Paths.Length; i++)
            {
                var path = Paths[i];
                var singleFileProc = new AvaSingleFileProcessor();
                singleFileProc.ProcessFile(path);
                
                Console.WriteLine($"[{i}/{Paths.Length}] Processing '{path}'");
            }
        }
    }
}