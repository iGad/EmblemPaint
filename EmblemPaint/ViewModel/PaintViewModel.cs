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
        private ColorViewModel selectedColorViewModel, selectedUpColor, selectedDownColor;
        private double imageWidth, imageHeight, brushContainerHeight, brushContainerWidth, brushWidth = double.MaxValue, brushHeight = double.MaxValue;

        public PaintViewModel(Configuration configuration):base(configuration)
        {
            MouseDownCommand = new DelegateCommand<EventInformation<MouseEventArgs>>(OnMouseDown);
            BrushContainerWidth = Configuration.WindowWidth * (double)169 / 225;
            
            BrushContainerHeight = (Configuration.WindowHeight * 5 / (double)13 - 30) * 3 / 5.5;
        }

        #region Properties

        /// <summary>
        /// Ширина окна
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
        /// Высота контейнера с цветами
        /// </summary>
        public double BrushContainerHeight
        {
            get { return this.brushContainerHeight; }
            set
            {
                if (value > 0 && !this.brushContainerHeight.Equals(value))
                {
                    this.brushContainerHeight = value;
                    BrushHeight = value  - 6;
                }
            }
        }

        /// <summary>
        /// Ширина контейнера с цветами
        /// </summary>
        public double BrushContainerWidth
        {
            get { return this.brushContainerWidth; }
            set
            {
                if (value > 0 && !this.brushContainerWidth.Equals(value))
                {
                    this.brushContainerWidth = value;
                    var width = value/4 - 12;
                    BrushWidth = width;
                }
            }
        }

        /// <summary>
        /// Ширина кисти
        /// </summary>
        public double BrushWidth
        {
            get { return this.brushWidth; }
            set
            {
                if (!this.brushWidth.Equals(value) && value > 0)
                {
                    this.brushWidth = value;
                    OnPropertyChanged(nameof(BrushWidth));
                    OnPropertyChanged(nameof(BrushSize));
                }
            }
        }

        /// <summary>
        /// Высота кисти
        /// </summary>
        public double BrushHeight
        {
            get { return this.brushHeight; }
            set
            {
                if (!this.brushHeight.Equals(value) && value > 0)
                {
                    this.brushHeight = value;
                    OnPropertyChanged(nameof(BrushHeight));
                    OnPropertyChanged(nameof(BrushSize));
                }
            }
        }

        public double BrushSize => Math.Min(BrushWidth, BrushHeight);

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
        public BitmapImage SourceImage { get; protected set; }

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
        public ObservableCollection<ColorViewModel> UpColors { get; protected set; }

        /// <summary>
        /// Набор цветов для закрашивания
        /// </summary>
        public ObservableCollection<ColorViewModel> DownColors { get; protected set; }

        /// <summary>
        /// Набор цветов для закрашивания
        /// </summary>
        public ObservableCollection<ColorViewModel> Colors { get; protected set; } 

        /// <summary>
        /// Команда обработки нажатия на кнопку мыши
        /// </summary>
        public DelegateCommand<EventInformation<MouseEventArgs>> MouseDownCommand { get; }

        /// <summary>
        /// Текущий выбранный цвет закраски
        /// </summary>
        public ColorViewModel SelectedColor => SelectedUpColor??SelectedDownColor;

        /// <summary>
        /// Текущий выбранный цвет закраски
        /// </summary>
        public ColorViewModel SelectedUpColor
        {
            get
            {
                return this.selectedUpColor;
            }
            set
            {
                if (this.selectedUpColor != value)
                {
                    this.selectedUpColor = value;
                    if (value != null)
                    {
                        SelectedDownColor = null;
                    }
                    OnPropertyChanged(nameof(SelectedUpColor));
                }
            }
        }

        /// <summary>
        /// Текущий выбранный цвет закраски
        /// </summary>
        public ColorViewModel SelectedDownColor
        {
            get
            {
                return this.selectedDownColor;
            }
            set
            {
                if (this.selectedDownColor != value)
                {
                    
                    this.selectedDownColor = value;
                    if (value != null)
                    {
                        SelectedUpColor = null;
                    }
                    
                    OnPropertyChanged(nameof(SelectedDownColor));
                }
            }
        }

        #endregion

        private void OnMouseDown(EventInformation<MouseEventArgs> obj)
        {
            var point = obj.EventArgs.GetPosition((IInputElement)obj.Sender);
            OnMouseDown(point);
        }

        protected virtual void OnMouseDown(Point point)
        {
            var normalizePoint = new Point(Convert.ToInt32(point.X * SourceImage.PixelWidth / ImageWidth),
                Convert.ToInt32(point.Y * SourceImage.PixelHeight / ImageHeight));
            Color sourceColor = SourceImage.GetColor((int)normalizePoint.X, (int)normalizePoint.Y);
            byte[] pixel = { sourceColor.B, sourceColor.G, sourceColor.R, sourceColor.A };
            if (this.painter.IsFillingPoint(normalizePoint))/*Utilities.IsColorPixel(pixel)*/
            {
                Utilities.PlaySound("Sounds/pencil_s_1.wav");
                PatternImage = this.painter.FillImage(normalizePoint, SelectedColor.Color, 40);

            }
            //else
            //{
            //    //TODO: сообщаем о том, что выбранная область незакрашиваема
            //}
        }

        protected virtual void Update()
        {
            this.region = Configuration.SelectedRegion;
            if (this.region != null)
            {
                ResetSourceImage();
                OnPropertyChanged(nameof(SourceImage));
                var image = Utilities.GetImageFromFile(this.region.PatternImageName);
                PatternImage = new WriteableBitmap(image);
                OnPropertyChanged(nameof(PatternImage));
                ResetPainter();
                Colors?.Clear();
                Colors = GetColors(this.region);
                UpColors?.Clear();
                UpColors = new ObservableCollection<ColorViewModel>(Colors.Take(4));
                DownColors?.Clear();
                DownColors = new ObservableCollection<ColorViewModel>(Colors.Except(UpColors));
                UpColors.First().IsSelected = true;
            }

            //BrushContainerWidth = Configuration.WindowWidth*(double) 169/225;
            //BrushContainerHeight = Configuration.WindowHeight*(5/(double) 13 - 30)*3/5.5;
        }

        private void ResetSourceImage()
        {
            if (SourceImage != null)
                Utilities.DisposeImage(SourceImage);
                SourceImage = Utilities.GetImageFromFile(this.region.SourceImageName);
                if (SourceImage.CanFreeze)
                    SourceImage.Freeze();
        }

        private void ResetPainter()
        {
            this.painter?.Dispose();
            if (PatternImage != null && SourceImage != null)
            {
                this.painter = new Painter(PatternImage, SourceImage);
            }
        }

        protected ObservableCollection<ColorViewModel> GetColors(Region currentRegion)
        {
            var colors = currentRegion.Colors.Any() ? currentRegion.Colors : Configuration.Colors;
            return new ObservableCollection<ColorViewModel>(colors.Select(color=>new ColorViewModel(color.Color)));
            //return GetColors(currentRegion.Colors.Any() ? currentRegion.Colors : 
            //    Configuration.Colors.Any() ? Configuration.Colors : 
            //    Constants.ByDefaultColors.Select(c => new FillingColor(c)).ToList());
        }

        private ObservableCollection<ColorViewModel> GetColors(IList<FillingColor> fillingColors)
        {
            List<ColorViewModel> collection = new List<ColorViewModel>(fillingColors.Count);
            collection.AddRange(fillingColors.Select(LoadColor));
            return new ObservableCollection<ColorViewModel>(collection);
        }

        protected ColorViewModel LoadColor(FillingColor fillingColor)
        {
            var image = Utilities.GetColorBrushImage(fillingColor.Color);
            return new ColorViewModel(fillingColor.Color) {Thumbnail = image};
        }

        public override void Reconfigure(Configuration newConfig)
        {
            base.Reconfigure(newConfig);
            Update();
        }

        protected override void Home(bool? askUser)
        {
            Configuration.Painter.Dispose();
            Configuration.Painter = null;
            base.Home(askUser);
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
