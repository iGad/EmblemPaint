using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EmblemPaint.Kernel;
using EmblemPaint.View;
using EmblemPaint.ViewModel;

namespace EmblemPaint
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegionsStorage storage = null;
            if (File.Exists(Constants.DefaultStorageName))
            {
                try
                {
                    using (var stream = new FileStream(Constants.DefaultStorageName, FileMode.Open))
                    {
                        storage = RegionsStorage.Load(stream);
                    }
                }
                catch(Exception)
                { }
            }
            if (storage == null)
            {
                try
                {
                    storage = RegionsStorage.GenerateStorageFromDefaultFolder(new RegionSuffixRegexes());
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удается загрузить данные. Проверьте наличие папки Content в каталоге с программой.");
                    Shutdown(1);
                    return;
                }
            }
            var windowDispatcher = new WindowDispatcher(storage);
            var mainWindow = new MainWindow();//View.MainWindow(storage);
            mainWindow.DataContext = windowDispatcher;
            mainWindow.Show();
        }
    }
}
