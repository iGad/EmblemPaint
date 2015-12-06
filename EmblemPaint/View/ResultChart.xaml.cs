using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private Geometry GetCircleGeometry(double startAngle, double arcAngle, double width, double height)
        {
            var circleR = Math.Min((width - BorderThickness.Left - BorderThickness.Right - 2*StrokeThikness)/2,
                (height - BorderThickness.Top - BorderThickness.Bottom - 2*StrokeThikness)/2);
            double xOffset = (width / 2 - circleR)/2;
            double yOffset = (height / 2 - circleR)/2;

            List<PathSegment> segments = new List<PathSegment>();
            segments.Add(new LineSegment(
                new Point(circleR*Math.Sin(startAngle) + circleR + xOffset, circleR + yOffset - circleR*Math.Cos(startAngle)), false));
            if (!startAngle.Equals(arcAngle))
            {
                var firstArcAngle = arcAngle > Math.PI ? Math.PI : arcAngle;
                segments.Add(new ArcSegment(new Point(circleR*Math.Sin(startAngle + firstArcAngle) + circleR + xOffset,
                    circleR + yOffset - circleR*Math.Cos(startAngle + firstArcAngle)),
                    new Size(circleR, circleR), 0,
                    false,
                    SweepDirection.Clockwise, true));
                if (arcAngle > Math.PI)
                {
                    segments.Add(new ArcSegment(new Point(circleR*Math.Sin(startAngle + arcAngle) + circleR + xOffset,
                        circleR + yOffset - circleR*Math.Cos(startAngle + arcAngle)),
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
        }

        private void Draw()
        {
            Path ellipsePath = new Path
            {
                Fill = new SolidColorBrush(Colors.White),
                StrokeThickness = StrokeThikness,
                Stroke = BorderBrush,
                Data = GetCircleGeometry(0, 2 * Math.PI, ActualWidth, ActualHeight)
            };
            Path filledEllipse = new Path
            {
                Fill = Foreground,
                StrokeThickness = StrokeThikness,
                Stroke = BorderBrush,
                Data = GetCircleGeometry(StartAngle, FillingAngle, ActualWidth, ActualHeight)
            };
            Path additionalEllipse = new Path
            {
                Fill = new SolidColorBrush(Color.FromArgb(224, 229,229,229)),
                StrokeThickness = 0,
                Data = GetCircleGeometry(0, 2*Math.PI, ActualWidth * 0.75, ActualHeight*0.75),
                HorizontalAlignment=HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            this.MainGrid.Children.Add(ellipsePath);
            this.MainGrid.Children.Add(filledEllipse);
            this.MainGrid.Children.Add(additionalEllipse);
        }
    }
}
