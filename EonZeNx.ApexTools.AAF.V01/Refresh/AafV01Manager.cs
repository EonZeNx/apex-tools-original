using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.AAF.V01.Models;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.AAF.V01.Refresh
{
    /// <summary>
    /// An <see cref="AafV01Manager"/> file.
    /// <br/> Structure:
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Comment (Length 28, UTF8) - <see cref="string"/>
    /// <br/> Uncompressed Size - <see cref="uint"/>
    /// <br/> Compressed Size - <see cref="uint"/>
    /// <br/> Block count - <see cref="uint"/>
    /// <br/> Blocks[]
    /// </summary>
    public class AafV01Manager : GenericAvaFileManager
    {
        #region Variables

        public override EFourCc FourCc => EFourCc.Aaf;
        public override int Version => 1;
        public override string Extension { get; set; }
        public override string DefaultExtension { get; set; } = ".aaf";

        private string Comment { get; set; }
        private uint UncompressedSize { get; set; }
        private uint BlockSize { get; set; }
        private uint BlockCount { get; set; }
        private Block[] Blocks { get; set; }

        #endregion


        #region Helpers

        private void AafDeserialize(BinaryReader br)
        {
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            
            var block = br.ReadBytes(16);
            var fourCc = FilePreProcessor.ValidCharacterCode(block);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            
            if (fourCc != FourCc) throw new InvalidFileVersion();
            br.ReadUInt32();
            
            if (br.ReadUInt32() != Version) throw new InvalidFileVersion();
            
            Comment = Encoding.UTF8.GetString(br.ReadBytes(8 + 16 + 4));
            
            UncompressedSize = br.ReadUInt32();
            BlockSize = br.ReadUInt32();
            BlockCount = br.ReadUInt32();

            Blocks = new Block[BlockCount];
            for (var i = 0; i < BlockCount; i++)
            {
                Blocks[i] = new Block(BlockSize);
                Blocks[i].BinaryDeserialize(br);
            }
        }

        private void SarcDeserialize(BinaryReader br)
        {
            
        }

        #endregion
        
        #region Public Functions

        public override void Deserialize(string path)
        {
            Extension = Path.GetExtension(path);
            using var fs = new FileStream(path, FileMode.Open);
            Deserialize(BinaryReaderUtils.StreamToBytes(fs));
        }

        public override void Deserialize(byte[] contents)
        {
            // Pre-process file
            using var ms = new MemoryStream(contents);
            using var br = new BinaryReader(ms);
            
            var firstBlock = br.ReadBytes(16);
            ms.Seek(0, SeekOrigin.Begin);
            var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);

            if (fourCc == EFourCc.Aaf)
            {
                AafDeserialize(br);
            }
            else if (fourCc == EFourCc.Sarc)
            {
                SarcDeserialize(br);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override byte[] Export(HistoryInstance[] history = null)
        {
            throw new NotImplementedException();
        }

        public override byte[] ExportBinary()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            bw.Write(ByteUtils.ReverseBytes((uint) FourCc));
            bw.Write((uint) Version);
            bw.Write(Encoding.UTF8.GetBytes("AVALANCHEARCHIVEFORMATISCOOL"));

            var uncompressedSize = (uint) Blocks.Sum(block => block.UncompressedSize);
            bw.Write(uncompressedSize);
            
            bw.Write(
                Blocks.Length == 1
                    ? Blocks[0].BlockSize
                    : Math.Min(Blocks.Max(block => block.BlockSize), Block.MaxBlockSizeSize)
            );

            bw.Write(BlockCount);

            foreach (var block in Blocks)
            {
                block.BinarySerialize(bw);
            }

            return ms.ToArray();
        }

        public override byte[] ExportConverted(HistoryInstance[] history = null)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            for (var i = 0; i < BlockCount; i++)
            {
                Blocks[i].BinaryConvertedSerialize(bw);
            }
            
            return ms.ToArray();
        }

        public override void Export(string path, HistoryInstance[] history = null)
        {
            throw new NotImplementedException();
        }

        public override void ExportBinary(string path)
        {
            throw new NotImplementedException();
        }

        public override void ExportConverted(string path, HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}