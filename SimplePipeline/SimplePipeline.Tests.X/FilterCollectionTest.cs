using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SimplePipeline.Tests.Filters;
using SimplePipeline.Tests.Pipelines;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterCollectionTest
    {
        private readonly MethodInfo sequenceAddFilterDefinition = typeof(FilterCollectionTest).GetMethod("SequenceAddFilter", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo sequenceAddPipelineDefinition = typeof(FilterCollectionTest).GetMethod("SequenceAddPipeline", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo sequenceAddFunctionDefinition = typeof(FilterCollectionTest).GetMethod("SequenceAddFunction", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo sequenceAddFilterData = typeof(FilterCollectionTest).GetMethod("SequenceAddFilterData", BindingFlags.NonPublic | BindingFlags.Static);

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreateSequenceFilledTestData
        {
            get
            {
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>()),
                        FilterData.Create(((Func<Object, String>)(input => input.ToString())).ToFilter()),
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>()),
                        FilterData.Create(((Func<Object, String>)(input => input.ToString())).ToFilter()),
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>()),
                        FilterData.Create(((Func<Object, String>)(input => input.ToString())).ToFilter()),
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>()),
                        FilterData.Create(((Func<Object, String>)(input => input.ToString())).ToFilter())
                    },
                    true
                };
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>()),
                        FilterData.Create(((Func<Object, String>)(input => input.ToString())).ToFilter()),
                        FilterData.Create(new EnumerableToArrayFilter<String>())
                    },
                    false
                };
            }
        }

        [Theory]
        [MemberData(nameof(CreateSequenceFilledTestData))]
        [AssertionMethod]
        public void CreateSequenceFilledTest(IEnumerable<FilterData> filterDatas, Boolean shouldSucceed)
        {
            filterDatas = filterDatas.ToList();

            FilterCollection CreateSequence()
            {
                return new FilterCollection(filterDatas);
            }

            if (shouldSucceed)
            {
                FilterCollection sequence = CreateSequence();
                Assert.Equal(filterDatas.First(), sequence.FirstFilter);
                Assert.Equal(filterDatas.Last(), sequence.LastFilter);
                Assert.Equal(filterDatas.Count(), sequence.Count);
            }
            else
                Assert.Throws<ArgumentException>(() => CreateSequence());
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreateSequenceTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new List<Tuple<Object, ItemType, Boolean, Type, Type>>()
                    {
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(FilterData.Create(new CharEnumerableToStringFilter()), ItemType.FilterData, true, typeof(IEnumerable<Char>), typeof(String)),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(new EnumerableCountFilter<Char>(), ItemType.Filter, true, typeof(IEnumerable<Char>), typeof(Int32)),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>((Func<Object, String>)(input => input.ToString()), ItemType.Function, true, typeof(Object), typeof(String)),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(FilterData.Create(new EnumerableToArrayFilter<String>()), ItemType.FilterData, false, typeof(IEnumerable<String>), typeof(String[])),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(new EnumerableToArrayPipeline<String>(), ItemType.Pipeline, false, typeof(IEnumerable<String>), typeof(String[]))
                    }
                };
                yield return new Object[]
                {
                    new List<Tuple<Object, ItemType, Boolean, Type, Type>>()
                    {
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(FilterData.Create(new EnumerableToArrayFilter<Char>()), ItemType.FilterData, true, typeof(IEnumerable<Char>), typeof(Char[])),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>((Func<String, Int32>)(input => input.Length), ItemType.Function, false, typeof(String), typeof(Int32)),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(new CharEnumerableToStringFilter(), ItemType.Filter, true, typeof(IEnumerable<Char>), typeof(String)),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(new EnumerableToArrayPipeline<Char>(), ItemType.Pipeline, true, typeof(IEnumerable<Char>), typeof(Char[])),
                        Tuple.Create<Object, ItemType, Boolean, Type, Type>(new EnumerableToArrayFilter<String>(), ItemType.Filter, false, typeof(IEnumerable<String>), typeof(String[]))
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(CreateSequenceTestData))]
        [AssertionMethod]
        public void CreateSequenceTest(IEnumerable<Tuple<Object, ItemType, Boolean, Type, Type>> tuples)
        {
            FilterCollection sequence = new FilterCollection();
            foreach (Tuple<Object, ItemType, Boolean, Type, Type> tuple in tuples)
            {
                Object item = tuple.Item1;
                ItemType itemType = tuple.Item2;
                Boolean shouldSucceed = tuple.Item3;
                Type inputType = tuple.Item4;
                Type outputType = tuple.Item5;

                void SequenceAdd(MethodInfo addMethod)
                {
                    Int32 countBeforeAdd = sequence.Count;
                    InvalidFilterException invalidFilterException = null;
                    try
                    {
                        try
                        {
                            addMethod.Invoke(null, new[] { sequence, item });
                        }
                        catch (TargetInvocationException e)
                        {
                            // ReSharper disable once PossibleNullReferenceException
                            throw e.InnerException;
                        }
                    }
                    catch (InvalidFilterException e)
                    {
                        invalidFilterException = e;
                    }
                    Int32 countAfterAdd = sequence.Count;
                    if (shouldSucceed) // The item should be added successfully.
                    {
                        Assert.Null(invalidFilterException); // Because the item should be added successfully, there shouldn't be any exception.
                        Assert.Equal(countBeforeAdd + 1, countAfterAdd); // If the item is added successfully the count of the sequence should be incremented by 1.
                        if (countAfterAdd == 1) // The item that is added is the first in the sequence.
                            Assert.True(sequence.FirstFilter == sequence.LastFilter); // The first item in the sequence should be equal to the last item in the sequence, since it's the first and only item in the sequence.
                        Assert.Equal(inputType, sequence.LastFilter.InputType); // Check if the input type of the last item is equal to the input type of this item.
                        Assert.Equal(outputType, sequence.LastFilter.OutputType); // Check if the output type of the last item is equal to the output type of this item.
                    }
                    else // The item cannot be added.
                    {
                        Assert.NotNull(invalidFilterException); // The filter shouldn't be added so there should be an exception.
                        Assert.Equal(countBeforeAdd, countAfterAdd); // The count of the sequence shouldn't be incremented.
                    }
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (itemType)
                {
                    case ItemType.FilterData:
                        SequenceAdd(sequenceAddFilterData);
                        break;
                    case ItemType.Filter:
                        SequenceAdd(sequenceAddFilterDefinition.MakeGenericMethod(inputType, outputType));
                        break;
                    case ItemType.Pipeline:
                        SequenceAdd(sequenceAddPipelineDefinition.MakeGenericMethod(inputType, outputType));
                        break;
                    case ItemType.Function:
                        SequenceAdd(sequenceAddFunctionDefinition.MakeGenericMethod(inputType, outputType));
                        break;
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
                    new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    },
                    typeof(String), typeof(Object), true
                };
                yield return new Object[]
                {
                    new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter()
                    },
                    typeof(IEnumerable<Char>), typeof(String), true
                };
                yield return new Object[]
                {
                    new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter()
                    },
                    typeof(String), typeof(IEnumerable<Char>), true
                };
            }
        }

        [Theory]
        [MemberData(nameof(SequenceCanCreatePipelineTestData))]
        [AssertionMethod]
        public void SequenceCanCreatePipelineTest(FilterCollection sequence, Type pipelineInputType, Type pipelineOutputType, Boolean canCreate)
        {
            Assert.Equal(canCreate, sequence.CanCreatePipeline(pipelineInputType, pipelineOutputType));
        }

        public enum ItemType
        {
            FilterData,
            Filter,
            Pipeline,
            Function
        }

        // ReSharper disable once UnusedMember.Local
        private static void SequenceAddFilter<TFilterInput, TFilterOutput>(FilterCollection sequence, IFilter<TFilterInput, TFilterOutput> filter)
        {
            sequence.Add(filter);
        }

        // ReSharper disable once UnusedMember.Local
        private static void SequenceAddPipeline<TPipelineInput, TPipelineOutput>(FilterCollection sequence, IPipeline<TPipelineInput, TPipelineOutput> pipeline)
        {
            sequence.Add(pipeline);
        }

        // ReSharper disable once UnusedMember.Local
        private static void SequenceAddFunction<TFunctionInput, TFunctionOutput>(FilterCollection sequence, Func<TFunctionInput, TFunctionOutput> func)
        {
            sequence.Add(func);
        }

        // ReSharper disable once UnusedMember.Local
        private static void SequenceAddFilterData(FilterCollection sequence, FilterData data)
        {
            sequence.Add(data);
        }

        [Fact]
        [AssertionMethod]
        public void SequenceAddFilterDataNullTest()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.Throws<ArgumentNullException>(() => sequence.Add(null)); // Null can't be added to a filter sequence.
        }

        [Fact]
        [AssertionMethod]
        public void SequenceAddFilterNullTest()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.Throws<ArgumentNullException>(() => sequence.Add<Object, Object>(filter: null)); // Null can't be added to a filter sequence.
        }

        [Fact]
        [AssertionMethod]
        public void SequenceAddFunctionNullTest()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.Throws<ArgumentNullException>(() => sequence.Add<Object, Object>(func: null)); // Null can't be added to a filter sequence.
        }

        [Fact]
        [AssertionMethod]
        public void SequenceAddPipelineNullTest()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.Throws<ArgumentNullException>(() => sequence.Add<Object, Object>(pipeline: null)); // Null can't be added to a filter sequence.
        }

        [Fact]
        [AssertionMethod]
        public void SequenceCanCreatePipelineParametersNullTest()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.False(sequence.CanCreatePipeline(null, typeof(Object))); // Null in any of the parameters in the 'CanCreatePipeline' method should return false.
            Assert.False(sequence.CanCreatePipeline(typeof(Object), null)); // Null in any of the parameters in the 'CanCreatePipeline' method should return false.
            Assert.False(sequence.CanCreatePipeline(null, null)); // Null in any of the parameters in the 'CanCreatePipeline' method should return false.
        }
    }
}