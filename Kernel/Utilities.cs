using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.DataGrid.Views;

namespace EmblemPaint.Kernel
{
    public static class Utilities
    {
        public static BitmapImage GetImageFromFile(string path)
        {
            Stream stream;
            if (File.Exists(path))
            {
                stream = new FileStream(path, FileMode.Open);
            }
            else
            {
                stream = new MemoryStream();
                Resource.defaultThumbnail.Save(stream,ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
            }
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
