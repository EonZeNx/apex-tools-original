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


        public SARC_Manager() { }
        public SARC_Manager(int version)
        {
            Version = version;
        }
        
        public override void GetClassIO(string path)
        {
            // Directory = Converted load
            if (Directory.Exists(path))
            {
                if (Version == 0) throw new IOException("Path was a directory but manager was not init using version");

                ParentPath = path;
                sarc = Version switch
                {
                    2 => new SARC_V02(),
                    _ => new SARC_V02()
                };
                
                return;
            }
            // File = Binary load
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
                sarc.StreamDeserialize(br);
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
            using (var bw = new BinaryWriter(new FileStream($"{ParentPath}{sarc.GetMetaInfo().Extension}", FileMode.Create)))
            {
                sarc.StreamSerialize(bw);
            }
        }
    }
}