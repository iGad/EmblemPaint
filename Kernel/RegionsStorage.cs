using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xceed.Wpf.DataGrid.Converters;

namespace EmblemPaint.Kernel
{
    [Serializable]
    public class RegionsStorage
    {
        private static string folder = Constants.DefaultStorageDirectoryName;
        
        public List<Region> Regions { get; set; } = new List<Region>();

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

        public static RegionsStorage GenerateStorageFromDefaultFolder(RegionSuffixRegexes regionSuffixRegexes)
        {
            return Generate(Constants.DefaultStorageDirectoryName, regionSuffixRegexes);
        }

        internal static RegionsStorage Generate(string folderName, RegionSuffixRegexes regionSuffixRegexes)
        {
            folder = folderName;
            var storage = new RegionsStorage();
            FileInfo[] files = (new DirectoryInfo(folder)).GetFiles("*_*.png");
            var fileNames = Directory.GetFiles(folder);
            var sortedFileNames = files.Select(file => file.Name).ToList();//fileNames.Where(fileName => fileName.IndexOf(regionSuffixRegexes.Separator) < fileName.Length - 1).ToList();
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
            string path = Path.Combine(folder, region.Name + regionSuffixRegexes.Separator + suffix);
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
            }
        }
        
        public Region FindRegion(string name)
        {
            return Regions.FirstOrDefault(region => string.Equals(region.Name, name));
        }

    }
}
