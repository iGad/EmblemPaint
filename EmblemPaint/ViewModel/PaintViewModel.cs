using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EmblemPaint.Kernel;
using Prism.Commands;

namespace EmblemPaint.ViewModel
{
    /// <summary>
    /// Модель окна раскрашивания
    /// </summary>
    public class PaintViewModel : FunctionalViewModel
    {
        private Region region;
        private WriteableBitmap patternImage;
        private Painter painter;
        private ColorViewModel selectedColorViewModel;
        private double imageWidth, imageHeight, brushWidth, brushHeight;

        public PaintViewModel(Configuration configuration):base(configuration)
        {
            MouseDownCommand = new DelegateCommand<EventInformation<MouseEventArgs>>(OnMouseDown);
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public double WindowWidth
        {
            set
            {
                if (value > 0)
                {
                    BrushWidth = value/Colors.Count - 10;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double BrushContainerHeight
        {
            set
            {
                if (value > 0)
                {
                    BrushHeight = value - 10;
                }
            }
        }

        public double BrushWidth
        {
            get { return this.brushWidth; }
            set
            {
                if (!this.brushWidth.Equals(value))
                {
                    this.brushWidth = value;
                    OnPropertyChanged(nameof(BrushWidth));
                }
            }
        }

        public double BrushHeight
        {
            get { return this.brushHeight; }
            set
            {
                if (!this.brushHeight.Equals(value))
                {
                    this.brushHeight = value;
                    OnPropertyChanged(nameof(BrushHeight));
                }
            }
        }

        /// <summary>
        /// Ширина изображения на экране
        /// </summary>
        public double ImageWidth
        {
            get { return this.imageWidth; }
            set
            {
                this.imageWidth = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        /// <summary>
        /// Высота изображения на экране
        /// </summary>
        public double ImageHeight
        {
            get { return this.imageHeight; }
            set
            {
                this.imageHeight = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }

        /// <summary>
        /// Идеальное изображение
        /// </summary>
        public BitmapImage SourceImage { get; private set; }

        /// <summary>
        /// Закрашиваемое изображение
        /// </summary>
        public WriteableBitmap PatternImage
        {
            get { return this.patternImage; }
            set
            {
                this.patternImage = value;
                OnPropertyChanged(nameof(PatternImage));
            }
        }

        /// <summary>
        /// Набор цветов для закрашивания
        /// </summary>
        public ObservableCollection<ColorViewModel> Colors { get; private set; } 

        /// <summary>
        /// Команда обработки нажатия на кнопку мыши
        /// </summary>
        public DelegateCommand<EventInformation<MouseEventArgs>> MouseDownCommand { get; }

        /// <summary>
        /// Текущий выбранный цвет закраски
        /// </summary>
        public ColorViewModel SelectedColor
        {
            get
            {
                return this.selectedColorViewModel;
            }
            set
            {
                if (this.selectedColorViewModel != value)
                {
                    this.selectedColorViewModel = value;
                    OnPropertyChanged(nameof(SelectedColor));
                }
            }
        }

        #endregion

        private void OnMouseDown(EventInformation<MouseEventArgs> obj)
        {
            var point = obj.EventArgs.GetPosition((IInputElement)obj.Sender);
            var normalizePoint = new Point(Convert.ToInt32(point.X * SourceImage.PixelWidth / ImageWidth),
                Convert.ToInt32(point.Y * SourceImage.PixelHeight / ImageHeight));
            Color sourceColor = SourceImage.GetColor((int)normalizePoint.X, (int)normalizePoint.Y);
            byte[] pixel = { sourceColor.B, sourceColor.G, sourceColor.R, sourceColor.A };
            if (Utilities.IsColorPixel(pixel))
            {
                PatternImage = this.painter.FillImage(normalizePoint, SelectedColor.Color);

            }
            else
            {
                //TODO: сообщаем о том, что выбранная область незакрашиваема
                //FilledImage = this.painter.FillImage(normalizePoint, SelectedColor.Color);
            }
        }

        private void Update()
        {
            this.region = Configuration.SelectedRegion;
            if (this.region != null)
            {
                ResetSourceImage();
                var image = Utilities.GetImageFromFile(this.region.PatternImageName);
                PatternImage = new WriteableBitmap(image);
                this.painter = new Painter(this.patternImage, SourceImage);
                if (Colors != null)
                    Colors.Clear();
                Colors = GetColors();
                Colors.First().IsSelected = true;
            }
           
            WindowWidth = Configuration.WindowWidth;
            BrushContainerHeight = Configuration.WindowHeight*(double)4/45;
        }

        private void ResetSourceImage()
        {
            if (SourceImage != null)
                Utilities.DisposeImage(SourceImage);
                SourceImage = Utilities.GetImageFromFile(this.region.SourceImageName);
                if (SourceImage.CanFreeze)
                    SourceImage.Freeze();
        }

        private ObservableCollection<ColorViewModel> GetColors()
        {
            return GetColors(this.region.Colors.Any() ? this.region.Colors : 
                Configuration.Colors.Any() ? Configuration.Colors : 
                Constants.ByDefaultColors.Select(c => new FillingColor(c)).ToList());
        }

        private ObservableCollection<ColorViewModel> GetColors(IList<FillingColor> fillingColors)
        {
            List<ColorViewModel> collection = new List<ColorViewModel>(fillingColors.Count);
            collection.AddRange(fillingColors.Select(LoadColor));
            return new ObservableCollection<ColorViewModel>(collection);
        }

        private ColorViewModel LoadColor(FillingColor fillingColor)
        {
            var image = Utilities.GetColorBrushImage(fillingColor.Color);
            return new ColorViewModel(fillingColor.Color) {Thumbnail = image};
        }

        public override void Reconfigure(Configuration newConfig)
        {
            base.Reconfigure(newConfig);
            Update();
        }

        protected override void Next()
        {
            Configuration.Painter = this.painter;
            base.Next();
        }

        protected override void DoDispose()
        {
            ((BitmapImage)this.painter.SourceImage)?.StreamSource?.Close();
            Colors.Clear();
        }
    }
}
