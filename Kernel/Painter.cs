using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmblemPaint.Kernel
{
    public class Painter : IDisposable
    {
        public const int BytesPerPixel = 4;
        private const int EqualtionConcurancy = 10;
        private WriteableBitmap filledImage;
        private readonly int sourceWidth, sourceHeight, stride;
        private byte[] sourceImageBytes, patternImageBytes, sourcePatternImageBytes;

        public Painter(WriteableBitmap filledImage, BitmapImage sourceImage)
        {
            this.filledImage = filledImage;
            SourceImage = sourceImage;
            this.sourceWidth = sourceImage.PixelWidth;
            this.sourceHeight = sourceImage.PixelHeight;
            this.stride = this.sourceWidth*BytesPerPixel;
            this.sourceImageBytes = sourceImage.GetBytes();
            
            this.patternImageBytes = filledImage.GetBytes();
            this.sourcePatternImageBytes = new byte[this.patternImageBytes.Length];
            this.patternImageBytes.CopyTo(this.sourcePatternImageBytes, 0);
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
            }
        }

        /// <summary>
        /// Идеальное изображение текущего герба
        /// </summary>
        public BitmapImage SourceImage { get; private set; }

        /// <summary>
        /// Закрасить все точки шаблона, цвета точки на идеальном изображении, указанным цветом
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="fillingColor"></param>
        /// <returns></returns>
        public WriteableBitmap FillImage(Point startPoint, Color fillingColor)
        {
            return FillImage(startPoint, fillingColor, 0.0);
        }

        /// <summary>
        /// Закрасить все точки шаблона, цвета точки на идеальном изображении, указанным цветом
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="fillingColor"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public WriteableBitmap FillImage(Point startPoint, Color fillingColor, double precision)
        {
            if (IsPointInvalid(startPoint))
            {
                throw new ArgumentException("Wrong point coordinates");
            }
            
            Color defaultColor = GetSourceColor((int)startPoint.X, (int)startPoint.Y);
            FillPixels(defaultColor, fillingColor, precision);
            WriteableBitmap image = new WriteableBitmap(FilledImage.Clone());
            image.WritePixels(new Int32Rect(0, 0, this.sourceWidth, this.sourceHeight), this.patternImageBytes, this.stride, 0);
            FilledImage = image;
            return FilledImage;
        }

        public bool IsFillingPoint(Point point)
        {
            int index = ((int) point.X)*4 + ((int) point.Y)*this.stride;
            return !IsPixelsEquals(index, this.sourceImageBytes, this.sourcePatternImageBytes);
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

        private bool IsPointInvalid(Point startPoint)
        {
            return startPoint.X < 0 || startPoint.X >= SourceImage.PixelWidth || startPoint.Y < 0 || startPoint.Y >= SourceImage.PixelHeight;
        }

        /// <summary>
        /// Замена всех пикселей первого цвета, на второй цвет
        /// </summary>
        /// <param name="defaultColor"></param>
        /// <param name="fillingColor"></param>
        /// <param name="precision"></param>
        private void FillPixels(Color defaultColor, Color fillingColor, double precision)
        {
            ParallelOptions po = new ParallelOptions {MaxDegreeOfParallelism = 4};
            Parallel.For(0, this.sourceHeight, po, i => FillLine(i, defaultColor, fillingColor, precision));
        }
        
        private void FillLine(int index, Color defaultColor, Color fillingColor, double precision)
        {
            for (var i = 0; i < this.sourceWidth; i++)
            {
                if (IsPixelHasExpectedColor(this.sourceImageBytes, i*4 + this.stride*index, defaultColor, precision))
                {
                    FillPixel(this.patternImageBytes, this.stride*index + i*BytesPerPixel, fillingColor);
                }
            }
        }

        private bool IsPixelHasExpectedColor(byte[] array, int index, Color defaultColor, double precision)
        {
            Color color = Color.FromArgb(array[index + 3], array[index + 2], array[index + 1], array[index]);
            return defaultColor.IsNearEqualTo(color, precision);
            //return Math.Abs(array[index] - defaultColor.B) <= precision &&
            //     Math.Abs(array[index + 1] - defaultColor.G) <= precision &&
            //     Math.Abs(array[index + 2] - defaultColor.R) <= precision &&
            //     Math.Abs(array[index + 3] - defaultColor.A) <= precision;
            //return (array[index + 2] == defaultColor.R &&
            //       array[index + 1] == defaultColor.G &&
            //       array[index] == defaultColor.B &&
            //       array[index + 3] == defaultColor.A);
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
                //byte[] pixel = new byte[4];
                int index = i*4;
                //Array.Copy(this.sourceImageBytes, index, pixel, 0, 4);
                if (!IsPixelsEquals(index, this.sourceImageBytes, this.sourcePatternImageBytes))//IsColoringPixel(i))//(Utilities.IsColorPixelStrong(pixel))
                {
                    colorPixelCount++;
                    if (IsPixelsEquals(index, this.sourceImageBytes, this.patternImageBytes))
                    {
                        matchColorPixelCount++;
                    }
                }
            }
            return Convert.ToInt32(Math.Round(matchColorPixelCount/(double) colorPixelCount, 2)*100);
        }
        

        private bool IsPixelsEquals(int pixelIndex, byte[] firstImage, byte[] secondImage)
        {
            return Math.Abs(firstImage[pixelIndex] - secondImage[pixelIndex]) <= EqualtionConcurancy &&
                  Math.Abs(firstImage[pixelIndex + 1] - secondImage[pixelIndex + 1]) <= EqualtionConcurancy &&
                  Math.Abs(firstImage[pixelIndex + 2] - secondImage[pixelIndex + 2]) <= EqualtionConcurancy &&
                  Math.Abs(firstImage[pixelIndex + 3] - secondImage[pixelIndex + 3]) <= EqualtionConcurancy;
        }

        private bool IsSourceAndFilledPixelsEquals(int pixelIndex)
        {
            return Math.Abs(this.sourceImageBytes[pixelIndex] - this.patternImageBytes[pixelIndex]) <= 1 &&
                   Math.Abs(this.sourceImageBytes[pixelIndex + 1] - this.patternImageBytes[pixelIndex + 1]) <= 1 &&
                   Math.Abs(this.sourceImageBytes[pixelIndex + 2] - this.patternImageBytes[pixelIndex + 2]) <= 1 &&
                   Math.Abs(this.sourceImageBytes[pixelIndex + 3] - this.patternImageBytes[pixelIndex + 3]) <= 1;
        }

        #region overrides

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DoDispose();
            }
        }

        protected virtual void DoDispose()
        {
            Utilities.DisposeImage(SourceImage);
            FilledImage = null;
            this.patternImageBytes = null;
            this.sourceImageBytes = null;
            this.sourcePatternImageBytes = null;
        }

        ~Painter()
        {
            Dispose(false);
        }


        #endregion
    }
}
