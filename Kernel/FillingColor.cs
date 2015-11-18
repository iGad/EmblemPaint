using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EmblemPaint.Kernel
{
    [Serializable]
    public class FillingColor
    {
        public FillingColor()
        {
            Color = "00000000";
            Name = "Default";
        }

        public FillingColor(string color, string name, string pathToImage)
        {
            Color = color;
            Name = name;
            PathToImage = pathToImage;
        }

        public string Color { get; set; }

        public string PathToImage { get; set; }

        public string Name { get; set; }

        public static FillingColor DefaultBrush => new FillingColor
        {
            Color = "FFFF0000",
            Name = "Default"
        };

        public static string ColorRegex => @"[#]?([0-9]|[a-f]|[A-F]){8}";

        public Color GetColor()
        {
            string hexCode = Regex.Match(Color, ColorRegex).Value.Replace("#", string.Empty);
            if (!string.IsNullOrWhiteSpace(hexCode))
            {
                return GetColor(hexCode);
            }
            return GetSystemColor();
        }

        private Color GetColor(string hexCode)
        {
            return System.Windows.Media.Color.FromArgb(
                byte.Parse(hexCode.Substring(0, 2), NumberStyles.HexNumber),
                byte.Parse(hexCode.Substring(2, 2), NumberStyles.HexNumber),
                byte.Parse(hexCode.Substring(4, 2), NumberStyles.HexNumber),
                byte.Parse(hexCode.Substring(6, 2), NumberStyles.HexNumber));
        }

        private Color GetSystemColor()
        {
            return typeof (Colors)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof (Color) && string.Equals(p.Name, Color, StringComparison.CurrentCultureIgnoreCase))
                .Select(p=> (Color)p.GetValue(null, null)).FirstOrDefault();
        } 

        public static FillingColor Parse(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(paramName: nameof(path), message: "File name cannot be null");
            }
            if (!File.Exists(path))
            {
                throw new ArgumentException("File " + path  + " does not exists!");
            }
            return ParseWithoutFileExistCheck(path);
        }

        internal static FillingColor ParseWithoutFileExistCheck(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            FillingColor brush = DefaultBrush;
            brush.PathToImage = path;
            brush.Name = fileName;
            int brushPosition = fileName.IndexOf("Brush", 0, StringComparison.CurrentCultureIgnoreCase);
            brush.Color = brushPosition > 0 ? fileName.Substring(0, brushPosition) : fileName;
            return brush;
        }
    }
}
