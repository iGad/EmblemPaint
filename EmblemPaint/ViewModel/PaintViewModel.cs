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
    public class PaintViewModel : FunctionalViewModel
    {
        private Region region;
        private WriteableBitmap regionSymbol;
        private BitmapImage sourceImage;
        private Painter painter;
        private ColorViewModel selectedColorViewModel;
        private double imageWidth, imageHeight;

        public PaintViewModel(Configuration configuration):base(configuration)
        {
            MouseDownCommand = new DelegateCommand<EventInformation<MouseEventArgs>>(OnMouseDown);
            this.region = configuration.SelectedRegion;
            this.sourceImage = Utilities.GetImageFromFile(this.region.SourceImageName);
            var symbolImage = Utilities.GetImageFromFile(this.region.PatternImageName);
            this.regionSymbol = new WriteableBitmap(symbolImage);
            this.painter = new Painter(this.regionSymbol, this.sourceImage);
            Colors = GetColors();
            Colors.First().IsSelected = true;
            //TODO: Проверка на равенство размеров изображений и соответствующие действия
        }

        private void OnMouseDown(EventInformation<MouseEventArgs> obj)
        {
            var point = obj.EventArgs.GetPosition((IInputElement) obj.Sender);
            var normalizePoint = new Point(Convert.ToInt32(point.X*SourceImage.PixelWidth/ImageWidth),
                Convert.ToInt32(point.Y*SourceImage.PixelHeight/ImageHeight));
            Color sourceColor = SourceImage.GetColor((int) normalizePoint.X, (int) normalizePoint.Y);
            if (IsNonFillingColor(sourceColor))
            {
                //TODO: сообщаем о том, что выбранная область незакрашиваема
            }
            else
            {
                RegionSymbol = this.painter.FillImage(normalizePoint, SelectedColor.Color);
            }
            //Определяем позицию на изображении
            //Находим ближайший не черный пиксель
            //Запускаем закраску
        }

        private bool IsNonFillingColor(Color sourceColor)
        {
            return sourceColor.IsNearEqualTo(System.Windows.Media.Colors.White, 9) ||
                   sourceColor.IsNearEqualTo(System.Windows.Media.Colors.Black, 9) ||
                   sourceColor.IsNearEqualTo(System.Windows.Media.Colors.Transparent, 9) ||
                   sourceColor.IsNearEqualTo(Color.FromArgb(0,0,0,0), 9);
        }


        public double ImageWidth
        {
            get { return this.imageWidth; }
            set
            {
                this.imageWidth = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        public double ImageHeight
        {
            get { return this.imageHeight; }
            set
            {
                this.imageHeight = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }

        public BitmapSource SourceImage => this.sourceImage;

        public WriteableBitmap RegionSymbol
        {
            get { return this.regionSymbol; }
            set
            {
                this.regionSymbol = value;
                OnPropertyChanged(nameof(RegionSymbol));
            }
        }

        public ObservableCollection<ColorViewModel> Colors { get; } 

        public DelegateCommand<EventInformation<MouseEventArgs>> MouseDownCommand { get; }

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

        public int CalculateFillAccuracy()
        {
            return this.painter.CalculateFillAccuracy();
        }

        private ObservableCollection<ColorViewModel> GetColors()
        {
            return GetColors(Configuration.Colors.Any() ? Configuration.Colors : Constants.DefaultColors.ToList());
        }

        private ObservableCollection<ColorViewModel> GetColors(IList<FillingColor> fillingColors)
        {
            List<ColorViewModel> collection = new List<ColorViewModel>(fillingColors.Count);
            collection.AddRange(fillingColors.Select(LoadColor));
            return new ObservableCollection<ColorViewModel>(collection);
        }

        private ObservableCollection<ColorViewModel> LoadConfigurationColors()
        {
            List<ColorViewModel> collection = new List<ColorViewModel>(Configuration.Colors.Count);
            collection.AddRange(Configuration.Colors.Select(LoadColor));
            return new ObservableCollection<ColorViewModel>(collection);
        }

        private ColorViewModel LoadColor(FillingColor fillingColor)
        {
            var image = Utilities.GetImageFromFile(fillingColor.PathToImage);
            if (image.CanFreeze)
            {
                image.Freeze();
            }
            return new ColorViewModel(fillingColor.GetColor()) {Thumbnail = image};
        }

        //private ObservableCollection<ColorViewModel> LoadDefaultColors()
        //{
        //    List<ColorViewModel> collection = new List<ColorViewModel>(Constants.DefaultColors.Count);
        //    collection.AddRange(Constants.DefaultColors.Select(CreateViewModel));

        //    return new ObservableCollection<ColorViewModel>(collection);
        //}

        //private static ColorViewModel CreateViewModel(FillingColor fillingColor)
        //{
        //    BitmapSource thumbnail = Utilities.GetImageFromFile(Constants.DefaultBrushesDirectoryName + "\\"+fillingColor.ThumbnailName);
        //    if(thumbnail.CanFreeze)
        //    {
        //        thumbnail.Freeze();
        //    }
        //    return new ColorViewModel(fillingColor.Color) {Thumbnail = thumbnail};
        //}


        protected override void Next()
        {
            Configuration.Painter = this.painter;
            base.Next();
        }

        protected override void DoDispose()
        {
            foreach (var colorViewModel in Colors)
            {
                ((BitmapImage)colorViewModel.Thumbnail)?.StreamSource?.Close();
            }
            ((BitmapImage)this.painter.SourceImage)?.StreamSource?.Close();
            Colors.Clear();
            
        }
    }
}
