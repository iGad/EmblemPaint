using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EmblemPaint.Kernel;
using EmblemPaint.ViewModel;

namespace EmblemPaint.ImageGenerator.ViewModel
{
    public class ModifyRegionViewModel : PaintViewModel
    {
        private Region currentRegion;
        private Painter painter;

        public ModifyRegionViewModel(Configuration configuration) : base(configuration)
        {

            //this.currentRegion = configuration.SelectedRegion;
            //Colors = GetColors();
            //var image = Utilities.GetImageFromFile(this.currentRegion.ThumbnailImageName);
            //PatternImage = new WriteableBitmap(image);
            //this.painter = new Painter(PatternImage, image);
            //new ObservableCollection<ColorViewModel>(configuration.SelectedRegion.Colors.Select(c => new ColorViewModel(c.Color)).ToList());
        }
        
        

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

        protected override void OnMouseDown(Point point)
        {
            var normalizePoint = new Point(Convert.ToInt32(point.X * PatternImage.PixelWidth / ImageWidth),
               Convert.ToInt32(point.Y * PatternImage.PixelHeight / ImageHeight));
            Color sourceColor = PatternImage.GetColor((int)normalizePoint.X, (int)normalizePoint.Y);
            //byte[] pixel = { sourceColor.B, sourceColor.G, sourceColor.R, sourceColor.A };
            Colors.Add(LoadColor(new FillingColor(sourceColor)));
            PatternImage = this.painter.FillImage(normalizePoint, System.Windows.Media.Colors.White);

           
        }

        protected override void Update()
        {
            if (Configuration.SelectedRegion != null)
            {
                this.currentRegion = Configuration.SelectedRegion;
                Colors = GetColors();
                SourceImage = Utilities.GetImageFromFile(this.currentRegion.ThumbnailImageName);
                PatternImage = new WriteableBitmap(SourceImage.Clone());
                this.painter = new Painter(PatternImage, SourceImage.Clone());
            }
        }

        protected override void Next()
        {
            Configuration.SelectedRegion.Colors = Colors.Select(c => new FillingColor(c.Color)).ToList();
            if (!Directory.Exists("Content"))
            {
                Directory.CreateDirectory("Content");
            }
            using (FileStream fs = new FileStream(Path.Combine("Content", RegionName + "_pattern.png"), FileMode.Create, FileAccess.Write))
            {
                PatternImage.WriteTga(fs);
            }
            using (FileStream fs = new FileStream(Path.Combine("Content", RegionName + "_ideal.png"), FileMode.Create, FileAccess.Write))
            {
                var temp = new WriteableBitmap(SourceImage);
                temp.WriteTga(fs);
            }
            base.Next();
        }
    }
}
