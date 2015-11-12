using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Assert.IsTrue(region.PatternImageName == region.Name + "_pattern");
                Assert.IsTrue(region.SourceImageName == region.Name + "_ideal");
                Assert.IsTrue(region.ThumbnailImageName == region.Name + "_thumb");
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

            var storage = RegionsStorage.Generate("TestFiles", regexes);

            Assert.IsTrue(storage.Regions.Count == 3 && IsRegionsCorrect(storage.Regions));
        }

        [TestMethod]
        public void Save_GeneretedStorage_Successfully()
        {
            if(File.Exists("TestStorage.xml"))
                File.Delete("TestStorage.xml");
            var regexes = TestHelper.CreateTestsRegionSuffixRegexes();
            var storage = RegionsStorage.Generate("TestFiles", regexes);

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
            var storage = RegionsStorage.Generate("TestFiles", regexes);

            using (FileStream stream = new FileStream("TestStorage.xml", FileMode.Create, FileAccess.Write))
            {
                storage.Save(stream);
            }

            Assert.IsTrue(File.Exists("TestStorage.xml"));
        }

    }
}
