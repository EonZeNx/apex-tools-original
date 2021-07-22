using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EonZeNx.ApexTools.Core.Processors
{
    public enum EFourCc
    {
        Rtpc = 0x52545043,
        Irtpc = 0x0,
        Aaf = 0x41414600,
        Sarc = 0x53415243,
        Adf = 0x20464441,
        Tab = 0x54414200,
        Xml = 0x3F786D6C
    }
    
    
    public static class FilePreProcessor
    {
        public static readonly Dictionary<int, EFourCc> CharacterCodes = Enum.GetValues<EFourCc>()
            .ToArray()
            .ToDictionary(val => (int) val, val => val);

        public static readonly Dictionary<string, EFourCc> StrToFourCc = Enum.GetValues<EFourCc>()
            .ToArray()
            .ToDictionary(val => val.ToString(), val => val);
        
        public static bool IsSupportedCharacterCode(int input)
        {
            var result = CharacterCodes.Any(pair => pair.Key == input);
            return result;
        }

        public static EFourCc ValidCharacterCode(byte[] input)
        {
            for (var i = 4; i <= input.Length; i += 1)
            {
                var lastI = i - 4;
                var bytes = input[lastI..i];

                Array.Reverse(bytes);
                
                var value = BitConverter.ToInt32(bytes);
                
                if (IsSupportedCharacterCode(value))
                {
                    return CharacterCodes[value];
                }
            }

            return EFourCc.Irtpc;
        }

        public static byte[] GetFirst16Bytes(string filepath)
        {
            using (var br = new BinaryReader(new FileStream(filepath, FileMode.Open)))
            {
                return br.ReadBytes(16);
            }
        }

        public static EFourCc GetCharacterCode(string filepath)
        {
            var bytes = GetFirst16Bytes(filepath);
            return ValidCharacterCode(bytes);
        }


        public static int GetVersion(string path, EFourCc fourCc)
        {
            return GetVersion(GetFirst16Bytes(path), fourCc);
        }
        
        public static int GetVersion(byte[] block, EFourCc fourCc)
        {
            return fourCc switch
            {
                EFourCc.Rtpc => GetRtpcVersion(block),
                EFourCc.Irtpc => GetIrtpcVersion(block),
                EFourCc.Aaf => GetAafVersion(block),
                EFourCc.Sarc => GetSarcVersion(block),
                EFourCc.Adf => throw new ArgumentOutOfRangeException(nameof(fourCc), fourCc, null),
                EFourCc.Tab => throw new ArgumentOutOfRangeException(nameof(fourCc), fourCc, null),
                EFourCc.Xml => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(fourCc), fourCc, null)
            };
        }

        #region Version Getters

        private static int GetRtpcVersion(byte[] block)
        {
            using var ms = new MemoryStream(block);
            ms.Seek(4, SeekOrigin.Begin);
            return ms.ReadByte();
        }

        private static int GetIrtpcVersion(byte[] block)
        {
            using var ms = new MemoryStream(block);
            return ms.ReadByte();
        }

        private static int GetAafVersion(byte[] block)
        {
            using var ms = new MemoryStream(block);
            ms.Seek(4, SeekOrigin.Begin);
            return ms.ReadByte();
        }
        
        private static int GetSarcVersion(byte[] block)
        {
            using var ms = new MemoryStream(block);
            return ms.ReadByte();
        }

        #endregion
    }
}