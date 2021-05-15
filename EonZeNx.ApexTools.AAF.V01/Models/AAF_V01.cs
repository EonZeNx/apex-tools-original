using System.IO;
using System.Text;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Models;

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
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks[i].BinarySerialize(bw);
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
            throw new System.NotImplementedException();
        }

        public void BinaryConvertedDeserialize(BinaryReader br)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}