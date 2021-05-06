using System;
using System.IO;
using System.Xml;
using EonZeNx.ApexTools.Core.Utils;
using EonZeNx.ApexTools.Models.Managers;
using EonZeNx.ApexTools.RTPC.V01.Models;
using NUnit.Framework;

namespace EonZeNx.ApexTools.Test
{
    [TestFixture]
    public class RTPC_Tests
    {
        public string basePath = @"E:\Projects\Just Cause Tools\Apex Engine Tools CSharp\EonZeNx.ApexTools.Test\RTPC";
        
        [Test]
        public void PropertyData_Jc3GlobalXml_ByteArrayShouldEqualRTPC_Test()
        {
            // Load XML data
            var xmlPath = @$"{basePath}\PropertyData - CCC46900.xml";
            var xr = XmlReader.Create(xmlPath);
            xr.MoveToContent();

            var container = new Container();
            container.XmlDeserialize(xr);

            // Process property data
            long coffset = 1352;
            var convertedBlock = container.GetPropertyData(ref coffset);
            
            // Load correct data
            var binaryPath = @$"{basePath}\PropertyData - CCC46900.rtpc";
            byte[] binaryBlock;
            using (var br = new BinaryReader(new FileStream(binaryPath, FileMode.Open)))
            {
                binaryBlock = br.ReadBytes((int) br.BaseStream.Length);
            }
            
            // Write converted block
            var convertedPath = @$"{basePath}\PropertyData - CCC46900 CONV.rtpc";
            using (var bw = new BinaryWriter(new FileStream(convertedPath, FileMode.Create)))
            {
                bw.Write(convertedBlock);
            }
            
            Assert.AreEqual(binaryBlock, convertedBlock);
        }
        
        [Test]
        public void ContainerData_Jc3GlobalXml_ByteArrayShouldEqualRTPC_Test()
        {
            // Load XML data
            var xmlPath = @$"{basePath}\PropertyData - 6E14D700.xml";
            var xr = XmlReader.Create(xmlPath);
            xr.MoveToContent();

            var container = new Container();
            container.XmlDeserialize(xr);

            // Process property data
            long coffset = 1524;
            byte[] convertedBlock;
            using (var ms = new MemoryStream())
            {
                container.MemorySerializeData(ms, coffset);
                convertedBlock = ms.ToArray();
            }
            
            
            // Load correct data
            var binaryPath = @$"{basePath}\PropertyData - 6E14D700.rtpc";
            byte[] binaryBlock;
            using (var br = new BinaryReader(new FileStream(binaryPath, FileMode.Open)))
            {
                binaryBlock = br.ReadBytes((int) br.BaseStream.Length);
            }
            
            // Write converted block
            var convertedPath = @$"{basePath}\PropertyData - 6E14D700 CONV.rtpc";
            using (var bw = new BinaryWriter(new FileStream(convertedPath, FileMode.Create)))
            {
                bw.Write(convertedBlock);
            }
            
            Assert.AreEqual(binaryBlock, convertedBlock);
        }
    }
}