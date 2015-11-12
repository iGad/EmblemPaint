using System.ComponentModel;
using System.Windows;
using EmblemPaint.ViewModel;

namespace EmblemPaint.View
{
    /// <summary>
    /// Interaction logic for PaintViewOld.xaml
    /// </summary>
    public partial class PaintViewOld : Window
    {
        public PaintViewOld(PaintViewModel paintViewModel)
        {
            InitializeComponent();
            DataContext = paintViewModel;
        }
        
        private void PaintView_OnClosing(object sender, CancelEventArgs e)
        {
            ((PaintViewModel) DataContext)?.Dispose();
            DataContext = null;
        }
    }
}
