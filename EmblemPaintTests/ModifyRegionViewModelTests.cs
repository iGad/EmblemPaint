using System;
using System.IO;
using System.Linq;
using EmblemPaint;
using EmblemPaint.Kernel;
using EmblemPaint.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmblemPaintTests
{
    [TestClass]
    public class ModifyRegionViewModelTests
    {
        private Configuration LoadConfiguration()
        {
            if (File.Exists(Constants.DefaultConfigurationName))
            {
                using (var stream = new FileStream(Constants.DefaultConfigurationName, FileMode.Open))
                {
                    return Configuration.Load(stream);
                }
            }
            return null;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var configuration = LoadConfiguration();
            var modifyViewModel = new ModifyRegionViewModel(configuration);
            int count = configuration.Storage.Regions.Count;
            configuration.SelectedRegion = configuration.Storage.Regions.First();
            modifyViewModel.Reconfigure(configuration);
            for (int i=0; i<count;i++)
            {
                if (File.Exists(configuration.SelectedRegion.SourceImageName))
                {
                    modifyViewModel.Reconfigure(configuration);
                    modifyViewModel.ReMakeImages();
                    modifyViewModel.NextCommand.Execute().Wait();
                }
                configuration.SelectedRegion = configuration.Storage.Regions[i];
            }
        }
    }
}
