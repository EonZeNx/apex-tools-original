using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.Refresh
{
    public class XmlManager : GenericAvaFileManager
    {
        #region Variables

        public override EFourCc FourCc { get; } = EFourCc.Xml;
        public override int Version { get; } = 0;
        
        public Stream CurrentContents;
        public HistoryInstance[] History;

        #endregion
        
        
        #region Abstract Functions

        public override void Deserialize(string path)
        {
            Deserialize(new FileStream(path, FileMode.Open));
        }

        public override void Deserialize(Stream contents)
        {
            contents.Seek(0, SeekOrigin.Begin);
            var history = new List<HistoryInstance>();
            
            var xr = XmlReader.Create(contents);
            xr.ReadStartElement("AvaFile");
            xr.ReadStartElement("History");

            while (xr.Read())
            {
                var nodeType = xr.NodeType;
                if (nodeType == XmlNodeType.EndElement && xr.Name == "History") break;
                if (nodeType != XmlNodeType.Element) continue;
                
                var versionStr = XmlUtils.GetAttribute(xr, "Version");
                var tryVersionParse = int.TryParse(versionStr, out var version);
                if (!tryVersionParse) throw new ArgumentOutOfRangeException(versionStr);

                var avaFileTypeStr = xr.ReadString();
                var tryParse = FilePreProcessor.StrToFourCc.ContainsKey(avaFileTypeStr);
                if (!tryParse) throw new ArgumentOutOfRangeException(avaFileTypeStr);
                
                history.Add(new HistoryInstance(FilePreProcessor.StrToFourCc[avaFileTypeStr], version));
            }

            var historyArray = history.ToArray();
            Array.Reverse(historyArray);

            CurrentContents = contents;
            CurrentContents.Seek(0, SeekOrigin.Begin);
            foreach (var hInstance in historyArray)
            {
                var manager = new AvaFileManagerFactory(hInstance.FourCc, hInstance.Version).Build();
                manager.Deserialize(CurrentContents);
                CurrentContents = manager.Export();
                CurrentContents.Seek(0, SeekOrigin.Begin);
            }
        }

        #region Unused
        // TODO: Check and make XML loader its own class

        public override void Export(string path, HistoryInstance[] history = null)
        {
            throw new NotImplementedException();
        }
        
        public override void ExportBinary(string path)
        {
            throw new NotImplementedException();
        }
        
        public override void ExportConverted(string path, HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override Stream Export(HistoryInstance[] history = null)
        {
            return CurrentContents;
        }

        public override Stream ExportBinary()
        {
            throw new NotImplementedException();
        }

        public override Stream ExportConverted(HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}