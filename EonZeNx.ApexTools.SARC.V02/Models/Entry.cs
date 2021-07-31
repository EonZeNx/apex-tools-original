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
    public class Entry : IBinarySerializable, IFolderSerializable, IDeferredSerializable
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
        
        public void XmlLoadReference(XmlReader xr)
        {
            DataOffset = 0;
            IsReference = bool.Parse(XmlUtils.GetAttribute(xr, "IsRef"));
            Size = uint.Parse(XmlUtils.GetAttribute(xr, "Size"));
            
            Path = xr.ReadString().Replace("\\", "/");
            PathLength = (uint) Path.Length;
        }

        #endregion
        
        #region Binary Serialization
        
        public void BinarySerializeData(BinaryWriter bw)
        {
            if (IsReference) return;
            
            DataOffset = (uint) bw.BaseStream.Position;
            bw.Write(Data);
            
            ByteUtils.Align(bw, 4, 0x00);
        }

        public void BinarySerialize(BinaryWriter bw)
        {
            var pathLengthWithNulls = ByteUtils.Align(PathLength, 4);
            var nulls = new string('\0', (int) (pathLengthWithNulls - PathLength));
            
            bw.Write(pathLengthWithNulls);
            bw.Write(Encoding.UTF8.GetBytes(Path));
            bw.Write(Encoding.UTF8.GetBytes(nulls));
            bw.Write(DataOffset);
            bw.Write(Size);
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            PathLength = br.ReadUInt32();
            
            Path = Encoding.UTF8.GetString(br.ReadBytes((int) PathLength));
            Path = Path.Replace("/", @"\");
            Path = Path.Replace("\0", "");
            
            DataOffset = br.ReadUInt32();
            IsReference = DataOffset == 0;
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