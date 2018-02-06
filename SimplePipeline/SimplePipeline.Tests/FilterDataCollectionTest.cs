using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterDataCollectionTest
    {
        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()), FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter()) });
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()), FilterData.Create(((Func<Double, Double>)Math.Round).ToFilter()) });
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()), FilterData.Create(((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter()), FilterData.Create(((Func<IEnumerable<IGrouping<Char, Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter()) });
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void AddFilterData(IEnumerable<FilterData> filterDatas)
        {
            FilterDataCollection filterDataCollection = new FilterDataCollection();
            foreach (FilterData filterData in filterDatas)
                if (filterDataCollection.Last == null || filterData.InputType.IsAssignableFrom(filterDataCollection.Last.OutputType))
                    Assert.DoesNotThrow(() => filterDataCollection.Add(filterData));
                else
                    Assert.Throws<ArgumentException>(() => filterDataCollection.Add(filterData));
        }
    }
}