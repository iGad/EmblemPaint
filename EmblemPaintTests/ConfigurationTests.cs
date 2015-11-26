using System;
using System.Collections.Generic;
using System.IO;
using EmblemPaint;
using EmblemPaint.Kernel;
using KernelTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmblemPaintTests
{
    [TestClass]
    public class ConfigurationTests
    {
        public static readonly string TestConfifurationName = "TestConfiguration.xml";

        private bool IsConfigurationExpected(Configuration configuration, int brushesCount, int regionsCount)
        {
            var defaultConfiguration = Configuration.DefaultConfiguration;
            return configuration.Colors.Count == brushesCount &&
                   configuration.Storage.Regions.Count == regionsCount &&
                   configuration.HorizontalItemsCount == defaultConfiguration.HorizontalItemsCount &&
                   configuration.VerticalItemsCount == defaultConfiguration.VerticalItemsCount &&
                   configuration.WaitAnswerInterval == defaultConfiguration.WaitAnswerInterval &&
                   configuration.WaitUserActionInterval == defaultConfiguration.WaitUserActionInterval &&
                   configuration.WindowHeight == defaultConfiguration.WindowHeight &&
                   configuration.WindowWidth == defaultConfiguration.WindowWidth;
        }

        [TestMethod]
        public void Generate_ByDefault_ReturnExpectedResult()
        {
            var configuration = Configuration.DefaultConfiguration;
            configuration = configuration.Generate(Environment.CurrentDirectory, TestHelper.TestRegionFolder, TestHelper.TestBrushFolder);

            Assert.IsTrue(IsConfigurationExpected(configuration, 8, 3));
        }

        [TestMethod]
        public void Save_Always_CreateConfigurationFile()
        {
            bool result;
            var configuration = Configuration.DefaultConfiguration;
            configuration.Colors = new List<FillingColor> {new FillingColor(), new FillingColor(), new FillingColor() };
            configuration.Storage.Regions.AddRange(new[] {new Region(), new Region()});
            var path = Path.Combine(TestHelper.TestDirectory, TestConfifurationName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                result = configuration.Save(stream);
            }

            Assert.IsTrue(result && File.Exists(path));
        }

        [TestMethod]
        public void Load_Always_ReturnSavedConfiguration()
        {
            var configuration = Configuration.DefaultConfiguration;
            configuration.Colors = new List<FillingColor> { new FillingColor(), new FillingColor(), new FillingColor() };
            configuration.Storage.Regions.AddRange(new[] { new Region(), new Region() });
            var path = Path.Combine(TestHelper.TestDirectory, TestConfifurationName);

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                configuration.Save(stream);
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                configuration = Configuration.Load(stream);
            }

            Assert.IsTrue(IsConfigurationExpected(configuration, 3, 2));
        }
    }
}
