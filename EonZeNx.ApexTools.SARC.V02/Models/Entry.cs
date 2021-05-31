using System.IO;
using System.Text;
using System.Xml;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.SARC.V02.Models
{
    /// <summary>
    /// An entry for a <see cref="SARC_V02"/> <see cref="Entry"/>.
    /// <br/> Structure:
    /// <br/> Path Length - <see cref="uint"/>
    /// <br/> Path - <see cref="string"/>
    /// <br/> Data offset - <see cref="uint"/>
    /// <br/> Size - <see cref="uint"/>
    /// <br/> Data - (Deferred)
    /// </summary>
    public class Entry : IStreamSerializable, IFolderSerializable, IDeferredSerializable
    {
        public uint PathLength { get; set; }
        public string Path { get; set; }
        public uint DataOffset { get; set; }
        public uint Size { get; set; }
        public bool IsReference { get; set; }
        public uint HeaderSize
        {
            get
            {
                var pathLengthWithNulls = ByteUtils.Align(PathLength, 4);
                return 4 + pathLengthWithNulls + 4 + 4;
            }
        }

        public byte[] Data { get; set; }


        public Entry() { }
        public Entry(string path)
        {
            Path = path;
            PathLength = (uint) Path.Length;
        }

        #region Xml Load Helpers

        public void XmlLoadExternalReference(XmlReader xr)
        {
            DataOffset = 0;
            IsReference = true;
            Size = uint.Parse(XmlUtils.GetAttribute(xr, "Size"));
            
            Path = xr.ReadString().Replace("\\", "/");
            PathLength = (uint) Path.Length;
        }

        #endregion
        
        #region Binary Serialization
        
        public void StreamSerializeData(Stream s)
        {
            if (IsReference) return;
            
            DataOffset = (uint) s.Position;
            s.Write(Data);
            
            ByteUtils.Align(s, 4, fill: 0x00);
        }

        public void StreamSerialize(Stream s)
        {
            var pathLengthWithNulls = ByteUtils.Align(PathLength, 4);
            var nulls = new string('\0', (int) (pathLengthWithNulls - PathLength));
            
            s.Write(pathLengthWithNulls);
            s.Write(Encoding.UTF8.GetBytes(Path));
            s.Write(Encoding.UTF8.GetBytes(nulls));
            s.Write(DataOffset);
            s.Write(Size);
        }

        public void StreamDeserialize(Stream s)
        {
            PathLength = s.ReadUInt32();
            
            Path = Encoding.UTF8.GetString(s.ReadBytes((int) PathLength));
            Path = Path.Replace("/", @"\");
            Path = Path.Replace("\0", "");
            
            DataOffset = s.ReadUInt32();
            IsReference = DataOffset == 0;
            Size = s.ReadUInt32();

            if (IsReference) return;
            
            var originalPosition = s.Position;
            s.Seek(DataOffset, SeekOrigin.Begin);

            Data = s.ReadBytes((int) Size);
            s.Seek(originalPosition, SeekOrigin.Begin);
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
            if (IsReference) return;
            
            using (var br = new BinaryReader(new FileStream(basePath, FileMode.Open)))
            {
                Data = br.ReadBytes((int) br.BaseStream.Length);
            }
            
            Size = (uint) Data.Length;
        }

        #endregion
    }
}