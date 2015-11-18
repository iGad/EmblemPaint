using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace EmblemPaint.Kernel
{
    public static class Utilities
    {
        public static BitmapImage GetImageFromFile(string path)
        {
            Stream stream;
            if (File.Exists(path))
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
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

        public static IEnumerable<string> GetFilesName(string directory, string searchPattern)
        {
            if (directory == null || !Directory.Exists(directory) || string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Invalid argument. Directory does not exest or parameter is null");
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            var fileInfos = directoryInfo.GetFiles(searchPattern);
            return fileInfos.Select(file => file.Name).ToList();
        }

        public static IEnumerable<string> GetFilesFullName(string directory, string searchPattern)
        {
            if (directory == null || !Directory.Exists(directory) || string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Invalid argument. Directory does not exest or parameter is null");
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            var fileInfos = directoryInfo.GetFiles(searchPattern);
            return fileInfos.Select(file => file.FullName).ToList();
        }

    }
}
