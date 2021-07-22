using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Processors;
using EonZeNx.ApexTools.Core.Refresh;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.RTPC.V01.Models;

namespace EonZeNx.ApexTools.RTPC.V01.Refresh
{
    /// <summary>
    /// An <see cref="RTPC_V01"/> file
    /// <br/> Structure:
    /// <br/> FourCc - <see cref="T:EFourCC"/>
    /// <br/> Version - <see cref="uint"/>
    /// <br/> Root container - <see cref="Container"/>
    /// </summary>
    public class RtpcV1Manager : GenericAvaFileManager, IDatabaseConnection
    {
        #region Variables

        public override EFourCc FourCc { get; } = EFourCc.Rtpc;
        public override int Version { get; } = 1;
        public SQLiteConnection DbConnection { get; set; }
        private Container Root { get; set; }

        #endregion

        
        #region Public Functions

        public override void Deserialize(string path)
        {
            Deserialize(new FileStream(path, FileMode.Open));
        }

        public override void Deserialize(Stream contents)
        {
            // Pre-process file
            contents.Seek(0, SeekOrigin.Begin);
            using var br = new BinaryReader(contents);
            
            var firstBlock = br.ReadBytes(16);
            var fourCc = FilePreProcessor.ValidCharacterCode(firstBlock);
            Root = new Container(DbConnection);

            if (fourCc == EFourCc.Rtpc)
            {
                br.BaseStream.Seek(4 + 4, SeekOrigin.Begin);
                Root.BinaryDeserialize(br);
            }
            else if (fourCc == EFourCc.Xml)
            {
                contents.Seek(0, SeekOrigin.Begin);
                Root.XmlDeserialize(XmlReader.Create(contents));
            }
            else
                throw new NotSupportedException();
        }

        public override Stream Export(HistoryInstance[] history = null)
        {
            throw new NotImplementedException();
        }

        public override Stream ExportBinary()
        {
            throw new NotImplementedException();
        }

        public override Stream ExportConverted(HistoryInstance[] history)
        {
            throw new NotImplementedException();
        }

        public override void Export(string path, HistoryInstance[] history = null)
        {
            var settings = new XmlWriterSettings{ Indent = true, IndentChars = "\t" };
            using var xw = XmlWriter.Create(path, settings);
            // TODO: Write history here
            Root.XmlSerialize(xw);
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
    }
}