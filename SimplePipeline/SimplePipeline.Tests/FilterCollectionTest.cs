using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterCollectionTest
    {
        private readonly MethodInfo addFilterDefenition = typeof(FilterCollectionTest).GetMethod("AddFilter");

        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, String>)(input => input.ToUpper())).ToFilter()), FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()), FilterData.Create(((Func<String, String>)(input => input.Substring(0, 4))).ToFilter()) }, true);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, IEnumerable<Char>>)(input => input.ToCharArray())).ToFilter()), FilterData.Create(((Func<Char[], Int32>)(input => input.Length)).ToFilter()) }, false);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter()), FilterData.Create(((Func<IEnumerable<Char>, Int32>)(input => input.Count())).ToFilter()) }, true);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter()), FilterData.Create(((Func<Double, Int32>)(input => (Int32)input)).ToFilter()) }, false);
            }
        }

        public void AddFilter<TInput, TOutput>(FilterCollection collection, IFilter<TInput, TOutput> filter)
        {
            collection.Add(filter);
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void BeginTest(IEnumerable<FilterData> filterDatas, Boolean canAdd)
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