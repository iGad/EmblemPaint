﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmblemPaint.Kernel
{
    public static class Constants
    {
        public const int BytesPerPixel = 4;
        public const double DefaultRegionSize = 100.0;
        public const string DefaultStorageDirectoryName = "Content";
        public const string DefaultBrushesDirectoryName = "Brushes";
        public const string DefaultStorageName = "Storage.xml";
        public static readonly Color HighlightColor = Color.FromRgb(255, 249, 196);
        public static readonly Color BorderColor = Color.FromRgb(186, 186, 186);
        public static readonly Color DarkBackColor = Color.FromRgb(179, 179, 179);
        public static readonly Color DarkBackBorderColor = Color.FromRgb(126, 126, 126);
        public static readonly IReadOnlyCollection<FillingColor> DefaultColors = 
            new ReadOnlyCollection<FillingColor>(new List<FillingColor>
            {
                new FillingColor("RedBrush.png", Colors.Red),
                new FillingColor("GreenBrush.png",Colors.Green),
                new FillingColor("BlueBrush.png", Colors.Blue),
                new FillingColor("DarkOrangeBrush.png", Colors.DarkOrange),
                new FillingColor("YellowBrush.png", Colors.Yellow),
                new FillingColor("AquaBrush.png", Colors.Aqua),
                new FillingColor("ChartreuseBrush.png", Colors.Chartreuse)
            }); 
    }
}