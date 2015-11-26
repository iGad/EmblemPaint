using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace EmblemPaint.Kernel
{
    public static class Utilities
    {
        /// <summary>
        /// Получить экземпляр BitmapImage из файла с изображением
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BitmapImage GetImageFromFile(string path)
        {
            Stream stream = new MemoryStream();
            if (File.Exists(path))
            {
                Image.FromFile(path).Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                //using (var fStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                //{
                //    var tempImage = CreateBitmapImage(fStream);
                //    stream = new MemoryStream(tempImage.GetBytes());
                //    stream.Seek(0, SeekOrigin.Begin);
                //}
            }
            else
            {
                //stream = new MemoryStream();
                Resource.defaultThumbnail.Save(stream,ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
            }
            var bitmap = CreateBitmapImage(stream);
            return bitmap;
        }

        private static BitmapImage CreateBitmapImage(Stream stream)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            return bitmap;
        }

        public static BitmapSource GetColorBrushImage(Color color)
        {
            var stream = new MemoryStream();
            Resource.EmptyBrush.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            var bitmap = CreateBitmapImage(stream);
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            var resultBitmap = new WriteableBitmap(width, height, 92, 92, PixelFormats.Bgra32, bitmap.Palette);
            var imageBytes = bitmap.GetBytes();
            DisposeImage(bitmap);
            var po = new ParallelOptions {MaxDegreeOfParallelism = 4};
            Parallel.For(0, width*height, po, i =>
            {
                int index = i*4;
                if (imageBytes[index + 3] < 16)
                {
                    imageBytes[index] = color.B;
                    imageBytes[index + 1] = color.G;
                    imageBytes[index + 2] = color.R;
                    imageBytes[index + 3] = color.A;
                }
            });
            resultBitmap.WritePixels(new Int32Rect(0, 0, width,  height), imageBytes, 4*width, 0);
            if (resultBitmap.CanFreeze)
            {
                resultBitmap.Freeze();
            }
            return resultBitmap;
        }

        /// <summary>
        /// Освободить поток, используемый изображением
        /// </summary>
        /// <param name="image"></param>
        public static void DisposeImage(BitmapImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            try
            {
                if(image.CanFreeze && !image.IsFrozen) image.Freeze();
                image.StreamSource?.Close();
            }
            catch { }
        }

        /// <summary>
        /// Получить список файлов в папке удовлетворяющих заданному шаблону
        /// </summary>
        /// <param name="directory">Папка с файлами</param>
        /// <param name="searchPattern">Шаблон поиска</param>
        /// <returns>Набор имен файлов, удовлетворяющих заданному шаблону</returns>
        public static IEnumerable<string> GetFilesName(string directory, string searchPattern)
        {
            if (directory == null || !Directory.Exists(directory) || String.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Invalid argument. Directory does not exest or parameter is null");
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            var fileInfos = directoryInfo.GetFiles(searchPattern);
            return fileInfos.Select(file => file.Name).ToList();
        }

        public static IEnumerable<string> GetFilesFullName(string directory, string searchPattern)
        {
            if (directory == null || !Directory.Exists(directory) || String.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Invalid argument. Directory does not exest or parameter is null");
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            var fileInfos = directoryInfo.GetFiles(searchPattern);
            return fileInfos.Select(file => file.FullName).ToList();
        }

        /// <summary>
        /// Проверка на то, является ли пиксель цветным
        /// </summary>
        /// <param name="bytes">Байты, соответствующие модели BGR или BGRA</param>
        /// <returns></returns>
        public static bool IsColorPixel(byte[] bytes)
        {
            bool result = bytes.Length == 4 && bytes[3] >= 239;

            return result && (bytes[0] <= 239 ||
                              bytes[1] <= 239 ||
                              bytes[2] <= 239) &&
                   (bytes[0] + bytes[1] + bytes[2] > 128);
        }

        /// <summary>
        /// Более медленная, но и более точная проверка на то, является ли пиксель цветным
        /// </summary>
        /// <param name="bytes">Байты, соответствующие модели BGR или BGRA</param>
        /// <returns></returns>
        public static bool IsColorPixelStrong(byte[] bytes)
        {
            bool result = bytes.Length == 4 && bytes[3] >= 239;

            return result && (bytes[0] <= 239 ||
                              bytes[1] <= 239 ||
                              bytes[2] <= 239) &&
                   (bytes[0] + bytes[1] + bytes[2] > 128) &&
                   (Math.Abs(bytes[0] - bytes[1]) + Math.Abs(bytes[0] - bytes[2]) + Math.Abs(bytes[1] - bytes[2]) > 12);
        }
    }
}
