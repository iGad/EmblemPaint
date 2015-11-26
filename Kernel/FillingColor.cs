using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml.Serialization;

namespace EmblemPaint.Kernel
{
    [Serializable]
    public class FillingColor
    {
        public FillingColor()
        {
            HexArgbColor = "00000000";
        }

        public FillingColor(Color color)
        {
            HexArgbColor = color.ToHexString();
        }
        
        public string HexArgbColor { get; set; }
        
        [XmlIgnore]
        public Color Color
        {
            get
            {
                string hexCode = Regex.Match(HexArgbColor, ColorRegex).Value.Replace("#", string.Empty);
                return !string.IsNullOrWhiteSpace(hexCode) ? GetColor(hexCode) : GetSystemColor();
            }
        }

        public static FillingColor DefaultBrush => new FillingColor
        {
            HexArgbColor = "FFFF0000"
        };

        public static string ColorRegex => @"[#]?([0-9]|[a-f]|[A-F]){8}";

        private Color GetColor(string hexCode)
        {
            return Color.FromArgb(
                byte.Parse(hexCode.Substring(0, 2), NumberStyles.HexNumber),
                byte.Parse(hexCode.Substring(2, 2), NumberStyles.HexNumber),
                byte.Parse(hexCode.Substring(4, 2), NumberStyles.HexNumber),
                byte.Parse(hexCode.Substring(6, 2), NumberStyles.HexNumber));
        }

        private Color GetSystemColor()
        {
            return typeof (Colors)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof (Color) && string.Equals(p.Name, HexArgbColor, StringComparison.CurrentCultureIgnoreCase))
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
            int brushPosition = fileName.IndexOf("Brush", 0, StringComparison.CurrentCultureIgnoreCase);
            brush.HexArgbColor = brushPosition > 0 ? fileName.Substring(0, brushPosition) : fileName;
            return brush;
        }
    }
}
