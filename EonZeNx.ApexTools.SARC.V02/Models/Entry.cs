using System.IO;
using System.Text;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;

namespace EonZeNx.ApexTools.SARC.V02.Models
{
    /// <summary>
    /// An entry for a <see cref="SARC_V02"/> <see cref="Entry"/>.
    /// <br/> Structure:
    /// <br/> Path Length - <see cref="uint"/>
    /// <br/> Path - <see cref="string"/>
    /// <br/> Data offset - <see cref="uint"/>
    /// <br/> Size - <see cref="uint"/>
    /// <br/> DATA - (Deferred)
    /// </summary>
    public class Entry : IBinarySerializable, IFolderSerializable
    {
        public uint PathLength { get; set; }
        public string Path { get; set; }
        public uint DataOffset { get; set; }
        public uint Size { get; set; }
        public bool IsReference => DataOffset == 0;

        public byte[] Data { get; set; }
        
        
        #region Binary Serialization

        public void BinarySerialize(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            PathLength = br.ReadUInt32();
            
            Path = Encoding.UTF8.GetString(br.ReadBytes((int) PathLength));
            Path = Path.Replace("/", @"\");
            Path = Path.Replace("\0", "");
            
            DataOffset = br.ReadUInt32();
            Size = br.ReadUInt32();

            if (IsReference) return;
            
            var originalPosition = br.BaseStream.Position;
            br.BaseStream.Seek(DataOffset, SeekOrigin.Begin);

            Data = br.ReadBytes((int) Size);
            br.BaseStream.Seek(originalPosition, SeekOrigin.Begin);
        }

        #endregion

        #region Converted Binary Serialization

        public void FolderSerialize(string basePath)
        {
            if (IsReference) return;
            
            var pathElements = Path.Split(@"\");
            var path = string.Join(@"\", pathElements[..^1]);

            Directory.CreateDirectory(@$"{basePath}\{path}");

            using (var bw = new BinaryWriter(new FileStream(@$"{basePath}\{Path}", FileMode.Create)))
            {
                bw.Write(Data);
            }
        }

        public void FolderDeserialize(string basePath)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}