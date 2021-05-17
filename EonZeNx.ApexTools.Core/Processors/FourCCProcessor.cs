using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.Core.Processors
{
    public enum EFourCc
    {
        RTPC,
        IRTPC,
        AAF,
        SARC,
        ADF,
        TAB,
        XML
    }
    
    public static class FourCCProcessor
    {
        public static readonly Dictionary<int, EFourCc> CharacterCodes = new()
        {
            {0x52545043, EFourCc.RTPC},
            {0x20464441, EFourCc.ADF},
            {0x53415243, EFourCc.SARC},
            {0x41414600, EFourCc.AAF},
            {0x54414200, EFourCc.TAB},
            {0x3F786D6C, EFourCc.XML},
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

            return EFourCc.IRTPC;
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
    }
}