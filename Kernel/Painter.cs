using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmblemPaint.Kernel
{
    public class Painter
    {
        public const int BytesPerPixel = 4;
        private WriteableBitmap patternImage;
        private readonly int sourceWidth, sourceHeight, stride;
        private readonly byte[] sourceImageBytes, patternImageBytes;

        public Painter(WriteableBitmap patternImage, BitmapSource sourceImage)
        {
            this.patternImage = patternImage;
            SourceImage = sourceImage;
            this.sourceWidth = sourceImage.PixelWidth;
            this.sourceHeight = sourceImage.PixelHeight;
            this.stride = this.sourceWidth*BytesPerPixel;
            this.sourceImageBytes = new byte[this.sourceHeight*this.stride];
            sourceImage.CopyPixels(this.sourceImageBytes, this.stride, 0);
            this.patternImageBytes = new byte[this.sourceHeight * this.stride];
            patternImage.CopyPixels(this.patternImageBytes, this.stride, 0);
        }

        public WriteableBitmap PatternImage
        {
            get
            {
                return this.patternImage;
            }
            set
            {
                this.patternImage = value;
                if(this.patternImage.CanFreeze)
                    this.patternImage.Freeze();
            }
        }

        public BitmapSource SourceImage { get; }

        public WriteableBitmap FillImage(Point startPoint, Color fillingColor)
        {
            if (IsPointInvalid(startPoint))
            {
                throw new ArgumentException("Wrong point coordinates");
            }
            
            Color defaultColor = GetSourceColor((int) startPoint.X, (int) startPoint.Y);
            FillPixels(defaultColor, fillingColor);
            WriteableBitmap filledImage = new WriteableBitmap(PatternImage.Clone());
            filledImage.WritePixels(new Int32Rect(0, 0, this.sourceWidth, this.sourceHeight), this.patternImageBytes, this.stride, 0);
            PatternImage = filledImage;
            return PatternImage;
        }

        private Color GetSourceColor(int x, int y)
        {
            int index = this.stride * y + BytesPerPixel * x;
            return Color.FromArgb(this.sourceImageBytes[index + 3],
                this.sourceImageBytes[index + 2],
                this.sourceImageBytes[index + 1],
                this.sourceImageBytes[index]);
        }


        internal Color GetSourceImageColorByCoordinate(int x, int y)
        {
            int index = this.stride * y + BytesPerPixel * x;
            return Color.FromArgb(this.sourceImageBytes[index + 3],
                this.sourceImageBytes[index + 2],
                this.sourceImageBytes[index + 1],
                this.sourceImageBytes[index]);
        }

        internal void FillImage(Color defaultColor, Color fillingColor)
        {
            WriteableBitmap filledImage = new WriteableBitmap(PatternImage.Clone());
            for (int i = 0; i < this.sourceWidth; i++)
            {
                for (int j = 0; j < this.sourceHeight; j++)
                {
                    if (IsPixelHasExpectedColor(i, j, defaultColor))
                    {
                        FillPixel(filledImage, i, j, fillingColor);
                    }
                }
            }
            PatternImage = new WriteableBitmap(filledImage);
        }

        internal void FillPixel(WriteableBitmap filledImage, int x, int y, Color fillingColor)
        {
            byte[] filledPixel = new[] {fillingColor.R, fillingColor.G, fillingColor.B, fillingColor.A};
            int offset = y * this.stride + 4 * x;
            var rect = new Int32Rect(x, y, 1, 1);
            filledImage.WritePixels(rect, filledPixel, this.stride, 0);
        }

        internal bool IsPixelHasExpectedColor(int x, int y, Color defaultColor)
        {
            int pixelIndex = y*this.stride +4*x;
            return IsPixelHasExpectedColor(pixelIndex, defaultColor);
        }

        private bool IsPixelHasExpectedColor(int index, Color defaultColor)
        {
            return (this.sourceImageBytes[index + 2] == defaultColor.R &&
                   this.sourceImageBytes[index + 1] == defaultColor.G &&
                   this.sourceImageBytes[index] == defaultColor.B &&
                   this.sourceImageBytes[index + 3] == defaultColor.A);
        }

        private bool IsPointInvalid(Point startPoint)
        {
            return startPoint.X < 0 || startPoint.X >= SourceImage.PixelWidth || startPoint.Y < 0 || startPoint.Y >= SourceImage.PixelHeight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels">Пиксели всего PatternImage</param>
        /// <param name="defaultColor"></param>
        /// <param name="fillingColor"></param>
        private void FillPixels(Color defaultColor, Color fillingColor)
        {
            ParallelOptions po = new ParallelOptions {MaxDegreeOfParallelism = 4};
            Parallel.For(0, this.sourceHeight, po, i => FillLine(i, defaultColor,fillingColor));
        }
        

        private void DrawLine(int index, Color defaultColor, Color fillingColor)
        {
            FillLine(index, defaultColor, fillingColor);
        }

        private byte[] GetSourceLine(int index)
        {
            byte[] result = new byte[this.stride];
            Array.Copy(this.sourceImageBytes, this.stride*index, result, 0, this.stride);
            return result;
        }

        private void FillLine(int index, Color defaultColor, Color fillingColor)
        {
            for (var i = 0; i < this.sourceWidth; i++)
            {
                if (IsPixelHasExpectedColor(i, index, defaultColor))
                {
                    FillPixel(this.patternImageBytes, this.stride * index + i*BytesPerPixel, fillingColor);
                }
            }
        }

        private void FillPixel(byte[] pixels, int index, Color color)
        {
            pixels[index] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }

        public int CalculateMatchesPercent()
        {
            int colorPixelCount = 0;
            int matchColorPixelCount = 0;
            var po = new ParallelOptions {MaxDegreeOfParallelism = 4};
            Parallel.For(0, SourceImage.PixelHeight*SourceImage.PixelWidth, po, (i) =>
            {
                if (IsColorPixel(i))
                {
                    colorPixelCount++;
                    if (IsPixelsEquals(i))
                    {
                        matchColorPixelCount++;
                    }
                }
            });
            return Convert.ToInt32(matchColorPixelCount*100/(double) colorPixelCount);
        }

        private bool IsColorPixel(int sourceImagePixelIndex)
        {
            return (this.sourceImageBytes[sourceImagePixelIndex] <= 250 ||
                   this.sourceImageBytes[sourceImagePixelIndex + 1] <= 250 ||
                   this.sourceImageBytes[sourceImagePixelIndex + 2] <= 250) &&
                   this.sourceImageBytes[sourceImagePixelIndex + 3] > 0;
        }

        private bool IsPixelsEquals(int pixelIndex)
        {
            return this.sourceImageBytes[pixelIndex] - this.sourceImageBytes[pixelIndex] <= 5 &&
                   this.sourceImageBytes[pixelIndex + 1] - this.sourceImageBytes[pixelIndex + 1] <= 5 &&
                   this.sourceImageBytes[pixelIndex + 2] - this.sourceImageBytes[pixelIndex + 2] <= 5 &&
                   this.sourceImageBytes[pixelIndex + 3] - this.sourceImageBytes[pixelIndex + 3] <= 5;
        }
    }
}
