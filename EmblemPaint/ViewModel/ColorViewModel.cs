using System.Windows.Media;
using EmblemPaint.Kernel;

namespace EmblemPaint.ViewModel
{
    public class ColorViewModel : ListViewItemModel
    {
        public ColorViewModel(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }
}
