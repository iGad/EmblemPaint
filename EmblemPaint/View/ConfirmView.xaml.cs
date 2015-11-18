using System.Timers;
using System.Windows;

namespace EmblemPaint.View
{
    /// <summary>
    /// Interaction logic for ConfirmView.xaml
    /// </summary>
    public partial class ConfirmView : Window
    {
        private Timer timer;
        public ConfirmView()
        {
            InitializeComponent();
            this.timer = new Timer(60 * 1000);
            this.timer.Elapsed += (o, e) => Dispatcher.Invoke(Close);
            this.timer.Start();
        }
        

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            DialogResult = true;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            DialogResult = false;
            Close();
        }
        
    }
}
