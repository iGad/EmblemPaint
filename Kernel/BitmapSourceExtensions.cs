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
    public static class BitmapSourceExtensions
    {
        public static Color GetColor(this BitmapSource image, int x, int y)
        {
            byte[] colorBytes = new byte[Constants.BytesPerPixel];
            image.CopyPixels(new Int32Rect(x, y, 1, 1), colorBytes, image.PixelWidth*Constants.BytesPerPixel, 0);
            return Color.FromArgb(colorBytes[3], colorBytes[2], colorBytes[1], colorBytes[0]);
        }

        public static byte[] GetBytes(this BitmapSource image)
        {
            byte[] bytes = new byte[image.PixelWidth*Constants.BytesPerPixel*image.PixelHeight];
            image.CopyPixels(bytes, image.PixelWidth*Constants.BytesPerPixel, 0);
            return bytes;
        }

        public static byte[] GetLineBytes(this BitmapSource image, int line)
        {
            //TODO: проверки на line
            byte[] bytes = new byte[image.PixelWidth * Constants.BytesPerPixel];
            var stride = image.PixelWidth*Constants.BytesPerPixel;
            image.CopyPixels(new Int32Rect(0, line, stride, 1), bytes, stride, 0);
            return bytes;
        }
    }
}
