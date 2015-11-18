using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace EmblemPaint.Kernel
{
    public static class Constants
    {
        public const int BytesPerPixel = 4;
        public const double DefaultRegionSize = 100.0;
        public const string DefaultStorageDirectoryName = "Content";
        public const string DefaultBrushesDirectoryName = "Brushes";
        public const string DefaultConfigurationName = "Configuration.xml";
        public static readonly Color HighlightColor = Color.FromRgb(255, 249, 196);
        public static readonly Color BorderColor = Color.FromRgb(186, 186, 186);
        public static readonly Color DarkBackColor = Color.FromRgb(179, 179, 179);
        public static readonly Color DarkBackBorderColor = Color.FromRgb(126, 126, 126);
        public static readonly IReadOnlyCollection<FillingColor> DefaultColors = 
            new ReadOnlyCollection<FillingColor>(new List<FillingColor>
            {
                new FillingColor("Red","RedBrush","RedBrush.png"),
                new FillingColor("Green", "GreenBrush", "GreenBrush.png"),
                new FillingColor("Blue", "BlueBrush", "BlueBrush.png"),
                new FillingColor("DarkOrange", "DarkOrangeBrush", "DarkOrangeBrush.png"),
                new FillingColor("Yellow", "YellowBrush", "YellowBrush.png"),
                new FillingColor("Aqua", "AquaBrush", "AquaBrush.png"),
                new FillingColor("Chartreuse", "ChartreuseBrush", "ChartreuseBrush.png")
            }); 
    }
}
