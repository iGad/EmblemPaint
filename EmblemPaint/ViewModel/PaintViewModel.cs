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

        public PaintViewModel(Region region)
        {
            MouseDownCommand = new DelegateCommand<EventInformation<MouseEventArgs>>(OnMouseDown);
            this.region = region;
            this.sourceImage = Utilities.GetImageFromFile(region.SourceImageName);
            var symbolImage = Utilities.GetImageFromFile(region.PatternImageName);
            this.regionSymbol = new WriteableBitmap(symbolImage);
            this.painter = new Painter(this.regionSymbol, this.sourceImage);
            Colors.First().IsSelected = true;
            StartTimer();
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

        public ObservableCollection<ColorViewModel> Colors { get; } = GetColors();

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

        private static ObservableCollection<ColorViewModel> GetColors()
        {
            List<ColorViewModel> collection = new List<ColorViewModel>(Constants.DefaultColors.Count);
            collection.AddRange(Constants.DefaultColors.Select(CreateViewModel));

            return new ObservableCollection<ColorViewModel>(collection);
        }

        private static ColorViewModel CreateViewModel(FillingColor fillingColor)
        {
            BitmapSource thumbnail = Utilities.GetImageFromFile(Constants.DefaultBrushesDirectoryName + "\\"+fillingColor.ThumbnailName);
            if(thumbnail.CanFreeze)
            {
                thumbnail.Freeze();
            }
            return new ColorViewModel(fillingColor.Color) {Thumbnail = thumbnail};
        }

        protected override void GoNext()
        {
            StopTimer();
            var percent = this.painter.CalculateMatchesPercent();
            var resultViewModel = new ResultViewModel(RegionSymbol, SourceImage, percent);
            var resultView = new View.ResultView(resultViewModel);
            resultView.Closed += ResultViewOnClosed;
            resultViewModel.HomeCommandExecuted += (o, e) => resultView.Close();
            resultView.ShowDialog();
        }

        private void ResultViewOnClosed(object sender, EventArgs eventArgs)
        {
            var resultView = sender as View.ResultView;
            if (resultView != null)
            {
                resultView.Closed -= ResultViewOnClosed;
                Home(false);
            }
        }

        protected override void Home(bool? askUser)
        {
            if (askUser.HasValue && askUser.Value)
            {
                if (AskUserAboutGoBack())
                {
                    StopTimer();
                    RaiseHomeCommandExecuted();
                }
                else
                {
                    ResetTimer();
                }
            }
            else
            {
                RaiseHomeCommandExecuted();
            }
        }

        private bool AskUserAboutGoBack()
        {
            View.ConfirmView confirmView = new View.ConfirmView();
            var dialogResult = confirmView.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
            {
                return true;
            }
            return false;
        }

        protected override void GoBack(object parameter)
        {
           
        }

        protected override void Close(Window window)
        {
            if (AskUserAboutGoBack())
            {
                StopTimer();
                window?.Close();
            }
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
