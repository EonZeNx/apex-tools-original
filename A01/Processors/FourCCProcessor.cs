using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using A01.Utils;

namespace A01
{
    public enum EFoucCC
    {
        RTPC,
        IRTPC,
        ADF,
        SARC,
        AAF,
        TAB
    }
    
    public class FourCCProcessor
    {
        public static readonly Dictionary<byte[], EFoucCC> FOURC_CCS = new(new ByteArrayEqualityComparer())
        {
            {new byte[] {0x52, 0x54, 0x50, 0x43}, EFoucCC.RTPC},
            {new byte[] {0x20, 0x46, 0x44, 0x41}, EFoucCC.ADF},
            {new byte[] {0x53, 0x41, 0x52, 0x43}, EFoucCC.SARC},
            {new byte[] {0x41, 0x41, 0x46, 0x00}, EFoucCC.AAF},
            {new byte[] {0x54, 0x41, 0x42, 0x00}, EFoucCC.TAB},
        };
        
        public bool IsSupportedFourCC(byte[] input)
        {
            var result = FOURC_CCS.Any(pair => pair.Key.SequenceEqual(input));
            return result;
        }

        public EFoucCC FourCCInByteArray(byte[] input)
        {
            for (var i = 4; i <= input.Length; i += 4)
            {
                var lastI = i - 4;
                var bytes = input[lastI..i];
                if (IsSupportedFourCC(bytes))
                {
                    return FOURC_CCS[bytes];
                }
            }

            return EFoucCC.IRTPC;
        }

        public byte[] GetFirst16Bytes(string filepath)
        {
            using (var br = new BinaryReader(new FileStream(filepath, FileMode.Open)))
            {
                return br.ReadBytes(16);
            }
        }

        public EFoucCC GetFourCC(string filepath)
        {
            
            var bytes = GetFirst16Bytes(filepath);
            return FourCCInByteArray(bytes);
        }
    }
}