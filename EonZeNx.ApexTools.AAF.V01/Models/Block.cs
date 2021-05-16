using System.IO;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using Ionic.Zlib;

namespace EonZeNx.ApexTools.AAF.V01.Models
{
    /*
     * Block
     * Compressed Size : uint32
     * Uncompressed Size : uint32
     * Next block offset : uint32 (From start of block)
     * FourCC
     * COMPRESSED DATA : ZLib uncompress Level 6
     */
    public class Block : IBinarySerializable, IBinaryConvertedSerializable
    {
        // EWAM / MAWE
        public const uint FOUR_CC = 0x4557414D;
        // 33,554,432 is 32MB
        public const int MAX_BLOCK_SIZE = 33554432;
            
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

                using var msData = new MemoryStream(Data);
                using var ms = new MemoryStream();
                using (var zs = new ZlibStream(msData, CompressionMode.Compress, CompressionLevel.Level6))
                {
                    zs.CopyTo(ms);
                }
                            
                _compressedData = ms.ToArray();

                return _compressedData;
            }
        }


        #region Binary Serialization

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(CompressedSize - 2);
            bw.Write(UncompressedSize);
            
            bw.Write(4 + CompressedSize - 2);
            bw.Write(ByteUtils.ReverseBytes(FOUR_CC));
            bw.Write(CompressedData[2..]);
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            DataOffset = br.BaseStream.Position;
            CompressedSize = br.ReadUInt32();
            UncompressedSize = br.ReadUInt32();

            var nextBlock = br.ReadUInt32() + DataOffset;
            var fourCc = ByteUtils.ReverseBytes(br.ReadUInt32());
            
            if (fourCc != FOUR_CC)
            {
                throw new IOException($"Block four cc was not valid (Pos: {br.BaseStream.Position})");
            }
            
            var compressedBlock = br.ReadBytes((int) CompressedSize);
            
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

            br.BaseStream.Seek(nextBlock, SeekOrigin.Begin);
        }

        #endregion

        #region Converted Binary Serialiation

        public void BinaryConvertedSerialize(BinaryWriter bw)
        {
            bw.Write(Data);
        }

        public void BinaryConvertedDeserialize(BinaryReader br)
        {
            Data = br.ReadBytes(MAX_BLOCK_SIZE);
            UncompressedSize = (uint) Data.Length;
            CompressedSize = (uint) CompressedData.Length;
        }

        #endregion
    }
}