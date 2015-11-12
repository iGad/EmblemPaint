using System.Windows.Media.Imaging;

namespace EmblemPaint.ViewModel
{
    public class ResultViewModel:FunctionalViewModel  
    {
        public ResultViewModel(BitmapSource resultImage, BitmapSource sourceImage, int resultPercents)
        {
            ResultImage = resultImage;
            SourceImage = sourceImage;
            Result = resultPercents + "%";
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

        #region Overrides

        //protected override void Home(bool? askUser)
        //{
        //    var window = parameter as Window;
        //    window?.Close();
        //}

        #endregion
    }
}
