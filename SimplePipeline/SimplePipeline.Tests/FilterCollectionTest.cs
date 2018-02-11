using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterCollectionTest
    {
        private readonly MethodInfo addFilterDefenition = typeof(FilterCollectionTest).GetMethod("AddFilter");

        public void AddFilter<TInput, TOutput>(FilterCollection collection, IFilter<TInput, TOutput> filter)
        {
            collection.Add(filter);
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.BuildCollectionData))]
        public void BuildCollection(IEnumerable<FilterData> filterDatas, Boolean canAdd)
        {
            FilterCollection collection = new FilterCollection();
            if (canAdd)
                Assert.DoesNotThrow(() =>
                {
                    foreach (FilterData filterData in filterDatas)
                        addFilterDefenition.MakeGenericMethod(filterData.InputType, filterData.OutputType).Invoke(this, new[] { collection, filterData.Filter });
                });
            else
            {
                TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() =>
                {
                    foreach (FilterData filterData in filterDatas)
                        addFilterDefenition.MakeGenericMethod(filterData.InputType, filterData.OutputType).Invoke(this, new[] { collection, filterData.Filter });
                });
                Assert.AreEqual(exception.InnerException?.GetType(), typeof(ArgumentException));
            }
        }
    }
}