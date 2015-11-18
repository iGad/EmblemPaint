using System;
using System.Windows.Media.Imaging;

namespace EmblemPaint.ViewModel
{
    public class ResultViewModel:FunctionalViewModel  
    {
        public ResultViewModel(BitmapSource resultImage, BitmapSource sourceImage, int resultPercents, Configuration configuration):base(configuration)
        {
            ResultImage = resultImage;
            SourceImage = sourceImage;
            Result = resultPercents + "%";
            FillingAngle = resultPercents*Math.PI/50;
            StartAngle = Math.PI + (2*Math.PI - FillingAngle);
        }

        /// <summary>
        /// Расскрашенное изображение
        /// </summary>
        public BitmapSource ResultImage { get; }

        /// <summary>
        /// Исходное изображение
        /// </summary>
        public BitmapSource SourceImage { get; }

        /// <summary>
        /// Процент совпадения
        /// </summary>
        public string Result { get; }

        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        public string Congratulation { get; }
        
        public double StartAngle { get; }

        public double FillingAngle { get; }

    }
}
