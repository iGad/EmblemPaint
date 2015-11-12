using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EmblemPaint.Kernel
{
    /// <summary>
    /// Interaction logic for PieView.xaml
    /// </summary>
    public partial class PieView : UserControl
    {
        public PieView()
        {
            BackBrush = new SolidColorBrush(Constants.HighlightColor);
            FillBrush = new SolidColorBrush(Constants.DarkBackBorderColor);
            InitializeComponent();
        }
        
        /// <summary>
        /// Свойство зависимости для FillBrush
        /// </summary>
        public static DependencyProperty FillBrushProperty = DependencyProperty.Register(nameof(FillBrushProperty), typeof(Brush), typeof(PieView));

        /// <summary>
        /// Кисть, которой заполняется круг
        /// </summary>
        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для FillBrush
        /// </summary>
        public static DependencyProperty BackBrushProperty = DependencyProperty.Register(nameof(BackBrushProperty), typeof(Brush), typeof(PieView));

        /// <summary>
        /// Кисть, которой заполняется задний план круга
        /// </summary>
        public Brush BackBrush
        {
            get { return (Brush)GetValue(BackBrushProperty); }
            set { SetValue(BackBrushProperty, value); }
        }

        public void StopAnimation()
        {
            this.AnimationTimeline.Stop();
        }

        /// <summary>
        /// Событие завершения анимации
        /// </summary>
        public event EventHandler TimeLineCompleted;

        private void Pie_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.BackEllipse.Fill = BackBrush;
            this.MainPath.Fill = FillBrush;
        }



        private void Timeline_OnCompleted(object sender, EventArgs e)
        {
            var handle = TimeLineCompleted;
            handle?.Invoke(this, e);
        }
    }
}
