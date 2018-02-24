using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterDataTest
    {
        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.FilterDataEqualsTrueData))]
        public void FilterDataEqualsTrue(FilterData firstFilterData, FilterData secondFilterData)
        {
            Assert.IsTrue(Equals(firstFilterData, secondFilterData));
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.FilterDataEqualsFalseData))]
        public void FilterDataEqualsFalse(FilterData firstFilterData, FilterData secondFilterData)
        {
            Assert.IsFalse(Equals(firstFilterData, secondFilterData));
        }
    }
}