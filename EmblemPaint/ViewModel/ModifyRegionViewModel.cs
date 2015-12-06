using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EmblemPaint.Kernel;
using Microsoft.Practices.Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public class ModifyRegionViewModel : PaintViewModel
    {
        private bool shiftMock = false;
        private Region currentRegion;
        private Painter patternImagePainter;
        private Painter sourceImagePainter;

        public ModifyRegionViewModel(Configuration configuration) : base(configuration)
        {
            ResetCommand = new DelegateCommand(Reset, CanExecuteResetCommand);
        }
        
        public List<FillingColor> RegionColors { get; set; }

        public string RegionName
        {
            get
            {
                return this.currentRegion.Name;
            }
            set
            {
                if (!this.currentRegion.Name.Equals(value))
                {
                    this.currentRegion.Name = value;
                    OnPropertyChanged(nameof(RegionName));
                }
            }
        }

        public string RegionDescription
        {
            get
            {
                return this.currentRegion.Description;
            }
            set
            {
                if (!this.currentRegion.Description.Equals(value))
                {
                    this.currentRegion.Description = value;
                    OnPropertyChanged(nameof(RegionDescription));
                }
            }
        }

        public WriteableBitmap ResultSourceImage { get; private set; }
        
        public DelegateCommand ResetCommand { get; private set; }

        protected override void OnMouseDown(Point point)
        {
            var normalizePoint = new Point(Convert.ToInt32(point.X * PatternImage.PixelWidth / ImageWidth),
               Convert.ToInt32(point.Y * PatternImage.PixelHeight / ImageHeight));
            Color sourceColor = PatternImage.GetColor((int)normalizePoint.X, (int)normalizePoint.Y);
            Color sourceFillingColor = sourceColor;
            if (SelectedColor != null && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || this.shiftMock))
            {
                sourceFillingColor = SelectedColor.Color;
            }
            else
            {
                var c = new ColorViewModel(sourceColor);
                if (UpColors.Count == 3)
                {
                    DownColors.Add((c));
                }
                else
                {
                    UpColors.Add(c);
                }
                Colors.Add(c);
            }
            ResultSourceImage = this.sourceImagePainter.FillImage(normalizePoint, sourceFillingColor, 40.0);
            PatternImage = this.patternImagePainter.FillImage(normalizePoint, System.Windows.Media.Colors.White, 40.0);
            ResetCommand.RaiseCanExecuteChanged();
        }

        protected override void Update()
        {
            if (Configuration.SelectedRegion != null)
            {
                this.currentRegion = Configuration.SelectedRegion;
                RegionColors = this.currentRegion.Colors;
                if (Colors == null)
                {
                    Colors = new ObservableCollection<ColorViewModel>();
                    UpColors = new ObservableCollection<ColorViewModel>();
                    DownColors = new ObservableCollection<ColorViewModel>();
                }
                else
                {
                    Colors.Clear();
                    UpColors.Clear();
                    DownColors.Clear();
                }//GetColors(this.currentRegion);
                SourceImage = Utilities.GetImageFromFile(this.currentRegion.SourceImageName);
                ResultSourceImage = new WriteableBitmap(SourceImage);
                OnPropertyChanged(nameof(SourceImage));
                PatternImage = new WriteableBitmap(SourceImage.Clone());
                OnPropertyChanged(nameof(PatternImage));
                OnPropertyChanged(nameof(RegionName));
                OnPropertyChanged(nameof(RegionDescription));
                this.patternImagePainter = new Painter(PatternImage, SourceImage);
                this.sourceImagePainter = new Painter(ResultSourceImage, SourceImage);
            }
        }

        protected override void Next()
        {
            Configuration.SelectedRegion.Colors = Colors.Select(c => new FillingColor(c.Color)).ToList();
            if (!Directory.Exists("Content2"))
            {
                Directory.CreateDirectory("Content2");
            }
            string path = Path.Combine("Content", RegionName + "_pattern.png");
            Configuration.SelectedRegion.PatternImageName = path;
            Configuration.SelectedRegion.ThumbnailImageName = Path.Combine("Content", RegionName + "_thumb.png");
            using (FileStream fs = new FileStream(Path.Combine("Content2", RegionName + "_pattern.png"), FileMode.Create, FileAccess.Write))
            {
                Utilities.SaveImage(fs, PatternImage);
            }
            path = Path.Combine("Content", RegionName + "_ideal.png");
            Configuration.SelectedRegion.SourceImageName = path;
            using (FileStream fs = new FileStream(Path.Combine("Content2", RegionName + "_ideal.png"), FileMode.Create, FileAccess.Write))
            {
                Utilities.SaveImage(fs, ResultSourceImage);
            }
            //Configuration.SelectedRegion.ThumbnailImageName = null;
            Utilities.AppendColors(Configuration.SelectedRegion.Colors);
            using (FileStream fs = new FileStream("ModifiedConfiguration.xml", FileMode.Create, FileAccess.Write))
            {
                Configuration.Save(fs);
            }

            int index = Configuration.Storage.Regions.IndexOf(Configuration.SelectedRegion);
            if (index < Configuration.Storage.Regions.Count - 1)
            {
                Configuration.SelectedRegion = Configuration.Storage.Regions[index + 1];
                Update();
            }
            //base.Next();
        }

        private void Reset()
        {
            Update();
            //PatternImage = new WriteableBitmap(SourceImage);
            //ResultSourceImage = new WriteableBitmap(SourceImage);
            //Colors?.Clear();
            //UpColors?.Clear();
            //DownColors?.Clear();
            ResetCommand.RaiseCanExecuteChanged();
        }

        internal void ReMakeImages()
        {
            ImageHeight = PatternImage.PixelHeight;
            ImageWidth = PatternImage.PixelWidth;
            Colors = new ObservableCollection<ColorViewModel>(RegionColors.Select(fc => new ColorViewModel(fc.Color)));
            var cls = RegionColors.Select(c => c.Color).ToList();
            for (int i = 0; i < PatternImage.PixelHeight; i += 2)
            {
                for (int j = 0; j < PatternImage.PixelWidth; j+=2)
                {
                    var color = PatternImage.GetColor(j, i);
                    if (color != System.Windows.Media.Colors.White && color.A > 10)
                    {
                        var col = Colors.FirstOrDefault(c => c.Color.IsNearEqualTo(color, 1));
                        if (col != null)
                        {
                            SelectedUpColor = col;
                            this.shiftMock = true;
                            OnMouseDown(new Point(j, i));
                        }
                    }
                }
            }

        }

        private bool CanExecuteResetCommand()
        {
            return Colors != null && Colors.Any();
        }
        
    }
}
