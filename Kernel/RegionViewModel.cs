namespace EmblemPaint.Kernel
{
    /// <summary>
    /// Модель для отображения региона для его выбора
    /// </summary>
    public class RegionViewModel : ListViewItemModel
    {
        private readonly Region region;

        public RegionViewModel(Region region)
        {
            this.region = region;
            Name = region.Name;
            Thumbnail = Utilities.GetImageFromFile(region.PatternImageName);
            if(Thumbnail.CanFreeze)
                Thumbnail.Freeze();
        }

        /// <summary>
        /// Данный регион
        /// </summary>
        public Region Region => this.region;

        /// <summary>
        /// Путь к идеальному изображению
        /// </summary>
        public string SourceFileName => this.region.SourceImageName;

        /// <summary>
        /// Путь к шаблонному изображению
        /// </summary>
        public string PatternFileName => this.region.PatternImageName;
    }
}
