using System;
using System.IO;
using System.Linq;

namespace A01
{
    public class FourCCProcessor
    {
        public static readonly byte[][] SUPPORTED_FOUR_CC = new[]
        {
            // RTPC
            new byte[] {0x52, 0x54, 0x50, 0x43},
            // AAF
            new byte[] {0x41, 0x41, 0x46, 0x00},
            // SARC
            new byte[] {0x53, 0x41, 0x52, 0x43},
            // TAB
            new byte[] {0x54, 0x41, 0x42, 0x00},
            // ADF/ FDA
            new byte[] {0x20, 0x46, 0x44, 0x41}
        };
        
        public bool IsSupportedFourCC(byte[] input)
        {
            var result = SUPPORTED_FOUR_CC.Any(fourCC => fourCC.SequenceEqual(input));
            return result;
        }

        public byte[] FourCCInByteArray(byte[] input)
        {
            for (var i = 4; i <= input.Length; i += 4)
            {
                var lastI = i - 4;
                var bytes = input[lastI..i];
                if (IsSupportedFourCC(bytes))
                {
                    return input[lastI..i];
                }
            }
            throw new ArgumentException("Did not find supported FourCC");
        }

        public byte[] GetFirst16Bytes(string filepath)
        {
            using (var binaryStream = new BinaryReader(new FileStream(filepath, FileMode.Open)))
            {
                return binaryStream.ReadBytes(16);
            }
        }
    }
}