using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Xml;
using EonZeNx.ApexTools.AAF.V01.Models;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.RTPC.V01.Models;

namespace EonZeNx.ApexTools.Models.Managers
{
    public class AAF_Manager : FileProcessor
    {
        public override string FullPath { get; protected set; }
        public override string ParentPath { get; protected set; }
        public override string PathName { get; protected set; }
        public override string Extension { get; protected set; }
        public override string[] ConvertedExtensions { get; protected set; } = {".sarc", ".yaml"};

        public override string FileType { get; protected set; }
        public override int Version { get; protected set; }
        
        private IBinaryClassIO aaf { get; set; }
        
        public override void GetClassIO(string path)
        {
            FullPath = path;
            (ParentPath, PathName, Extension) = PathUtils.SplitPath(path);

            int version;
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                version = br.ReadByte();
            }

            aaf = version switch
            {
                1 => new AAF_V01(),
                _ => new AAF_V01()
            };
        }

        public override bool FileIsBinary()
        {
            return !ConvertedExtensions.Contains(Extension);
        }

        public override void LoadBinary()
        {
            aaf.GetMetaInfo().Extension = Extension;
            using (var br = new BinaryReader(new FileStream(FullPath, FileMode.Open)))
            {
                aaf.BinaryDeserialize(br);
            }
        }

        public override void ExportConverted()
        {
            using (var bw = new BinaryWriter(new FileStream(@$"{ParentPath}\{PathName}.sarc", FileMode.Create)))
            {
                aaf.BinarySerialize(bw);
            }
        }

        public override void LoadConverted()
        {
            throw new NotImplementedException("Have yet to implement LoadConverted on AAF_Manager");
        }

        public override void ExportBinary()
        {
            throw new NotImplementedException("Have yet to implement ExportBinary on AAF_Manager");
            using (var bw = new BinaryWriter(new FileStream(@$"{ParentPath}\{PathName}{aaf.GetMetaInfo().Extension}", FileMode.Create)))
            {
                aaf.BinarySerialize(bw);
            }
        }
    }
}