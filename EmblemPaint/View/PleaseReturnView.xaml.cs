using System.Windows;

namespace EmblemPaint.View
{
    /// <summary>
    /// Interaction logic for PleaseReturnView.xaml
    /// </summary>
    public partial class PleaseReturnView : Window
    {
        public PleaseReturnView()
        {
            InitializeComponent();
            this.AnimatedCircle.TimeLineCompleted += AnimatedCircleTimeLineCompleted;
        }

        public new bool? DialogResult { get; private set; }

        private void AnimatedCircleTimeLineCompleted(object sender, System.EventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ReturnButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.AnimatedCircle.StopAnimation();
            Close();
        }

    }
}
