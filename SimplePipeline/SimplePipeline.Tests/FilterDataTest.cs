using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterDataTest
    {
        public static IEnumerable<TestCaseData> ValidateFilterData
        {
            get
            {
                yield return new TestCaseData(FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<Double, Double>)Math.Round).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<IEnumerable<IGrouping<Char, Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter()));
            }
        }

        public static IEnumerable<TestCaseData> FilterDataEqualityData
        {
            get
            {
                IFilter<String, String> firstFilter = ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter();
                yield return new TestCaseData(FilterData.Create(firstFilter), FilterData.Create(firstFilter));
                IFilter<String, Int32> secondFilter = ((Func<String, Int32>)(input => input.Length)).ToFilter();
                yield return new TestCaseData(FilterData.Create(secondFilter), FilterData.Create(secondFilter));
                IFilter<Double, Double> thirdFilter = ((Func<Double, Double>)Math.Round).ToFilter();
                yield return new TestCaseData(FilterData.Create(thirdFilter), FilterData.Create(thirdFilter));
                IFilter<String, IEnumerable<IGrouping<Char, Char>>> fourthFilter = ((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter();
                yield return new TestCaseData(FilterData.Create(fourthFilter), FilterData.Create(fourthFilter));
                IFilter<IEnumerable<IGrouping<Char, Char>>, Int32> fifthFilter = ((Func<IEnumerable<IGrouping<Char, Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter();
                yield return new TestCaseData(FilterData.Create(fifthFilter), FilterData.Create(fifthFilter));
            }
        }

        [Test]
        [TestCaseSource(nameof(ValidateFilterData))]
        public void ValidateFilter(FilterData data)
        {
            Assert.IsInstanceOf(typeof(IFilter<,>).MakeGenericType(data.InputType, data.OutputType), data.Filter);
        }

        [Test]
        [TestCaseSource(nameof(FilterDataEqualityData))]
        public void FilterDataEquality(FilterData first, FilterData second)
        {
            Assert.AreEqual(first, second);
        }
    }
}