using System;
using System.IO;
using System.Reflection.Metadata;
using NUnit.Framework;

namespace A01.Tests
{
    /*
     * File type processor:
     * - Takes a path as a string.
     * - Open the file stream.
     * - Read the first 16 bytes.
     * - Iterate over groups of 4 bytes and...
     *   - determine if supported FourCC.
     */
    
    [TestFixture]
    public class FourCCProcessorIntegrationTests
    {
        public const string RTPC_FILEPATH = @"E:\Projects\Just Cause Tools\Apex Engine Tools\tests\rtpc\jc3\heat.blo";
        public const string AAF_FILEPATH = @"E:\Projects\Just Cause Tools\Apex Engine Tools\tests\aaf\jc3\main_character.ee";
        
        #region Get 16 bytes from File

        [Test]
        public void FourCCProcessor_RTPCFourCC_ReturnsByteArray()
        {
            var expected = new byte[]
            {
                0x52, 0x54, 0x50, 0x43, // RTPC
                0x01, 0x00, 0x00, 0x00, 
                0x2A, 0x52, 0x7D, 0xAA, 
                0x14, 0x00, 0x00, 0x00
            };

            var processor = new FourCCProcessor();
            var result = processor.GetFirst16Bytes(RTPC_FILEPATH);
            
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void FourCCProcessor_AAFFourCC_ReturnsByteArray()
        {
            var expected = new byte[]
            {
                0x41, 0x41, 0x46, 0x00, // AAF
                0x01, 0x00, 0x00, 0x00, 
                0x41, 0x56, 0x41, 0x4C, 
                0x41, 0x4E, 0x43, 0x48
            };

            var processor = new FourCCProcessor();
            var result = processor.GetFirst16Bytes(AAF_FILEPATH);
            
            Assert.AreEqual(expected, result);
        }

        #endregion
        
    }
}