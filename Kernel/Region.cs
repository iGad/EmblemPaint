
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmblemPaint.Kernel
{
    [Serializable]
    public class Region
    {
        /// <summary>
        /// Конструктор для сериализации
        /// </summary>
        public Region() { }

        public Region(string name)
        {
            Name = name;
        }

        public string ThumbnailImageName { get; set; }

        public string Name { get; set; }

        public string SourceImageName { get; set; }

        public string PatternImageName { get; set; }
    }
}
