using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using EmblemPaint.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KernelTests
{
    [TestClass]
    public class RegionsStorageTests
    {
        private List<string[]> CreateTestStrings()
        {
            var fileNames = new List<string[]>(9);
            for (int i = 1; i <= fileNames.Capacity; i++)
            {
                if (i%3 == 0)
                {
                    fileNames.Add(new[] {"region" + (i - 1)/3, "pattern"});
                    continue;
                }
                if (i%2 == 0)
                {
                    fileNames.Add(new[] {"region" + (i - 1)/3, "ideal"});
                    continue;
                }
                fileNames.Add(new[] {"region" + (i - 1)/3, "thumb"});
            }
            return fileNames;
        }

        private List<string[]> CreateTestWrongStrings()
        {
            var fileNames = new List<string[]>(6);
            for (int i = 1; i <= fileNames.Capacity; i++)
            {
                if (i % 3 == 0)
                {
                    fileNames.Add(new[] { "region" + (i-1) / 3, "pat" });
                    continue;
                }
                if (i % 2 == 0)
                {
                    fileNames.Add(new[] { "region" + (i - 1) / 3, "1234" });
                    continue;
                }
                fileNames.Add(new[] { "region" + (i - 1) / 3, "23" });
            }
            return fileNames;
        }

        private bool IsRegionsCorrect(IEnumerable<Region> regions)
        {
            return regions.All(region => !string.IsNullOrEmpty(region.ThumbnailImageName) &&
                                  !string.IsNullOrEmpty(region.PatternImageName) &&
                                  !string.IsNullOrEmpty(region.SourceImageName));
        }

        [TestMethod]
        public void FindRegion_WhenRegionExsts_ReturnExpectedRegion()
        {
            var storage = TestHelper.CreateFilledRegionsStorage(10);

            var result = storage.FindRegion("region5");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FindRegion_WhenRegionDoesNotExsts_ReturnNull()
        {
            var storage = TestHelper.CreateFilledRegionsStorage(10);

            var result = storage.FindRegion("5");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FillStorage_ByDefault_IncreaseRegionsCount()
        {
            var storage = TestHelper.CreateEmptyRegionsStorage();
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();

            RegionsStorage.FillStorage(storage, CreateTestStrings(), regexes);

            Assert.AreEqual(3, storage.Regions.Count);
        }

        [TestMethod]
        public void FillStorage_ByDefault_RegionsPropertiesAreCorrect()
        {
            var storage = TestHelper.CreateEmptyRegionsStorage();
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();

            RegionsStorage.FillStorage(storage, CreateTestStrings(), regexes);

            foreach (var region in storage.Regions)
            {
                Assert.IsTrue(Path.GetFileNameWithoutExtension(region.PatternImageName) == region.Name + "_pattern");
                Assert.IsTrue(Path.GetFileNameWithoutExtension(region.SourceImageName) == region.Name + "_ideal");
                Assert.IsTrue(Path.GetFileNameWithoutExtension(region.ThumbnailImageName) == region.Name + "_thumb");
            }

        }



        [TestMethod]
        public void FillStorage_WhenStringsNotMatchesToTheRegexes_DoesNotFillRegion()
        {
            var storage = TestHelper.CreateEmptyRegionsStorage();
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();

            RegionsStorage.FillStorage(storage, CreateTestWrongStrings(), regexes);

            foreach (var region in storage.Regions)
            {
                Assert.IsTrue(string.IsNullOrEmpty(region.PatternImageName));
                Assert.IsTrue(string.IsNullOrEmpty(region.SourceImageName));
                Assert.IsTrue(string.IsNullOrEmpty(region.ThumbnailImageName));
            }

        }

        [TestMethod]
        public void Generate_WhenTestFolder_Successfully()
        {
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();

            var storage = RegionsStorage.Generate(TestHelper.TestRegionFolder, regexes);

            Assert.IsTrue(storage.Regions.Count == 3 && IsRegionsCorrect(storage.Regions));
        }

        [TestMethod]
        public void Save_GeneretedStorage_Successfully()
        {
            if(File.Exists("TestStorage.xml"))
                File.Delete("TestStorage.xml");
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();
            var storage = RegionsStorage.Generate(TestHelper.TestRegionFolder, regexes);

            using (FileStream stream = new FileStream("TestStorage.xml", FileMode.Create, FileAccess.Write))
            {
                storage.Save(stream);
            }
                
            Assert.IsTrue(File.Exists("TestStorage.xml"));
        }

        [TestMethod]
        public void Save_WhenStreamIsInvalid_ExceptionExpected()
        {
            var storage = TestHelper.CreateEmptyRegionsStorage();

            using (var stream = new MemoryStream(new byte[0]))
            {
                TestHelper.Catch<NotSupportedException>(() => storage.Save(stream));
            }
        }

        [TestMethod]
        public void Load_SavedFile_Successfully()
        {
            RegionsStorage storage;
            using (var stream = new MemoryStream(new byte[] {0,1,2,3}))
            {
                TestHelper.Catch<InvalidOperationException>(() => storage = RegionsStorage.Load(stream));
            }
        }

        [TestMethod]
        public void Load_WhenStreamIsInvalid_ExceptionExpected()
        {
            if (File.Exists("TestStorage.xml"))
                File.Delete("TestStorage.xml");
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();
            var storage = RegionsStorage.Generate(TestHelper.TestRegionFolder, regexes);

            using (FileStream stream = new FileStream("TestStorage.xml", FileMode.Create, FileAccess.Write))
            {
                storage.Save(stream);
            }

            Assert.IsTrue(File.Exists("TestStorage.xml"));
        }

        [TestMethod]
        public void GetColorsForImage_ByDefault_ReturnExpectedValue()
        {
            var filePath = Path.Combine(TestHelper.TestRegionFolder, "region2_ideal.png");
            var expectedColorsCount = 4;

            var colors = RegionsStorage.GetColorsForImage(filePath);

            Assert.AreEqual(expectedColorsCount, colors.Count);
        }

        [TestMethod]
        public void GetColorsForImage_WhenFileNotExists_ExceptionExpected()
        {
            TestHelper.Catch<FileNotFoundException>(()=>RegionsStorage.GetColorsForImage("aaaaaasdaf.fsad"));
        }

        [TestMethod]
        public void AppendColors_WhenColorsFromConstants_Successfully()
        {
            var colors = Constants.ByDefaultColors.Take(5).Select(c => new FillingColor {HexArgbColor = c.ToHexString()}).ToList();

            Utilities.AppendColors(colors);

            Assert.AreEqual(7, colors.Count);
        }

        [TestMethod]
        public void AppendColors_WhenColorsFromConstants_AppendExpectedColors()
        {
            var colors = Constants.ByDefaultColors.Take(5).Select(c => new FillingColor { HexArgbColor = c.ToHexString() }).ToList();

            Utilities.AppendColors(colors);

            Assert.IsTrue(colors[5].Color.Equals(Constants.ByDefaultColors.ElementAt(5)) &&
                          colors[6].Color.Equals(Constants.ByDefaultColors.ElementAt(6)));
        }

        [TestMethod]
        public void GenerateColorsForImage_ByDefault_ReturnExpectedColors()
        {
            var filePath = Path.Combine(TestHelper.TestRegionFolder, "region2_ideal.png");

            var colors = RegionsStorage.GenerateColorsForImage(filePath);

            Assert.IsTrue(IsColorsExpected(colors));
        }

        private bool IsColorsExpected(List<FillingColor> colors)
        {
            var expectedColors = new []
            {
                Color.FromArgb(245, 110, 214, 97),
                Color.FromArgb(255, 0, 0, 252),
                Color.FromArgb(255, 238, 0, 252),
                Color.FromArgb(255, 252, 244, 0),
                Color.FromArgb(255, 255, 0, 0),
                Color.FromArgb(255, 0, 128, 0),
                Color.FromArgb(255, 255, 140, 0)
            };
            var colorList = colors.Select(color => color.Color);
            return colorList.SequenceEqual(expectedColors);
        }
    }
}
