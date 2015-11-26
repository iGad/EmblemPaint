using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace EmblemPaint.Kernel
{
    [Serializable]
    public class RegionsStorage
    {
        private static string folder = Constants.DefaultStorageDirectoryName;
        
        public List<Region> Regions { get; set; } = new List<Region>();

        public static RegionsStorage DefaultStorage => new RegionsStorage();

        public static RegionsStorage Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RegionsStorage));
            return (RegionsStorage) serializer.Deserialize(stream);
        }

        public void Save(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(stream, this);
        }

        /// <summary>
        /// Сгенерировать хранилище регионов из изображений
        /// </summary>
        /// <param name="path">Путь к папке с изображениями</param>
        /// <param name="regionSuffixRegexes">Набор регулярных выражений для парсинга названий</param>
        /// <returns>Хранидище регионов</returns>
        public static RegionsStorage Generate(string path, RegionSuffixRegexes regionSuffixRegexes)
        {
            if (!Directory.Exists(path))
            {
                return DefaultStorage;
            }
            
            folder = path;
            var storage = new RegionsStorage();
            FileInfo[] files = (new DirectoryInfo(folder)).GetFiles("*_*.png");
            var sortedFileNames = files.Select(file => file.Name).ToList();
            sortedFileNames.Sort();
            var separtedFileNames = sortedFileNames.Select(file=>file.Split(regionSuffixRegexes.Separator)).ToList();

            FillStorage(storage, separtedFileNames, regionSuffixRegexes);
            
            return storage;
        }

        internal static void FillStorage(RegionsStorage storage, IList<string[]> fileNamesParts, RegionSuffixRegexes regionSuffixRegexes)
        {
            foreach (string[] fileNameStrings in fileNamesParts)
            {
                AddOrAppendRegion(storage, fileNameStrings, regionSuffixRegexes);
            }
        }

        private static void AddOrAppendRegion(RegionsStorage storage, string[] fileNameParts, RegionSuffixRegexes regionSuffixRegexes)
        {
            var region = storage.FindRegion(fileNameParts[0]);
            if (region == null)
            {
                region = new Region(fileNameParts[0]);
                storage.Regions.Add(region);
            }
            FillRegion(region, fileNameParts[1], regionSuffixRegexes);
        }

        private static void FillRegion(Region region, string suffix, RegionSuffixRegexes regionSuffixRegexes)
        {
            string folderName = folder;
            if (folder.Contains(Environment.CurrentDirectory))
            {
                folderName = Path.GetDirectoryName(folder) ?? folder;
            }
            string path = Path.Combine(folderName, region.Name + regionSuffixRegexes.Separator + suffix);
            if (regionSuffixRegexes.ThumbnailRegex.IsMatch(suffix))
            {
                region.ThumbnailImageName= path;
                return;
            }
            if (regionSuffixRegexes.PatternImageRegex.IsMatch(suffix))
            {
                region.PatternImageName = path;
                return;
            }
            if (regionSuffixRegexes.SourceImageRegex.IsMatch(suffix))
            {
                region.SourceImageName = path;
                if (File.Exists(path))
                {
                    region.Colors = GenerateColorsForImage(path);
                }
            }
        }

        internal static List<FillingColor> GenerateColorsForImage(string imagePath)
        {

            var localColors = GetColorsForImage(imagePath);

            AppendColors(localColors);

            return localColors;
        }

        internal static List<FillingColor> GetColorsForImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("File " + imagePath + " not found");
            }

            var image = Utilities.GetImageFromFile(imagePath);
            var imageBytes = image.GetBytes();
            Utilities.DisposeImage(image);
            var count = imageBytes.Length / 4;
            var localColors = new List<FillingColor>();

            for (int i = 0; i < count && localColors.Count < 10; i += 10)
            {
                var pixel = new byte[4];
                int index = i*4;
                Array.Copy(imageBytes, index, pixel, 0, 4);
                if (Utilities.IsColorPixelStrong(pixel))
                {
                    Color color = Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
                    if (!localColors.Any(f => f.Color.IsNearEqualTo(color, 100)))
                    {
                        localColors.Add(new FillingColor(color));
                    }

                }
            }
            return localColors;
        }

        /// <summary>
        /// Добавляет цвета для того, чтобы их количество в палитре стало равно 7
        /// </summary>
        /// <param name="colors">Список цветов из изображения</param>
        internal static void AppendColors(IList<FillingColor> colors)
        {
            if (colors.Count >= 7)
            {
                return;
            }
            int i = 0;
            while (colors.Count < 7 && i < Constants.ByDefaultColors.Count)
            {
                var minDifference = colors.Select(color => color.Color.DistanceTo(Constants.ByDefaultColors.ElementAt(i))).Min();
                if (minDifference > 100)
                {
                    colors.Add(new FillingColor {HexArgbColor = Constants.ByDefaultColors.ElementAt(i).ToHexString()});
                }
                i++;
            }
        } 

        public Region FindRegion(string name)
        {
            return Regions.FirstOrDefault(region => string.Equals(region.Name, name));
        }

    }
}
