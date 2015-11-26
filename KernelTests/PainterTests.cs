using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EmblemPaint.Kernel;
using NUnit.Framework;

namespace KernelTests
{
    [TestFixture]
    public class PainterTests
    {
        private List<Color> testColors = new List<Color> {Colors.Black, Colors.White, Colors.Red, Colors.Green, Colors.Blue};

        private bool IsPixelHasExpectedColor(BitmapSource image, int x, int y, Color expectedColor)
        {
            byte[] pixelBytes = new byte[4];
            int stride = image.PixelWidth*4;
            image.CopyPixels(new Int32Rect(x, y, 1, 1), pixelBytes, stride, 0);

            return pixelBytes[0] == expectedColor.B &&
                   pixelBytes[1] == expectedColor.G &&
                   pixelBytes[2] == expectedColor.R &&
                   pixelBytes[3] == expectedColor.A;
        }

        /// <summary>
        /// Получить тестовое заполненное изображение
        /// </summary>
        /// <returns>Изображение, состоящее из 25 пикселей, 4 из которых красные, 4 синие, 4 зеленые и 4 белые и 9 черных (сетка, между ячейками черные полосы в 1 пиксель)</returns>
        private BitmapSource GetTestSourceImage()
        {
            //WriteableBitmap result = new WriteableBitmap(5,5,72,72,PixelFormats.Bgra32, new BitmapPalette(this.testColors));
            int stride = 20;
            byte[] firstLine = 
            {
                255, 255, 255, 255,
                255, 255, 255, 255,
                0, 0, 0, 255,
                0, 0, 255, 255,
                0, 0, 255, 255
            };
            byte[] middleLine =
            {
                0,0,0, 255,
                0,0,0, 255,
                0, 0, 0, 255,
                0,0,0, 255,
                0,0,0, 255,
            };
            byte[] lastLine =
            {
                0, 128, 0, 255,
                0, 128, 0, 255,
                0, 0, 0, 255,
                255, 0, 0, 255,
                255, 0, 0, 255
            };
            byte[] imageBytes = new byte[25*4];
            Array.Copy(firstLine, 0, imageBytes, 0, stride);
            Array.Copy(firstLine, 0, imageBytes, stride, stride);
            Array.Copy(middleLine, 0, imageBytes, stride*2, stride);
            Array.Copy(lastLine, 0, imageBytes, stride*3, stride);
            Array.Copy(lastLine, 0, imageBytes, stride*4, stride);
            return BitmapSource.Create(5, 5, 96, 96, PixelFormats.Bgra32, new BitmapPalette(this.testColors), imageBytes, stride);
        }

        /// <summary>
        /// Получить тестовое незаполненное изображение
        /// </summary>
        /// <returns>Изображение, состоящее из 25 пикселей, 16 из которых белые и 9 черных (сетка, между ячейками черные полосы в 1 пиксель)</returns>
        private WriteableBitmap GetTestPatternImage()
        {
            WriteableBitmap result = new WriteableBitmap(5,5,96,96,PixelFormats.Bgra32, new BitmapPalette(this.testColors));
            int stride = 20;
            byte[] firstLine =
            {
                255, 255, 255, 255,
                255, 255, 255, 255,
                0, 0, 0, 255,
                255, 255, 255, 255,
                255, 255, 255, 255,
            };
            byte[] middleLine =
            {
                0,0,0, 255,
                0,0,0, 255,
                0, 0, 0, 255,
                0,0,0, 255,
                0,0,0, 255,
            };
            byte[] imageBytes = new byte[25 * 4];
            Array.Copy(firstLine, 0, imageBytes, 0, stride);
            Array.Copy(firstLine, 0, imageBytes, stride, stride);
            Array.Copy(middleLine, 0, imageBytes, stride * 2, stride);
            Array.Copy(firstLine, 0, imageBytes, stride * 3, stride);
            Array.Copy(firstLine, 0, imageBytes, stride * 4, stride);
            result.WritePixels(new Int32Rect(0,0,5,5), imageBytes,stride,0);
            return result;
        }

        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(0, 1, 1)]
        [TestCase(1, 1, 1)]
        [TestCase(0, 2, 0)]
        [TestCase(2, 0, 0)]
        [TestCase(0, 3, 2)]
        [TestCase(1, 4, 2)]
        [TestCase(3, 0, 3)]
        [TestCase(4, 1, 3)]
        [TestCase(3, 3, 4)]
        [TestCase(4, 4, 4)]
        public void GetSourceImageColorByCoordinate_Always_ReturnExpectedValue(int y, int x, int colorIndex)
        {
            var painter = new Painter(GetTestPatternImage(), GetTestSourceImage());
            var expectedColor = this.testColors[colorIndex];

            var result = painter.GetSourceColor(x, y);

            Assert.AreEqual(expectedColor, result);
        }


        [Test]
        [TestCase(0, 0, 1)]
        [TestCase(0, 1, 1)]
        [TestCase(1, 1, 1)]
        [TestCase(0, 2, 0)]
        [TestCase(2, 0, 0)]
        [TestCase(0, 3, 2)]
        [TestCase(1, 4, 2)]
        [TestCase(3, 0, 3)]
        [TestCase(4, 1, 3)]
        [TestCase(3, 3, 4)]
        [TestCase(4, 4, 4)]
        public void FillPixel_Always_FillExpectedPixelByExpectedColor(int y, int x, int colorIndex)
        {
            var painter = new Painter(GetTestPatternImage(), GetTestSourceImage());
            var expectedColor = this.testColors[colorIndex];

            painter.FillPixel(painter.FilledImage, x, y, expectedColor);

            Assert.IsTrue(IsPixelHasExpectedColor(painter.FilledImage, x, y, expectedColor));
        }
    }
}
