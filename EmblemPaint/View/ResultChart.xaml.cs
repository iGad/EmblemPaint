using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace EmblemPaint.View
{
    /// <summary>
    /// Interaction logic for ResultChart.xaml
    /// </summary>
    public partial class ResultChart : UserControl
    {
        public ResultChart()
        {
            InitializeComponent();
            //StartAngle = 0;
            //FillingAngle = 0;
            //DataContext = this;
        }

        public static readonly DependencyProperty StrokeThiknessProperty = DependencyProperty.Register(nameof(StrokeThikness), typeof(double), typeof(ResultChart));

        public double StrokeThikness
        {
            get { return (double) GetValue(StrokeThiknessProperty); }
            set { SetValue(StrokeThiknessProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(nameof(StartAngle), typeof (double), typeof (ResultChart),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double StartAngle
        {
            get { return (double) GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty FillingAngleProperty = DependencyProperty.Register(nameof(FillingAngle), typeof(double), typeof(ResultChart),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double FillingAngle
        {
            get { return (double)GetValue(FillingAngleProperty); }
            set { SetValue(FillingAngleProperty, value); }
        }

        public Geometry Geometry
        {
            get; set;
        }

        private Geometry GetCircleGeometry(double startAngle, double arcAngle)
        {
            var circleR = Math.Min((ActualWidth - BorderThickness.Left - BorderThickness.Right - 2*StrokeThikness)/2,
                (ActualHeight - BorderThickness.Top - BorderThickness.Bottom - 2*StrokeThikness)/2);
            double xOffset = (ActualWidth/2 - circleR)/2;
            double yOffset = (ActualHeight/2 - circleR)/2;
            
            List<PathSegment> segments = new List<PathSegment>();
            segments.Add(new LineSegment(
                new Point(circleR * Math.Sin(startAngle) + circleR + xOffset, circleR + yOffset - circleR * Math.Cos(startAngle)), false));
            if (!startAngle.Equals(arcAngle))
            {
                var firstArcAngle = arcAngle > Math.PI ? Math.PI : arcAngle;
                segments.Add(new ArcSegment(new Point(circleR*Math.Sin(startAngle + firstArcAngle) + circleR + xOffset,
                    circleR + yOffset - circleR * Math.Cos(startAngle + firstArcAngle)),
                    new Size(circleR, circleR), 0,
                    false,
                    SweepDirection.Clockwise, true));
                if (arcAngle > Math.PI)
                {
                    segments.Add(new ArcSegment(new Point(circleR * Math.Sin(startAngle + arcAngle) + circleR + xOffset, 
                        circleR +yOffset - circleR * Math.Cos(startAngle + arcAngle)),
                    new Size(circleR, circleR), 0,
                    false,
                    SweepDirection.Clockwise, true));
                }
                
            }
            PathFigure figure = new PathFigure(new Point(circleR + xOffset, circleR + yOffset), segments, false);
            //figures.Add(new Line {  new Point(ActualWidth/2, BorderThickness.Top + StrokeThikness), true));
            return new PathGeometry(new[] {figure});

        }

      

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            //Path ellipsePath = new Path
            //{
            //    Fill = new SolidColorBrush(Colors.White),
            //    StrokeThickness = StrokeThikness,
            //    Stroke = BorderBrush,
            //    Data = GetCircleGeometry(0, 2*Math.PI)
            //};
            //Path filledEllipse = new Path
            //{
            //    Fill = Foreground,
            //    StrokeThickness = StrokeThikness,
            //    Stroke = BorderBrush,
            //    Data = GetCircleGeometry(StartAngle, FillingAngle)
            //};
            //this.MainGrid.Children.Add(ellipsePath);
            //this.MainGrid.Children.Add(filledEllipse);
            
        }

        private void Draw()
        {
            Path ellipsePath = new Path
            {
                Fill = new SolidColorBrush(Colors.White),
                StrokeThickness = StrokeThikness,
                Stroke = BorderBrush,
                Data = GetCircleGeometry(0, 2 * Math.PI)
            };
            Path filledEllipse = new Path
            {
                Fill = Foreground,
                StrokeThickness = StrokeThikness,
                Stroke = BorderBrush,
                Data = GetCircleGeometry(StartAngle, FillingAngle)
            };
            this.MainGrid.Children.Add(ellipsePath);
            this.MainGrid.Children.Add(filledEllipse);

        }
    }
}
