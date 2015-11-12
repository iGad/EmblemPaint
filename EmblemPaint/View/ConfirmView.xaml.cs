using System.Timers;
using System.Windows;

namespace EmblemPaint.View
{
    /// <summary>
    /// Interaction logic for ConfirmView.xaml
    /// </summary>
    public partial class ConfirmView : Window
    {
        public ConfirmView()
        {
            InitializeComponent();
            Timer timer = new Timer(60 * 1000);
            timer.Elapsed += (o, e) =>
            Close();
            timer.Start();
        }
        

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
    }
}
