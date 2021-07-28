using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.IRTPC.V01.Refresh;
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


        public GenericAvaFileManager XmlLoad(string path)
        {
            // TODO: Fix this
            var xr = XmlReader.Create(new FileStream(path, FileMode.Open));
            xr.ReadStartElement("AvaFile");
            xr.ReadStartElement("History");

            return BuildRtpcFileManager();
        }
        
        private GenericAvaFileManager BuildRtpcFileManager()
        {
            return Version switch
            {
                1 => new RtpcV1Manager(),
                _ => throw new NotImplementedException($"Version not supported: '{Version}'")
            };
        }
        
        private GenericAvaFileManager BuildIrtpcFileManager()
        {
            return Version switch
            {
                1 => new IrtpcV1Manager(),
                _ => throw new NotImplementedException($"Version not supported: '{Version}'")
            };
        }

        public GenericAvaFileManager Build()
        {
            if (!string.IsNullOrEmpty(Path))
            {
                FourCc = FilePreProcessor.GetCharacterCode(Path);
                Version = FilePreProcessor.GetVersion(Path, FourCc);
            }
            
            // Factory for AvaFileManager
            switch (FourCc)
            {
                case EFourCc.Rtpc:
                    return BuildRtpcFileManager();
                case EFourCc.Irtpc:
                    return BuildIrtpcFileManager();
                case EFourCc.Aaf:
                case EFourCc.Sarc:
                case EFourCc.Adf:
                case EFourCc.Tab:
                case EFourCc.Xml:
                    return XmlLoad(Path);
                default:
                    throw new NotImplementedException($"EFourCc not supported: '{FourCc}'");
            }
        }
    }
}