using System;
using EmblemPaint.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KernelTests
{
    public static class TestHelper
    {
        public static string TestRegionFolder => "TestFiles";

        public static string TestBrushFolder => "TestBrushes";

        public static string TestDirectory => Environment.CurrentDirectory;

        public static RegionSuffixRegexes CreateTestsRegionSuffixRegexes()
        {
            return new RegionSuffixRegexes('_', "thumb", "ideal", "pattern");
        }

        public static RegionsStorage CreateEmptyRegionsStorage()
        {
            return new RegionsStorage();
        }

        public static RegionsStorage CreateFilledRegionsStorage(int count)
        {
            var storage = CreateEmptyRegionsStorage();
            for (int i = 0; i < count; i++)
            {
                storage.Regions.Add(new Region("region" + i));
            }
            return storage;
        }

        public static void Catch<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            Assert.Fail("Expected exception was not thrown");
        }
    }
}
