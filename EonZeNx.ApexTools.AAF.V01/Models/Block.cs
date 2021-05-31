using System.IO;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using Ionic.Zlib;

namespace EonZeNx.ApexTools.AAF.V01.Models
{
    /// <summary>
    /// A <see cref="Block"/> for a <see cref="AAF_V01"/>
    /// <br/> Structure:
    /// <br/> Compressed Size - <see cref="uint"/>
    /// <br/> Uncompressed Size - <see cref="uint"/>
    /// <br/> Next block offset : uint32 (From start of block) - <see cref="uint"/>
    /// <br/> FourCC
    /// <br/> Compressed Data : ZLib uncompress Level 6
    /// </summary>
    public class Block : IStreamSerializable, IStreamConvertedSerializable
    {
        // EWAM / MAWE
        public const uint FourCc = 0x4557414D;
        // 33,554,432 is 32MB
        public const uint MaxBlockSizeSize = 33554432;

        public uint BlockSize { get; set; } = MaxBlockSizeSize;
        public uint CompressedSize { get; set; }
        public uint UncompressedSize { get; set; }
        public long DataOffset { get; set; }

        public byte[] Data { get; set; }
        private byte[] _compressedData;
        public byte[] CompressedData
        {
            get
            {
                if (_compressedData != null) return _compressedData;

                using var ms = new MemoryStream();
                using (var zs = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level6))
                {
                    zs.Write(Data, 0, Data.Length);
                }
                            
                _compressedData = ms.ToArray();
                return _compressedData;
            }
        }
        
        
        public Block() { }
        public Block(uint blockSize)
        {
            BlockSize = blockSize;
        }


        #region Binary Serialization

        public void StreamSerialize(Stream s)
        {
            var blockStartPos = (uint) s.Position;
            s.Write(CompressedSize - 2);
            s.Write(UncompressedSize);
            
            s.Write(4 + 4 + 4 + 4 + CompressedSize - 2);
            s.Write(ByteUtils.ReverseBytes(FourCc));
            s.Write(CompressedData[2..]);
        }

        public void StreamDeserialize(Stream s)
        {
            DataOffset = s.Position;
            CompressedSize = s.ReadUInt32();
            UncompressedSize = s.ReadUInt32();

            var nextBlock = s.ReadUInt32() + DataOffset;
            var fourCc = ByteUtils.ReverseBytes(s.ReadUInt32());
            
            if (fourCc != FourCc)
            {
                throw new IOException($"Block four cc was not valid (Pos: {s.Position})");
            }
            
            var compressedBlock = s.ReadBytes((int) CompressedSize);
            
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(0x78);
                ms.WriteByte(0x01);
                ms.Write(compressedBlock);

                compressedBlock = ms.ToArray();
            }
            
            Data = ZlibStream.UncompressBuffer(compressedBlock);

            if (Data.Length != UncompressedSize)
            {
                throw new IOException(
                    "Error in decompression, uncompressed size does not equal uncompressed data length");
            }

            s.Seek(nextBlock, SeekOrigin.Begin);
        }

        #endregion

        #region Converted Binary Serialiation

        public void StreamConvertedSerialize(Stream s)
        {
            s.Write(Data);
        }

        public void StreamConvertedDeserialize(Stream s)
        {
            Data = s.ReadBytes((int) BlockSize);
            UncompressedSize = (uint) Data.Length;
            BlockSize = UncompressedSize;
            CompressedSize = (uint) CompressedData.Length;
        }

        #endregion
    }
}