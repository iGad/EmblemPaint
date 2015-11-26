using System;
using System.Windows.Media;
using EmblemPaint.Kernel;
using NUnit.Framework;

namespace KernelTests
{
    [TestFixture]
    public class FillingColorTests
    {
        private bool IsBrushExpected(FillingColor brush, string color)
        {
            return brush.HexArgbColor == color;
        }

        [Test]
        [TestCase("\\AquaBrush.png", "Aqua")]
        [TestCase("\\Aquabrush.png", "Aqua")]
        [TestCase("\\FFAACC.png", "FFAACC")]
        [TestCase("FFAACC.png", "FFAACC")]
        public void ParseWithoutFileExistCheck_WhenDifferentFileName_ReturnExpectedResult(string path, string expectedColorText)
        {
            var result = FillingColor.ParseWithoutFileExistCheck(path);

            Assert.IsTrue(IsBrushExpected(result, expectedColorText));
        }

        [Test]
        public void Parse_WhenPathIsNull_ExceptionExpected()
        {
            TestHelper.Catch<ArgumentNullException>(() => FillingColor.Parse(null));
        }

        [Test]
        public void Parse_WhenPathNotExist_ExceptionExpected()
        {
            TestHelper.Catch<ArgumentException>(() => FillingColor.Parse("/Test.png"));
        }

        [Test]
        [TestCase("FFFF0000", 255, 255, 0, 0)]
        [TestCase("#FFFF0000", 255, 255, 0, 0)]
        [TestCase("FFFFFFFF", 255, 255, 255, 255)]
        [TestCase("00FF00FF", 0, 255, 0, 255)]
        [TestCase("00800010", 0, 128, 0, 16)]
        [TestCase("Aqua", 255, 0, 255, 255)]
        [TestCase("Red", 255, 255, 0, 0)]
        [TestCase("transparent", 0, 255, 255, 255)]
        [TestCase("fskdjfgskdjl", 0, 0, 0, 0)]
        public void GetColor_Always_ReturnExpectedValue(string color, byte a, byte r, byte g, byte b)
        {
            var expectedColor = Color.FromArgb(a, r, g, b);
            var brush = new FillingColor {HexArgbColor = color};

            var result = brush.Color;

            Assert.AreEqual(expectedColor, result);
        }
    }
}
