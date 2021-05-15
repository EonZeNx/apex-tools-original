using System;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using EonZeNx.ApexTools.Core.External;
using EonZeNx.ApexTools.Core.Interfaces.Serializable;
using EonZeNx.ApexTools.Core.Utils;
using CompressionMode = Ionic.Zlib.CompressionMode;

namespace EonZeNx.ApexTools.AAF.V01.Models
{
    public class Block : IBinarySerializable
    {
        [DllImport("zlib.dll")]
        static extern int uncompress(IntPtr dest, ref uint destLen, IntPtr source, uint sourceLen);
        
        public static uint FOUR_CC => 0x4557414D;  // EWAM / MAWE
        public uint CompressedSize { get; set; }
        public uint UncompressedSize;
        public long DataOffset { get; set; }

        public byte[] Data;


        #region Binary Serialization

        public void BinarySerialize(BinaryWriter bw)
        {
            bw.Write(Data);
        }

        public void BinaryDeserialize(BinaryReader br)
        {
            CompressedSize = br.ReadUInt32();
            UncompressedSize = br.ReadUInt32();
            DataOffset = br.BaseStream.Position;

            var nextBlock = br.ReadUInt32();
            var fourCc = ByteUtils.ReverseBytes(br.ReadUInt32());
            
            if (fourCc != FOUR_CC)
            {
                throw new IOException($"Block four cc was not valid (Pos: {br.BaseStream.Position})");
            }
            
            Data = new byte[UncompressedSize];
            var compressedBlock = br.ReadBytes((int) CompressedSize);
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(0x78);
                ms.WriteByte(0x01);
                ms.Write(compressedBlock);
                
                
                using (var zs = new Ionic.Zlib.ZlibStream(ms, CompressionMode.Decompress))
                {
                    zs.Read(Data, 0, (int) UncompressedSize);
                }
            }

            // Data = new byte[UncompressedSize];
            // var destPtr = Marshal.AllocHGlobal(Data.Length);
            // Marshal.Copy(Data, 0, destPtr, Data.Length);
            //
            // var compressedBlock = br.ReadBytes((int) CompressedSize);
            // var srcPtr = Marshal.AllocHGlobal(compressedBlock.Length);
            // Marshal.Copy(compressedBlock, 0, srcPtr, compressedBlock.Length);
            //
            // var result = uncompress(destPtr, ref UncompressedSize, srcPtr, CompressedSize);
            //
            // Marshal.FreeHGlobal(destPtr);
            // Marshal.FreeHGlobal(srcPtr);

            // var compressedBlock = br.ReadBytes((int) CompressedSize);
            // Data = Oodle.Decompress(compressedBlock, UncompressedSize);
        }

        #endregion
    }
}