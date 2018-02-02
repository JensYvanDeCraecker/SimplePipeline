using System;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterTest
    {
        [Test]
        [TestCaseSource(typeof(FilterTestData), nameof(FilterTestData.TestCases))]
        public String CreateFilterAndExecute(Func<String, String> func, string input)
        {
            IFilter<String, String> filter = func.ToFilter();
            return filter.Execute(input);
        }
    }

}