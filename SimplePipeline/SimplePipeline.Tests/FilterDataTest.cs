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

        [Test]
        [TestCaseSource(nameof(ValidateFilterData))]
        public void ValidateFilter(FilterData data)
        {
            Assert.IsInstanceOf(typeof(IFilter<,>).MakeGenericType(data.InputType, data.OutputType), data.Filter);
        }
    }
}