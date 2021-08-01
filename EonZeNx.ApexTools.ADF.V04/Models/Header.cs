using System;
using System.Collections.Generic;
using System.IO;
using EonZeNx.ApexTools.Core.Refresh.Interfaces;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools.ADF.V04.Models
{
    public class Header : IAvaFileSimple
    {
        #region Variables

        public uint InstanceCount { get; set; }
        public uint InstanceOffset { get; set; }
        
        public uint TypeDefCount { get; set; }
        public uint TypeDefOffset { get; set; }
        
        public uint StringHashCount { get; set; }
        public uint StringHashOffset { get; set; }
        
        public uint NameTableCount { get; set; }
        public uint NameTableOffset { get; set; }
        
        public uint TotalSize { get; set; }
        
        public uint[] Unknowns { get; set; }
        
        public string Comment { get; set; }

        #endregion


        #region Functions

        public void Deserialize(byte[] contents)
        {
            using var ms = new MemoryStream(contents);
            using var br = new BinaryReader(ms);

            ms.Seek(4 + 4, SeekOrigin.Begin);

            InstanceCount = br.ReadUInt32();
            InstanceOffset = br.ReadUInt32();
            
            TypeDefCount = br.ReadUInt32();
            TypeDefOffset = br.ReadUInt32();
            
            StringHashCount = br.ReadUInt32();
            StringHashOffset = br.ReadUInt32();
            
            NameTableCount = br.ReadUInt32();
            NameTableOffset = br.ReadUInt32();
            
            TotalSize = br.ReadUInt32();

            var unknowns = new List<uint>();
            for (var i = 0; i < 5; i++) { unknowns.Add(br.ReadUInt32()); }
            
            Unknowns = unknowns.ToArray();

            Comment = br.ReadStringZ();
        }

        public byte[] Export()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            bw.Write(InstanceCount);
            bw.Write(InstanceOffset);
            
            bw.Write(TypeDefCount);
            bw.Write(TypeDefOffset);
            
            bw.Write(StringHashCount);
            bw.Write(StringHashOffset);
            
            bw.Write(NameTableCount);
            bw.Write(NameTableOffset);
            
            bw.Write(TotalSize);
            foreach (var unk in Unknowns) { bw.Write(unk); }
            bw.WriteStringZ(Comment);

            return ms.ToArray();
        }

        #endregion
    }
}