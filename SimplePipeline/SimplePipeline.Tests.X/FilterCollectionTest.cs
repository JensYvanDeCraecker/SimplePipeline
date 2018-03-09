using System;
using System.Collections.Generic;
using System.Linq;
using SimplePipeline.Tests.Filters;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterCollectionTest
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreateSequenceTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>())
                    }
                };
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter())
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(CreateSequenceTestData))]
        public void CreateSequenceTest(IEnumerable<FilterData> datas)
        {
            FilterCollection sequence = new FilterCollection();
            foreach (FilterData data in datas)
            {
                Int32 countBeforeAdd = sequence.Count;
                try
                {
                    sequence.Add(data);
                    Assert.Equal(countBeforeAdd + 1, sequence.Count);
                    Assert.True(Equals(sequence.Last(), data));
                }
                catch (InvalidFilterException)
                {
                    Assert.Equal(countBeforeAdd, sequence.Count);
                }
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> SequenceCanCreatePipelineTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    },
                    typeof(String), typeof(Int32), true
                };
            }
        }

        [Theory]
        [MemberData(nameof(SequenceCanCreatePipelineTestData))]
        public void SequenceCanCreatePipelineTest(FilterCollection sequence, Type pipelineInputType, Type pipelineOutputType, Boolean canCreate)
        {
            Assert.Equal(canCreate, sequence.CanCreatePipeline(pipelineInputType, pipelineOutputType));
        }

        [Fact]
        public void SequenceCanCreatePipelineParametersNull()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.False(sequence.CanCreatePipeline(null, typeof(Object)));
            Assert.False(sequence.CanCreatePipeline(typeof(Object), null));
            Assert.False(sequence.CanCreatePipeline(null, null));
        }
    }
}