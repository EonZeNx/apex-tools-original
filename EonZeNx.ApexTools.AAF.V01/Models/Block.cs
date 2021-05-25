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
    /// <br/> COMPRESSED DATA : ZLib uncompress Level 6
    /// </summary>
    public class Block : IBinarySerializable, IBinaryConvertedSerializable
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

        public void BinarySerialize(BinaryWriter bw)
        {
            var blockStartPos = (uint) bw.BaseStream.Position;
            bw.Write(CompressedSize - 2);
            bw.Write(UncompressedSize);
            
            bw.Write(4 + 4 + 4 + 4 + CompressedSize - 2);
            bw.Write(ByteUtils.ReverseBytes(FourCc));
            bw.Write(CompressedData[2..]);
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            DataOffset = br.BaseStream.Position;
            CompressedSize = br.ReadUInt32();
            UncompressedSize = br.ReadUInt32();

            var nextBlock = br.ReadUInt32() + DataOffset;
            var fourCc = ByteUtils.ReverseBytes(br.ReadUInt32());
            
            if (fourCc != FourCc)
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
            Data = br.ReadBytes((int) BlockSize);
            UncompressedSize = (uint) Data.Length;
            BlockSize = UncompressedSize;
            CompressedSize = (uint) CompressedData.Length;
        }

        #endregion
    }
}