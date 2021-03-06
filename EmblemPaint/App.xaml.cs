﻿using System.Collections.Generic;
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
            //Window1 w = new Window1();
            //w.Show();
            bool exit = false;
            while (!exit)
            {
                Configuration configuration = LoadConfiguration();
                if (configuration.ModifyMode)
                {
                    PrepareRegions(configuration);
                }
           
                var windowDispatcher = new WindowDispatcher(configuration, CreateViewModels(configuration));
                try
                {
                    var mainWindow = new MainWindow { DataContext = windowDispatcher };
                    mainWindow.ShowDialog();
                    exit = true;
                }
                catch
                {
                    windowDispatcher.Dispose();
                }
            }
        }

        private void PrepareRegions(Configuration configuration)
        {
            foreach (var region in configuration.Storage.Regions)
            {
                region.ThumbnailImageName = region.SourceImageName;
            }
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
            if (configuration.ModifyMode)
            {

                var list = new List<FunctionalViewModel>(3)
                {
                    new SelectRegionViewModel(configuration),
                    new ModifyRegionViewModel(configuration)
                };
                if (!configuration.UseConfigFile)
                {
                    list.Insert(0, new SelectFolderViewModel(configuration));
                }
                else
                {
                    list[0].Reconfigure(configuration);
                }
                return list;
            }
            else
            {
                return new List<FunctionalViewModel>(6)
                {
                    new ScreensaverViewModel(configuration),
                    new SelectRegionViewModel(configuration),
                    new PaintViewModel(configuration),
                    new ResultViewModel(configuration),
                    new SendEmailViewModel(configuration),
                    new SendCompleteViewModel(configuration)
                };
            }
            
            
        }
    }
}
