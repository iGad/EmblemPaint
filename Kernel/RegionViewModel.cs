using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace EmblemPaint.Kernel
{
    public class RegionViewModel : ListViewItemModel
    {
        private readonly Region region;

        public RegionViewModel(Region region)
        {
            this.region = region;
            Name = region.Name;
            Thumbnail = Utilities.GetImageFromFile(region.ThumbnailImageName);
        }

        /// <summary>
        /// 
        /// </summary>
        public Region Region => this.region;

        /// <summary>
        /// 
        /// </summary>
        public string SourceFileName => this.region.SourceImageName;

        /// <summary>
        /// 
        /// </summary>
        public string PatternFileName => this.region.PatternImageName;
    }
}
