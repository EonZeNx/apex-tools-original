using System;
using System.IO;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.RTPC.V01.Refresh;

namespace EonZeNx.ApexTools.Refresh
{
    public class AvaFileManagerFactory
    {
        private string Path { get; }
        private EFourCc FourCc { get; set; }
        private int Version { get; set; }
        
        
        public AvaFileManagerFactory(string path)
        {
            Path = path;
        }
        
        public AvaFileManagerFactory(EFourCc fourCc, int version)
        {
            FourCc = fourCc;
            Version = version;
        }


        private IAvaFileManager BuildRtpcFileManager()
        {
            return Version switch
            {
                1 => new RtpcV1Manager(),
                _ => throw new NotImplementedException($"Version not supported: '{Version}'")
            };
        }

        public IAvaFileManager Build()
        {
            // Factory for AvaFileManager
            switch (FourCc)
            {
                case EFourCc.Rtpc:
                    return BuildRtpcFileManager();
                case EFourCc.Irtpc:
                case EFourCc.Aaf:
                case EFourCc.Sarc:
                case EFourCc.Adf:
                case EFourCc.Tab:
                case EFourCc.Xml:
                default:
                    throw new NotImplementedException($"EFourCc not supported: '{FourCc}'");
            }
        }
        
        public IAvaFileManager BuildFromPath()
        {
            FourCc = FilePreProcessor.GetCharacterCode(Path);
            Version = FilePreProcessor.GetVersion(Path, FourCc);

            return Build();
        }
    }
}