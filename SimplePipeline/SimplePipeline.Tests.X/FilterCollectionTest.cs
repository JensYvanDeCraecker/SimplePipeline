using System;
using System.Collections.Generic;
using SimplePipeline.Tests.Filters;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterCollectionTest
    {
        [Fact]
        public void CanCreatePipelineParametersNull()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.False(sequence.CanCreatePipeline(null, typeof(Object)));
            Assert.False(sequence.CanCreatePipeline(typeof(Object), null));
            Assert.False(sequence.CanCreatePipeline(null, null));
        }

        public static IEnumerable<Object[]> FilterDataCollections
        {
            get
            {
                yield return new Object[]{ new List<FilterData>()
                {
                    FilterData.Create(new CharEnumerableToStringFilter()),
                    FilterData.Create(new EnumerableCountFilter<Char>())
                }, true};
                yield return new Object[]{ new List<FilterData>()
                {
                    FilterData.Create(new EnumerableToArrayFilter<Char>()),
                    FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter())
                }, false};
            }
        }

        [Theory]
        [MemberData(nameof(FilterDataCollections))]
        public void CreateSequence(IEnumerable<FilterData> datas, Boolean shouldSucceed)
        {
            void Test()
            {
                FilterCollection sequence = new FilterCollection();
                foreach (FilterData data in datas)
                    sequence.Add(data);
            }
            if (shouldSucceed)
                Test();
            else
                Assert.Throws<InvalidFilterException>((Action)Test);
        }
    }
}