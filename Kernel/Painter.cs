using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmblemPaint.Kernel
{
    public class Painter
    {
        public const int BytesPerPixel = 4;
        private const int EqualtionConcurancy = 20;
        private WriteableBitmap filledImage;
        private readonly int sourceWidth, sourceHeight, stride;
        private readonly byte[] sourceImageBytes, patternImageBytes;

        public Painter(WriteableBitmap filledImage, BitmapSource sourceImage)
        {
            this.filledImage = filledImage;
            SourceImage = sourceImage;
            this.sourceWidth = sourceImage.PixelWidth;
            this.sourceHeight = sourceImage.PixelHeight;
            this.stride = this.sourceWidth*BytesPerPixel;
            this.sourceImageBytes = sourceImage.GetBytes();
            this.patternImageBytes = filledImage.GetBytes();
        }
        
        /// <summary>
        /// Заполненное пользователем изображение текущего герба
        /// </summary>
        public WriteableBitmap FilledImage
        {
            get
            {
                return this.filledImage;
            }
            set
            {
                this.filledImage = value;
                if(this.filledImage.CanFreeze)
                    this.filledImage.Freeze();
            }
        }

        /// <summary>
        /// Идеальное изображение текущего герба
        /// </summary>
        public BitmapSource SourceImage { get; private set; }

        /// <summary>
        /// Закрасить все точки шаблона, цвета точки на идеальном изображении, указанным цветом
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="fillingColor"></param>
        /// <returns></returns>
        public WriteableBitmap FillImage(Point startPoint, Color fillingColor)
        {
            if (IsPointInvalid(startPoint))
            {
                throw new ArgumentException("Wrong point coordinates");
            }
            
            Color defaultColor = GetSourceColor((int) startPoint.X, (int) startPoint.Y);
            FillPixels(defaultColor, fillingColor);
            WriteableBitmap image = new WriteableBitmap(FilledImage.Clone());
            image.WritePixels(new Int32Rect(0, 0, this.sourceWidth, this.sourceHeight), this.patternImageBytes, this.stride, 0);
            FilledImage = image;
            return FilledImage;
        }

        internal Color GetSourceColor(int x, int y)
        {
            int index = this.stride * y + BytesPerPixel * x;
            return Color.FromArgb(this.sourceImageBytes[index + 3],
                this.sourceImageBytes[index + 2],
                this.sourceImageBytes[index + 1],
                this.sourceImageBytes[index]);
        }

        internal void FillPixel(WriteableBitmap fillingImage, int x, int y, Color fillingColor)
        {
            byte[] filledPixel = {fillingColor.R, fillingColor.G, fillingColor.B, fillingColor.A};
            var rect = new Int32Rect(x, y, 1, 1);
            fillingImage.WritePixels(rect, filledPixel, this.stride, 0);
        }

        internal bool IsPixelHasExpectedColor(int x, int y, Color defaultColor)
        {
            int pixelIndex = y*this.stride + 4*x;
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
        /// Замена всех пикселей первого цвета, на второй цвет
        /// </summary>
        /// <param name="defaultColor"></param>
        /// <param name="fillingColor"></param>
        private void FillPixels(Color defaultColor, Color fillingColor)
        {
            ParallelOptions po = new ParallelOptions {MaxDegreeOfParallelism = 4};
            Parallel.For(0, this.sourceHeight, po, i => FillLine(i, defaultColor,fillingColor));
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

        public int CalculateFillAccuracy()
        {
            int colorPixelCount = 0;
            int matchColorPixelCount = 0;
            int count = SourceImage.PixelHeight*SourceImage.PixelWidth;
            for (int i=0; i < count; i++)
            {
                byte[] pixel = new byte[4];
                int index = i*4;
                Array.Copy(this.sourceImageBytes, index, pixel, 0, 4);
                if (Utilities.IsColorPixelStrong(pixel))
                {
                    colorPixelCount++;
                    if (IsPixelsEquals(index))
                    {
                        matchColorPixelCount++;
                    }
                }
            }
            return Convert.ToInt32(Math.Round(matchColorPixelCount/(double) colorPixelCount, 2)*100);
        }
        

        private bool IsPixelsEquals(int pixelIndex)
        {
            return Math.Abs(this.sourceImageBytes[pixelIndex] - this.patternImageBytes[pixelIndex]) <= EqualtionConcurancy &&
                   Math.Abs(this.sourceImageBytes[pixelIndex + 1] - this.patternImageBytes[pixelIndex + 1]) <= EqualtionConcurancy &&
                   Math.Abs(this.sourceImageBytes[pixelIndex + 2] - this.patternImageBytes[pixelIndex + 2]) <= EqualtionConcurancy &&
                   Math.Abs(this.sourceImageBytes[pixelIndex + 3] - this.patternImageBytes[pixelIndex + 3]) <= EqualtionConcurancy;
        }
    }
}
