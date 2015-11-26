using System.Collections.Generic;
using System.IO;
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
            Configuration configuration = LoadConfiguration();
            
            var windowDispatcher = new WindowDispatcher(configuration, CreateViewModels(configuration));
            var mainWindow = new MainWindow {DataContext = windowDispatcher};
            mainWindow.Show();
        }

        private Configuration LoadConfiguration()
        {
            if (File.Exists(Constants.DefaultConfigurationName))
            {
                using (var stream = new FileStream(Constants.DefaultConfigurationName, FileMode.Open))
                {
                    return Configuration.Load(stream);
                }
            }
            var config = Configuration.DefaultConfiguration.GenerateDefault();
            using (var stream = new FileStream(Constants.DefaultConfigurationName, FileMode.Create))
            {
                config.Save(stream);
            }
            return config;
        }

        private IList<FunctionalViewModel> CreateViewModels(Configuration configuration)
        {
            return new List<FunctionalViewModel>(6)
            {
                new ScreensaverViewModel(configuration),
                new SelectRegionViewModel(configuration),
                new PaintViewModel(configuration),
                new ResultViewModel(configuration),
                new SendEmailViewModel(configuration)
            };
        } 
    }
}
