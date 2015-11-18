using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using EmblemPaint.Kernel;
using EmblemPaint.ViewModel;

namespace EmblemPaint.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       // private RegionsStorage storage;

        public MainWindow()//RegionsStorage storage
        {
            InitializeComponent();
           
        }

        public SelectRegionViewModel Model { get; }

        public void EventSetter_OnHandler(object sender, MouseEventArgs e)
        {
            
        }
        
    }
}
