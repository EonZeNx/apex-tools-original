using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EonZeNx.ApexTools.Core.Utils;

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
        public static readonly Dictionary<int, EFourCc> CharacterCodes = new()
        {
            {0x52545043, EFourCc.Rtpc},
            {0x20464441, EFourCc.Adf},
            {0x53415243, EFourCc.Sarc},
            {0x41414600, EFourCc.Aaf},
            {0x54414200, EFourCc.Tab},
            {0x3F786D6C, EFourCc.Xml},
        };
        
        public static bool IsSupportedCharacterCode(int input)
        {
            var result = CharacterCodes.Any(pair => pair.Key == input);
            return result;
        }

        public static EFourCc ValidCharacterCode(byte[] input)
        {
            for (var i = 4; i <= input.Length; i += 4)
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
            // TODO: Get version from file
            return 0;
        }
        
        public static int GetVersion(byte[] block, EFourCc fourCc)
        {
            // TODO: Get version from block
            return 0;
        }
    }
}