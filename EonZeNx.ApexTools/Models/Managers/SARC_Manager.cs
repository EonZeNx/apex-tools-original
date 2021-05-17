using System;
using System.IO;
using System.Linq;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.SARC.V02.Models;

namespace EonZeNx.ApexTools.Models.Managers
{
    public class SARC_Manager : FileProcessor
    {
        public override string FullPath { get; protected set; }
        public override string ParentPath { get; protected set; }
        public override string PathName { get; protected set; }
        public override string Extension { get; protected set; }
        public override string[] ConvertedExtensions { get; protected set; }

        public override string FileType { get; protected set; }
        public override int Version { get; protected set; }
        
        private IFolderClassIO sarc { get; set; }
        
        public override void GetClassIO(string path)
        {
            FullPath = path;
            (ParentPath, PathName, Extension) = PathUtils.SplitPath(path);

            int version;
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                version = br.ReadByte();
            }

            sarc = version switch
            {
                2 => new SARC_V02(),
                _ => new SARC_V02()
            };
        }

        public override bool FileIsBinary()
        {
            return !string.IsNullOrEmpty(Extension);
        }

        public override void LoadBinary()
        {
            sarc.GetMetaInfo().Extension = Extension;
            using (var br = new BinaryReader(new FileStream(FullPath, FileMode.Open)))
            {
                sarc.BinaryDeserialize(br);
            }
        }

        public override void ExportConverted()
        {
            var subPath = $@"{ParentPath}\{PathName}";
            sarc.FolderSerialize(subPath);
        }

        public override void LoadConverted()
        {
            sarc.FolderDeserialize(ParentPath);
        }

        public override void ExportBinary()
        {
            throw new NotImplementedException();
        }
    }
}