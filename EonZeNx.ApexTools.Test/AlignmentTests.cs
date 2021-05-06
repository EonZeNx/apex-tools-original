using System;
using EonZeNx.ApexTools.Core.Utils;
using NUnit.Framework;

namespace EonZeNx.ApexTools.Test
{
    [TestFixture]
    public class AlignmentTests
    {
        [Test]
        public void Align_Value0Align0_Return0()
        {
            var value = ByteUtils.Align(0, 0);
            Assert.AreEqual(0, value);
        }
        
        [Test]
        public void Align_Value1Align0_Return1()
        {
            var value = ByteUtils.Align(1, 0);
            Assert.AreEqual(1, value);
        }
        
        [Test]
        public void Align_Value0Align1_Return1()
        {
            var value = ByteUtils.Align(0, 1);
            Assert.AreEqual(1, value);
        }
        
        [Test]
        public void Align_Value0Align4_Return0()
        {
            var value = ByteUtils.Align(0, 4);
            Assert.AreEqual(4, value);
        }
        
        [Test]
        public void Align_Value5Align4_Return8()
        {
            var value = ByteUtils.Align(5, 4);
            Assert.AreEqual(8, value);
        }
        
        [Test]
        public void Align_Value401Align4_Return404()
        {
            var value = ByteUtils.Align(401, 4);
            Assert.AreEqual(404, value);
        }
        [Test]
        public void Align_Value402Align4_Return404()
        {
            var value = ByteUtils.Align(402, 4);
            Assert.AreEqual(404, value);
        }
        
        [Test]
        public void Align_Value403Align4_Return404()
        {
            var value = ByteUtils.Align(403, 4);
            Assert.AreEqual(404, value);
        }
        
        [Test]
        public void Align_Value404Align4_Return404()
        {
            var value = ByteUtils.Align(404, 4);
            Assert.AreEqual(404, value);
        }
    }
}