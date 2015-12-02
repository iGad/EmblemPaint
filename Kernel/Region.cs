
using System;
using System.Collections.Generic;

namespace EmblemPaint.Kernel
{
    [Serializable]
    public class Region : IComparer<Region>
    {
        /// <summary>
        /// Конструктор для сериализации
        /// </summary>
        public Region() { }

        public Region(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Путь к файлу с иконкой региона
        /// </summary>
        public string ThumbnailImageName { get; set; }

        /// <summary>
        /// Название региона
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Путь к изображению с идеальным гербом
        /// </summary>
        public string SourceImageName { get; set; }

        /// <summary>
        /// Путь к изображению с шаблоном зарисовки
        /// </summary>
        public string PatternImageName { get; set; }

        /// <summary>
        /// Краткое описание герба
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Цвета для закраски
        /// </summary>
        public List<FillingColor> Colors { get; set; } = new List<FillingColor>();

        public int Compare(Region x, Region y)
        {
            return x.Name.CompareTo(y.Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
