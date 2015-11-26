using System;
using System.Windows.Media;
using EmblemPaint.Kernel;
using NUnit.Framework;

namespace KernelTests
{
    [TestFixture]
    public class ColorExtensionsTests
    {
        [Test]
        [TestCase(255,0,0,"FFFF0000")]
        [TestCase(255, 255, 255, "FFFFFFFF")]
        [TestCase(0, 0, 0, "FF000000")]
        [TestCase(0, 128, 0, "FF008000")]
        [TestCase(0,0,0,"FF000000")]
        [TestCase(10, 15, 16, "FF0A0F10")]
        public void ToHexString_Always_ReturnExpectedValue(byte r, byte g, byte b, string expectedResult)
        {
            Color color = Color.FromRgb(r, g, b);

            string result = color.ToHexString();

            Assert.AreEqual(expectedResult, result.ToUpperInvariant());
        }

        [Test]
        [TestCase(255, 0, 0, 255, 0, 0, 0.0)]
        [TestCase(255, 255, 255, 255, 255, 0, 255.0)]
        [TestCase(0, 50, 255, 3, 50, 251, 5.0)]
        public void DistanceTo_Always_ReturnExpectedValue(byte r, byte g, byte b, byte r2, byte g2, byte b2, double expectedResult)
        {
            Color color = Color.FromRgb(r, g, b);

            var result = color.DistanceTo(Color.FromRgb(r2, g2, b2));

            Assert.AreEqual(expectedResult, result);
        }
    }
}
