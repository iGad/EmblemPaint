using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmblemPaint.Kernel
{
    /// <summary>
    /// Базовый класс для элементов ListView
    /// </summary>
    public class ListViewItemModel : ViewModel
    {
        private double size = Constants.DefaultRegionSize;
        private BitmapSource thumbnail;
        private string name;
        private bool isSelected;
        
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (!string.Equals(this.name, value))
                {
                    this.name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected != value )
                {
                    this.isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BitmapSource Thumbnail
        {
            get { return this.thumbnail; }
            set
            {
                this.thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Size
        {
            get { return this.size; }
            set
            {
                if (!this.size.Equals(value))
                {
                    this.size = value;
                    OnPropertyChanged(nameof(Size));
                }
            }
        }
    }
}
