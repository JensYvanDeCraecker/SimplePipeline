using System;
using System.Collections.Generic;
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
                    new List<Tuple<FilterData, Boolean>>()
                    {
                        Tuple.Create(FilterData.Create(new CharEnumerableToStringFilter()), true),
                        Tuple.Create(FilterData.Create(new EnumerableCountFilter<Char>()), true),
                        Tuple.Create(FilterData.Create(new ObjectToStringFilter()), true)
                    }
                };
                yield return new Object[]
                {
                    new List<Tuple<FilterData, Boolean>>()
                    {
                        Tuple.Create(FilterData.Create(new EnumerableToArrayFilter<Char>()), true),
                        Tuple.Create(FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter()), false),
                        Tuple.Create(FilterData.Create(new CharEnumerableToStringFilter()), true)
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(CreateSequenceTestData))]
        public void CreateSequenceTest(IEnumerable<Tuple<FilterData, Boolean>> tuples)
        {
            FilterCollection sequence = new FilterCollection();
            foreach (Tuple<FilterData, Boolean> tuple in tuples)
            {
                FilterData data = tuple.Item1;
                Boolean shouldSucceed = tuple.Item2;
                Int32 countBeforeAdd = sequence.Count;
                if (shouldSucceed)
                {
                    sequence.Add(data);
                    Assert.Equal(countBeforeAdd + 1, sequence.Count);
                }
                else
                {
                    Assert.Throws<InvalidFilterException>(() => sequence.Add(data));
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
                yield return new Object[]
                {
                    new FilterCollection(new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>())
                    }),
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