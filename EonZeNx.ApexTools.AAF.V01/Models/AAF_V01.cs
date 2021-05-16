using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.AAF.V01.Models
{
    public class AAF_V01 : IBinaryClassIO
    {
        public MetaInfo Minfo { get; set; } = new (){FileType = "AAF", Version = 01};
        public int Version { get; set; }
        public string Comment { get; set; }
        
        public uint UncompressedSize { get; set; }
        public uint CompressedSize { get; set; }
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
            
            bw.Write((uint) Blocks.Sum(block => block.UncompressedSize));
            bw.Write((uint) Blocks.Sum(block => block.CompressedSize));
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
            CompressedSize = br.ReadUInt32();
            BlockCount = br.ReadUInt32();

            Blocks = new Block[BlockCount];
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks[i] = new Block();
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