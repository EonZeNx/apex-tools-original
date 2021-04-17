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
    public class FourCCProcessorUnitTests
    {
        #region Supported FourCC

        [Test]
        public void IsSupportedFourCC_UnsupportedFourCC_ReturnsFalse()
        {
            var input = new byte[]{0x52, 0x54, 0x50, 0x44};
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.IsSupportedFourCC(input);
            Assert.AreEqual(false, result);
        }
        
        [Test]
        public void IsSupportedFourCC_InputRTPC_ReturnsTrue()
        {
            var input = new byte[]{0x52, 0x54, 0x50, 0x43};  // RTPC
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.IsSupportedFourCC(input);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsSupportedFourCC_InputAAF_ReturnsTrue()
        {
            var input = new byte[]{0x41, 0x41, 0x46, 0x00};  // AAF
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.IsSupportedFourCC(input);
            Assert.AreEqual(true, result);
        }
        
        [Test]
        public void IsSupportedFourCC_InputSARC_ReturnsTrue()
        {
            var input = new byte[]{0x53, 0x41, 0x52, 0x43};  // SARC
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.IsSupportedFourCC(input);
            Assert.AreEqual(true, result);
        }
        
        [Test]
        public void IsSupportedFourCC_InputTAB_ReturnsTrue()
        {
            var input = new byte[]{0x54, 0x41, 0x42, 0x00};  // TAB
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.IsSupportedFourCC(input);
            Assert.AreEqual(true, result);
        }
        
        [Test]
        public void IsSupportedFourCC_InputADF_ReturnsTrue()
        {
            var input = new byte[]{0x20, 0x46, 0x44, 0x41};  // ADF/ FDA
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.IsSupportedFourCC(input);
            Assert.AreEqual(true, result);
        }

        #endregion

        #region Groups of Bytes

        [Test]
        public void ByteInArray_UnsupportedFourCC_ThrowsArgException()
        {
            var input = new byte[]
            {
                0x41, 0x41, 0x46, 0x20, 
                0x01, 0x00, 0x00, 0x00, 
                0x41, 0x56, 0x41, 0x4C, 
                0x41, 0x4E, 0x43, 0x48
            };
            var fourCCProcessor = new FourCCProcessor();
            
            Assert.Throws<ArgumentException>(() => fourCCProcessor.FourCCInByteArray(input));
        }
        
        [Test]
        public void ByteInArray_FirstGroupRTPC_ReturnsByteArray()
        {
            var input = new byte[]
            {
                0x52, 0x54, 0x50, 0x43, // RTPC
                0x01, 0x00, 0x00, 0x00, 
                0x2A, 0x52, 0x7D, 0xAA, 
                0x14, 0x00, 0x00, 0x00
            };
            var expectedOutput = EFoucCC.RTPC;
            
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.FourCCInByteArray(input);
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        public void ByteInArray_FourthGroupRTPC_ReturnsByteArray()
        {
            var input = new byte[]
            {
                0x01, 0x00, 0x00, 0x00, 
                0x2A, 0x52, 0x7D, 0xAA, 
                0x14, 0x00, 0x00, 0x00, 
                0x52, 0x54, 0x50, 0x43 // RTPC
            };
            var expectedOutput = EFoucCC.RTPC;
            
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.FourCCInByteArray(input);
            Assert.AreEqual(expectedOutput, result);
        }
        
        [Test]
        public void ByteInArray_FirstGroupAAF_ReturnsByteArray()
        {
            var input = new byte[]
            {
                0x41, 0x41, 0x46, 0x00, // AAF
                0x01, 0x00, 0x00, 0x00, 
                0x41, 0x56, 0x41, 0x4C, 
                0x41, 0x4E, 0x43, 0x48
            };
            var expectedOutput = EFoucCC.AAF;
            
            var fourCCProcessor = new FourCCProcessor();
            
            var result = fourCCProcessor.FourCCInByteArray(input);
            Assert.AreEqual(expectedOutput, result);
        }
        
        #endregion
        
    }
}