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
        
        
        public AvaFileManagerFactory(string path)
        {
            Path = path;
        }


        private IAvaFileManager RtpcFileManager()
        {
            int version;
            using (var br = new BinaryReader(new FileStream(Path, FileMode.Open)))
            {
                version = br.ReadByte();
            }

            return version switch
            {
                1 => new AvaRtpcV1Manager(),
                _ => throw new NotImplementedException($"Version not supported: '{version}'")
            };
        }
        
        
        public IAvaFileManager GetAvaFileManager()
        {
            var fourCc = FourCCProcessor.GetCharacterCode(Path);
            
            // Factory for AvaFileManager
            switch (fourCc)
            {
                case EFourCc.Rtpc:
                    return RtpcFileManager();
                case EFourCc.Irtpc:
                case EFourCc.Aaf:
                case EFourCc.Sarc:
                case EFourCc.Adf:
                case EFourCc.Tab:
                case EFourCc.Xml:
                default:
                    throw new NotImplementedException($"EFourCc not supported: '{fourCc}'");
            }
        }
    }
}