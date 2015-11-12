using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mvvm;

namespace EmblemPaint.Kernel
{
    public class Pie : BindableBase
    {
        private Point center, arcStartPoint, arcEndPoint;
        private double angle, startAngle, radius;
        private bool isAnimating = false;

        public Pie(double size, double startAngle, double angle)
        {
            Radius = size / 2;
            Angle = angle;
            StartAngle = startAngle;
            SetStartPoint();
            SetEndPoint();
            OnPropertyChanged(nameof(ArcSize));
        }

        public double TwoPi => 2 * Math.PI;

        public double Degree => TwoPi / 360;

        public Point Center
        {
            get { return this.center; }
            set
            {
                if (this.center != value)
                {

                    this.center = value;
                    OnPropertyChanged(() => Center);
                }
            }
        }

        public Point ArcStartPoint
        {
            get { return this.arcStartPoint; }
            set
            {
                if (this.arcStartPoint != value)
                {
                    this.arcStartPoint = value;
                    OnPropertyChanged(() => ArcStartPoint);
                }
            }
        }

        public Point ArcEndPoint
        {
            get { return this.arcEndPoint; }
            set
            {
                if (this.arcEndPoint != value)
                {
                    this.arcEndPoint = value;
                    OnPropertyChanged(() => ArcEndPoint);
                }
            }
        }

        public Size ArcSize => new Size(Radius, Radius);

        /// <summary>
        /// 
        /// </summary>
        public double Angle
        {
            get { return this.angle; }
            set
            {
                if (!this.angle.Equals(value))
                {
                    this.angle = value/180 * Math.PI;
                    SetEndPoint();
                    OnPropertyChanged(nameof(Angle));
                    OnPropertyChanged(nameof(IsLargeArc));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double StartAngle
        {
            get { return this.startAngle; }
            set
            {
                if (!this.startAngle.Equals(value))
                {
                    this.startAngle = value;
                    OnPropertyChanged(nameof(StartAngle));
                    SetStartPoint();
                    SetEndPoint();
                }
            }
        }

        public bool IsAnimating
        {
            get { return this.isAnimating; }
            set
            {
                this.isAnimating = value;
                OnPropertyChanged(nameof(IsAnimating));
            }
        }

        public double Radius
        {
            get { return this.radius; }
            set
            {
                if (!this.radius.Equals(value))
                {
                    this.radius = value;
                    Center = new Point(Radius, Radius);
                    OnPropertyChanged(nameof(Radius));
                }
            }
        }

        public bool IsLargeArc
        {
            get { return this.angle > Math.PI; }
            //set
            //{
            //    this.isLargeArc = value;
            //    OnPropertyChanged(()=>IsLargeArc);
            //}
        }

        private void SetStartPoint()
        {
            ArcStartPoint = new Point(Radius * Math.Sin(StartAngle) + Radius,
                        -Radius * Math.Cos(StartAngle) + Radius);
        }

        private void SetEndPoint()
        {
            ArcEndPoint = new Point(Radius * Math.Sin(StartAngle + Angle) + Radius,
                        -Radius * Math.Cos(StartAngle + Angle) + Radius);
        }

        private double GetAngle()
        {
            var a = Math.Pow(Distance(ArcStartPoint, Center), 2);
            var b = Math.Pow(Distance(ArcEndPoint, Center), 2);
            var c = Math.Pow(Distance(ArcStartPoint, ArcEndPoint), 2);
            //angle = ArcCos((a^2 + b^2 - c^2)/2ab)
            return Math.Acos((a * a + b * b - c * c) / (2 * a * b));
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }
}
