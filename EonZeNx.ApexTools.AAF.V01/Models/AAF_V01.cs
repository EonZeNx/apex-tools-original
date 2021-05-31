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
    public class AAF_V01 : IStreamClassIo
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

        public void StreamSerialize(Stream s)
        {
            s.Write(ByteUtils.ReverseBytes(0x41414600));  // FourCC 'AAF '
            s.Write((uint) 1);
            s.Write(Encoding.UTF8.GetBytes("AVALANCHEARCHIVEFORMATISCOOL"));

            var uncompressedSize = (uint) Blocks.Sum(block => block.UncompressedSize);
            s.Write(uncompressedSize);
            
            s.Write(
                Blocks.Length == 1
                    ? Blocks[0].BlockSize
                    : Math.Min(Blocks.Max(block => block.BlockSize), Block.MaxBlockSizeSize)
            );

            s.Write(BlockCount);

            foreach (var block in Blocks)
            {
                block.StreamSerialize(s);
            }
        }

        public void StreamDeserialize(Stream s)
        {
            var fourCc = s.ReadInt32();
            Version = s.ReadInt32();
            Comment = Encoding.UTF8.GetString(s.ReadBytes(8 + 16 + 4));
            
            UncompressedSize = s.ReadUInt32();
            BlockSize = s.ReadUInt32();
            BlockCount = s.ReadUInt32();

            Blocks = new Block[BlockCount];
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks[i] = new Block(BlockSize);
                Blocks[i].StreamDeserialize(s);
            }
        }

        #endregion

        #region Converted Binary Serialization

        public void StreamConvertedSerialize(Stream s)
        {
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks[i].StreamConvertedSerialize(s);
            }
        }

        public void StreamConvertedDeserialize(Stream s)
        {
            var blockList = new List<Block>();
            while (s.Position < s.Length)
            {
                var block = new Block();
                block.StreamConvertedDeserialize(s);
                blockList.Add(block);
            }

            Blocks = blockList.ToArray();
            BlockCount = (uint) Blocks.Length;
        }

        #endregion
    }
}