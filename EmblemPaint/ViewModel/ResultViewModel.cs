using System;
using System.IO;
using System.Media;
using System.Windows.Media.Imaging;
using EmblemPaint.Kernel;
using Microsoft.Practices.Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public class ResultViewModel:FunctionalViewModel
    {
        private int percents;

        public ResultViewModel(Configuration configuration):base(configuration)
        {
            if (configuration.Painter != null)
            {
                Update();
            }
            WindowLoadedCommand= new DelegateCommand(WindowLoaded);
        }


        #region Properties

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

        /// <summary>
        /// Команда реагирования на событие загрузки окна
        /// </summary>
        public DelegateCommand WindowLoadedCommand { get; }

        #endregion

        public override void Reconfigure(Configuration newConfig)
        {
            base.Reconfigure(newConfig);
            Update();
        }


        private void WindowLoaded()
        {
            string path = "Sounds/" + (this.percents > 50 ? "well_done_g.wav" : "try_again_g.wav");
            SoundPlayer sp = new SoundPlayer {SoundLocation = path};
            sp.Load();
            sp.Play();
        }

        private void Update()
        {
            ResultImage = Configuration.Painter.FilledImage;
            SourceImage = Configuration.Painter.SourceImage;
            Description = Configuration.SelectedRegion.Description;
            this.percents = Configuration.Painter.CalculateFillAccuracy();
            Result = this.percents + "%";
            FillingAngle = this.percents * Math.PI / 50;
            StartAngle = Math.PI + (2 * Math.PI - FillingAngle);
        }

        protected override void Home(bool? askUser)
        {
            Utilities.PlaySound("Sounds/Button.wav");
            base.Home(askUser);
        }
    }
}
