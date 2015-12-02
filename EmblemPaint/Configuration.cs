using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using EmblemPaint.Kernel;

namespace EmblemPaint
{
    [Serializable]
    public class Configuration : IConfiguration
    {
        public Configuration()
        {
            Storage = RegionsStorage.DefaultStorage;
            Colors = new List<FillingColor>();
        }

        #region Properties

        /// <summary>
        /// Режим изменения информации о регионах
        /// </summary>

        public bool ModifyMode { get; set; } = false;

        /// <summary>
        /// Использовать файл конфигурации для изменения информации о регионах
        /// </summary>

        public bool UseConfigFile { get; set; } = true;

        /// <summary>
        /// Количество элементов в выборе региона по горизонтали
        /// </summary>
        public int HorizontalItemsCount { get; set; }

        /// <summary>
        /// Количество элементов в выборе региона по вертикали
        /// </summary>
        public int VerticalItemsCount { get; set; }

        /// <summary>
        /// Первоначальная ширина окна
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// Первоначальная высота окна
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// Время ожидания действия пользователя перед показом окна о возврате в секундах
        /// </summary>
        public int WaitUserActionInterval { get; set; }

        /// <summary>
        /// Время ожидания ответа пользователя на окно возврата в секундах
        /// </summary>
        public int WaitAnswerInterval { get; set; }

        /// <summary>
        /// Имя или полный путь до папки с изображениями регионов
        /// </summary>
        public string RegionsDirectory { get; set; }

        /// <summary>
        /// Шаблон части имени идеального изображения
        /// </summary>
        public string IdealImageRegex { get; set; }

        /// <summary>
        /// Шаблон части имени закрашиваемого изображения
        /// </summary>
        public string PatternImageRegex { get; set; }

        /// <summary>
        /// Разделитель частей имени файла (имярегиона<разделитель/>шаблон)
        /// </summary>
        public char FileNameSeparator { get; set; }

        /// <summary>
        /// Шаблон части имени изображения миниатюры региона
        /// </summary>
        public string ThumbnailRegex { get; set; }

        /// <summary>
        /// Список цветов которыми будут дополнятся цвета регионов
        /// </summary>
        public List<FillingColor> Colors { get; set; }
        
        /// <summary>
        /// Хранилище информации о регионах
        /// </summary>
        public RegionsStorage Storage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Region SelectedRegion { get; set; }

        /// <summary>
        /// Закрашиватель
        /// </summary>
        [XmlIgnore]
        public Painter Painter { get; set; }

       

        /// <summary>
        /// Конфигурация по умолчанию
        /// </summary>
        public static Configuration DefaultConfiguration => new Configuration
        {
            HorizontalItemsCount = 3,
            VerticalItemsCount = 4,
            WindowHeight = 1920,
            WindowWidth = 1080,
            WaitUserActionInterval = 50,
            WaitAnswerInterval = 30,
            RegionsDirectory = Constants.DefaultStorageDirectoryName,
            FileNameSeparator = '_',
            IdealImageRegex= "ideal",
            PatternImageRegex = "pattern",
            ThumbnailRegex = "thumb"
        };

        #endregion

        public bool Save(Stream stream)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                serializer.Serialize(stream, this);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Загрузка существущей конфигурации из указанного потока
        /// </summary>
        /// <param name="stream">Открытый для чтения поток</param>
        /// <returns>Считаная конфигурация. Если чтение не удалось - пустая конфигурация по умолчанию</returns>
        public static Configuration Load(Stream stream)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (Configuration));
                var configuration = (Configuration) serializer.Deserialize(stream);
                Region comparer = new Region();
                configuration.Storage.Regions.Sort(comparer);
                return configuration;
            }
            catch
            {
                return DefaultConfiguration;
            }
        }

        /// <summary>
        /// Сгенерировать конфигурацию из папок по умолчанию
        /// </summary>
        /// <returns></returns>
        public Configuration GenerateDefault()
        {
            return Generate(Environment.CurrentDirectory, Constants.DefaultStorageDirectoryName, Constants.DefaultBrushesDirectoryName);
        }

        /// <summary>
        /// Папка, в которой будет выполнятся поиск папок с регионами и кистями
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="regionsFolderName"></param>
        /// <param name="brushesFolderName"></param>
        /// <returns></returns>
        public Configuration Generate(string parentFolder, string regionsFolderName, string brushesFolderName)
        {
            var configuration = DefaultConfiguration;
            if (!Directory.Exists(parentFolder))
            {
                return configuration;
            }
            string regionsDirectory = parentFolder == Environment.CurrentDirectory ? regionsFolderName : Path.Combine(parentFolder, regionsFolderName);
            if (Directory.Exists(regionsDirectory))
            {
                configuration.RegionsDirectory = regionsDirectory;
                configuration.Storage = RegionsStorage.Generate(regionsDirectory, CreateRegionSuffixRegexes());
            }

            configuration.Colors = Constants.ByDefaultColors.Select(color => new FillingColor(color)).ToList();
            return configuration;
        }

        private RegionSuffixRegexes CreateRegionSuffixRegexes()
        {
            return new RegionSuffixRegexes(FileNameSeparator, ThumbnailRegex, IdealImageRegex, PatternImageRegex);
        }
    }
}
