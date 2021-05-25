using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.AAF.V01.Models
{
    /// <summary>
    /// An <see cref="AAF_V01"/> file.
    /// <br/> Structure:
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Comment (Length 28, UTF8) - <see cref="string"/>
    /// <br/> Uncompressed Size - <see cref="uint"/>
    /// <br/> Compressed Size - <see cref="uint"/>
    /// <br/> Block count - <see cref="uint"/>
    /// <br/> Blocks[]
    /// </summary>
    public class AAF_V01 : IBinaryClassIO
    {
        public MetaInfo Minfo { get; set; } = new (){FileType = "AAF", Version = 01, Extension = ".ee"};
        public int Version { get; set; }
        public string Comment { get; set; }
        
        public uint UncompressedSize { get; set; }
        public uint BlockSize { get; set; }
        public uint BlockCount { get; set; }
        
        public Block[] Blocks { get; set; }
        
        
        public MetaInfo GetMetaInfo()
        {
            return Minfo;
        }
        
        #region Binary Serialization

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(ByteUtils.ReverseBytes(0x41414600));  // FourCC 'AAF '
            bw.Write((uint) 1);
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
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            var fourCc = br.ReadInt32();
            Version = br.ReadInt32();
            Comment = Encoding.UTF8.GetString(br.ReadBytes(8 + 16 + 4));
            
            UncompressedSize = br.ReadUInt32();
            BlockSize = br.ReadUInt32();
            BlockCount = br.ReadUInt32();

            Blocks = new Block[BlockCount];
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks[i] = new Block(BlockSize);
                Blocks[i].BinaryDeserialize(br);
            }
        }

        #endregion

        #region Converted Binary Serialization

        public void BinaryConvertedSerialize(BinaryWriter bw)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks[i].BinaryConvertedSerialize(bw);
            }
        }

        public void BinaryConvertedDeserialize(BinaryReader br)
        {
            var blockList = new List<Block>();
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var block = new Block();
                block.BinaryConvertedDeserialize(br);
                blockList.Add(block);
            }

            Blocks = blockList.ToArray();
            BlockCount = (uint) Blocks.Length;
        }

        #endregion
    }
}