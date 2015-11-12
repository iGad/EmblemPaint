using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmblemPaint.Kernel
{
    public class FillingColor
    {
        public FillingColor(string thumbnailName, Color color)
        {
            ThumbnailName = thumbnailName;
            Color = color;
        }

        /// <summary>
        /// 
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// 
        /// </summary>
        public string ThumbnailName { get; }
    }
}
