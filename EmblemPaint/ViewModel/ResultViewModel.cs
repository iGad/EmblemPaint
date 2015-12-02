using System;
using System.Windows.Media.Imaging;
using EmblemPaint.Kernel;

namespace EmblemPaint.ViewModel
{
    public class ResultViewModel:FunctionalViewModel  
    {
        public ResultViewModel(Configuration configuration):base(configuration)
        {
            if (configuration.Painter != null)
            {
                Update();
            }
        }

        /// <summary>
        /// Расскрашенное изображение
        /// </summary>
        public BitmapSource ResultImage { get; private set; }

        /// <summary>
        /// Исходное изображение
        /// </summary>
        public BitmapSource SourceImage { get; private set; }

        /// <summary>
        /// Процент совпадения
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// Описание герба
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// Начальный угол индикатора
        /// </summary>
        public double StartAngle { get; private set; }

        /// <summary>
        /// Заполненный угол индикатора
        /// </summary>
        public double FillingAngle { get; private set; }
        

        public override void Reconfigure(Configuration newConfig)
        {
            base.Reconfigure(newConfig);
            Update();
        }

        private void Update()
        {
            ResultImage = Configuration.Painter.FilledImage;
            SourceImage = Configuration.Painter.SourceImage;
            Description = Configuration.SelectedRegion.Description;
            var persents = Configuration.Painter.CalculateFillAccuracy();
            Result = persents + "%";
            FillingAngle = persents * Math.PI / 50;
            StartAngle = Math.PI + (2 * Math.PI - FillingAngle);
        }
    }
}
