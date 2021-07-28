using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.Refresh
{
    public class XmlManager : GenericAvaFileBare
    {
        #region Variables

        public override EFourCc FourCc { get; } = EFourCc.Xml;
        public override int Version { get; } = 0;
        public override string Extension { get; set; }
        
        public byte[] CurrentContents;
        public HistoryInstance[] History;

        #endregion


        #region Helpers

        public HistoryInstance ReadHistoryFromXmlNode(XmlReader xr)
        {
            var versionStr = XmlUtils.GetAttribute(xr, "Version");
            var tryVersionParse = int.TryParse(versionStr, out var version);
            if (!tryVersionParse) throw new ArgumentOutOfRangeException(versionStr);

            var avaFileTypeStr = xr.ReadString();
            var tryParse = FilePreProcessor.StrToFourCc.ContainsKey(avaFileTypeStr);
            if (!tryParse) throw new ArgumentOutOfRangeException(avaFileTypeStr);

            return new HistoryInstance(FilePreProcessor.StrToFourCc[avaFileTypeStr], version);
        }
        
        public HistoryInstance[] GetHistoryFromXml(Stream contents)
        {
            var history = new List<HistoryInstance>();
            
            contents.Seek(0, SeekOrigin.Begin);
            var xr = XmlReader.Create(contents);
            xr.ReadToDescendant("History");

            Extension = XmlUtils.GetAttribute(xr, "Extension");

            while (xr.Read())
            {
                var nodeType = xr.NodeType;
                if (nodeType == XmlNodeType.EndElement && xr.Name == "History") break;
                if (nodeType != XmlNodeType.Element) continue;
                
                history.Add(ReadHistoryFromXmlNode(xr));
            }

            var historyArray = history.ToArray();
            Array.Reverse(historyArray);

            return history.ToArray();
        }

        #endregion
        
        
        #region Abstract Functions

        public override void Deserialize(string path)
        {
            using var fs = new FileStream(path, FileMode.Open);
            Deserialize(BinaryReaderUtils.StreamToBytes(fs));
        }

        public override void Deserialize(byte[] contents)
        {
            using var ms = new MemoryStream(contents);
            ms.Seek(0, SeekOrigin.Begin);
            History = GetHistoryFromXml(ms);
            
            CurrentContents = ms.ToArray();
            foreach (var hInstance in History)
            {
                var manager = new AvaFileManagerFactory(hInstance.FourCc, hInstance.Version).Build();
                manager.Deserialize(CurrentContents);
                CurrentContents = manager.ExportBinary();
            }
        }

        public override byte[] Export()
        {
            return CurrentContents;
        }
        
        #endregion
    }
}