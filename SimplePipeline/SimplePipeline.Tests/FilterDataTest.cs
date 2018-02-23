using System;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterDataTest
    {
        //private readonly MethodInfo processFilterDataEqualityDefinition = typeof(FilterDataTest).GetMethod("ProcessFilterDataEquality", BindingFlags.NonPublic | BindingFlags.Instance);

        //// ReSharper disable once UnusedMember.Local
        //private void ProcessFilterDataEquality<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        //{
        //    Assert.AreEqual(FilterData.Create(filter), FilterData.Create(filter));
        //}

        //[Test]
        //[TestCaseSource(typeof(TestData), nameof(TestData.CompareFilterTypeData))]
        //public void CompareFilterType(FilterData data)
        //{
        //    Type expectedFilterType = typeof(IFilter<,>).MakeGenericType(data.InputType, data.OutputType);
        //    Assert.AreEqual(expectedFilterType, data.FilterType);
        //    Assert.IsTrue(expectedFilterType.IsInstanceOfType(data.Filter));
        //}

        //[Test]
        //[TestCaseSource(typeof(TestData), nameof(TestData.FilterDataEqualityData))]
        //public void FilterDataEquality(Object filter, Type filterInputType, Type filterOutputType)
        //{
        //    processFilterDataEqualityDefinition.MakeGenericMethod(filterInputType, filterOutputType).Invoke(this, new[] { filter });
        //}

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.FilterDataEqualsTrueData))]
        public void FilterDataEqualsTrue(FilterData firstFilterData, FilterData secondFilterData)
        {
            Assert.IsTrue(Equals(firstFilterData, secondFilterData));
        }

        public void FilterDataEqualsFalse(FilterData firstFilterData, FilterData secondFilterData)
        {
            Assert.IsFalse(Equals(firstFilterData, secondFilterData));
        }
    }
}