using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SimplePipeline.Tests.Filters;
using SimplePipeline.Tests.Pipelines;
using Xunit;

namespace SimplePipeline.Tests
{
    public class PipelineTest
    {
        private readonly MethodInfo processCreatePipelineEnumerableTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessCreatePipelineEnumerableTest), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processCreatePipelineSequenceTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessCreatePipelineSequenceTest), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processExecutePipelineTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessExecutePipelineTest), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processPipelineToFilterTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessPipelineToFilterTest), BindingFlags.NonPublic | BindingFlags.Static);

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> PipelineToFilterTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new CountElementsPipeline<Char>(),
                    typeof(String), typeof(Int32), "Pipeline", 8, null, true
                };
                yield return new Object[]
                {
                    new CountElementsPipeline<Char>(),
                    typeof(String), typeof(Int32), null, 0, typeof(ArgumentNullException), false
                };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterSequence()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), null, null, null, true };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterSequence()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), "Pipeline", "Pipeline", null, true };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> ExecutePipelineTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new CountElementsPipeline<Char>(),
                    typeof(String), typeof(Int32), "Pipeline", 8, null, true
                };
                yield return new Object[]
                {
                    new CountElementsPipeline<Char>(),
                    typeof(String), typeof(Int32), null, 0, typeof(ArgumentNullException), false
                };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterSequence()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), null, null, null, true };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterSequence()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), "Pipeline", "Pipeline", null, true };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreatePipelineSequenceTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    },
                    typeof(String), typeof(Int32), true
                };
                yield return new Object[]
                {
                    new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    },
                    typeof(String), typeof(Object), true
                };
                yield return new Object[]
                {
                    new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter()
                    },
                    typeof(IEnumerable<Char>), typeof(String), true
                };
                yield return new Object[]
                {
                    new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter()
                    },
                    typeof(String), typeof(IEnumerable<Char>), true
                };
                yield return new Object[]
                {
                    new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter()
                    },
                    typeof(IEnumerable<String>), typeof(String), false
                };
                yield return new Object[] { new FilterSequence(), typeof(String), typeof(IEnumerable<Char>), true };
                yield return new Object[] { new FilterSequence(), typeof(IEnumerable<Char>), typeof(String), false };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreatePipelineEnumerableTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>())
                    },
                    typeof(String), typeof(Int32), true
                };
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(new CharEnumerableToStringFilter()),
                        FilterData.Create(new EnumerableCountFilter<Char>())
                    },
                    typeof(String), typeof(Object), true
                };
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(new CharEnumerableToStringFilter())
                    },
                    typeof(IEnumerable<Char>), typeof(String), true
                };
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(new CharEnumerableToStringFilter())
                    },
                    typeof(String), typeof(IEnumerable<Char>), true
                };
                yield return new Object[]
                {
                    new List<FilterData>()
                    {
                        FilterData.Create(new EnumerableToArrayFilter<Char>()),
                        FilterData.Create(new CharEnumerableToStringFilter())
                    },
                    typeof(IEnumerable<String>), typeof(String), false
                };
                yield return new Object[] { new FilterSequence(), typeof(String), typeof(IEnumerable<Char>), true };
                yield return new Object[] { new FilterSequence(), typeof(IEnumerable<Char>), typeof(String), false };
            }
        }

        [Theory]
        [MemberData(nameof(PipelineToFilterTestData))]
        [AssertionMethod]
        public void PipelineToFilterTest(Object pipeline, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput, Object expectedPipelineOutput, Type expectedPipelineExceptionType, Boolean shouldSucceed)
        {
            processPipelineToFilterTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new[] { pipeline, pipelineInput, expectedPipelineOutput, expectedPipelineExceptionType, shouldSucceed });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessPipelineToFilterTest<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline, TPipelineInput pipelineInput, TPipelineOutput expectedPipelineOutput, Type expectedPipelineExceptionType, Boolean shouldSucceed)
        {
            IFilter<TPipelineInput, TPipelineOutput> filter = pipeline.ToFilter();
            if (shouldSucceed)
                Assert.Equal(expectedPipelineOutput, filter.Execute(pipelineInput));
            else
                Assert.Throws(expectedPipelineExceptionType, () => filter.Execute(pipelineInput));
            Assert.True(pipeline.IsBeginState);
            Assert.Throws<InvalidOperationException>(() => pipeline.Exception);
            Assert.Throws<InvalidOperationException>(() => pipeline.Output);
        }

        [Theory]
        [MemberData(nameof(ExecutePipelineTestData))]
        [AssertionMethod]
        public void ExecutePipelineTest(Object pipeline, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput, Object expectedPipelineOutput, Type expectedPipelineExceptionType, Boolean shouldSucceed)
        {
            processExecutePipelineTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new[] { pipeline, pipelineInput, expectedPipelineOutput, expectedPipelineExceptionType, shouldSucceed });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessExecutePipelineTest<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline, TPipelineInput pipelineInput, TPipelineOutput expectedPipelineOutput, Type expectedPipelineExceptionType, Boolean shouldSucceed)
        {
            Assert.Equal(shouldSucceed, pipeline.Execute(pipelineInput));
            Assert.False(pipeline.IsBeginState);
            if (shouldSucceed)
            {
                Assert.Equal(expectedPipelineOutput, pipeline.Output);
                Assert.Throws<InvalidOperationException>(() => pipeline.Exception);
            }
            else
            {
                Assert.IsType(expectedPipelineExceptionType, pipeline.Exception);
                Assert.Throws<InvalidOperationException>(() => pipeline.Output);
            }
            pipeline.Reset();
            Assert.Throws<InvalidOperationException>(() => pipeline.Exception);
            Assert.Throws<InvalidOperationException>(() => pipeline.Output);
        }

        [Theory]
        [MemberData(nameof(CreatePipelineSequenceTestData))]
        [AssertionMethod]
        public void CreatePipelineSequenceTest(FilterSequence sequence, Type pipelineInputType, Type pipelineOutputType, Boolean shouldSucceed)
        {
            processCreatePipelineSequenceTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new Object[] { sequence, shouldSucceed });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessCreatePipelineSequenceTest<TPipelineInput, TPipelineOutput>(FilterSequence sequence, Boolean shouldSucceed)
        {
            Pipeline<TPipelineInput, TPipelineOutput> CreatePipelineSequence()
            {
                return new Pipeline<TPipelineInput, TPipelineOutput>(sequence);
            }

            if (shouldSucceed)
            {
                Pipeline<TPipelineInput, TPipelineOutput> pipeline = CreatePipelineSequence();
                Assert.Equal(sequence.Count, pipeline.Count());
                if (!sequence.Any())
                    return;
                Assert.Equal(sequence.FirstFilter, pipeline.First());
                Assert.Equal(sequence.LastFilter, pipeline.Last());
            }
            else
                Assert.Throws<InvalidFilterCollectionException>(() => CreatePipelineSequence());
        }

        [Theory]
        [MemberData(nameof(CreatePipelineEnumerableTestData))]
        [AssertionMethod]
        public void CreatePipelineEnumerableTest(IEnumerable<FilterData> filters, Type pipelineInputType, Type pipelineOutputType, Boolean shouldSucceed)
        {
            processCreatePipelineEnumerableTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new Object[] { filters, shouldSucceed });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessCreatePipelineEnumerableTest<TPipelineInput, TPipelineOutput>(IEnumerable<FilterData> filters, Boolean shouldSucceed)
        {
            Pipeline<TPipelineInput, TPipelineOutput> CreatePipelineEnumerable()
            {
                // ReSharper disable once PossibleMultipleEnumeration
                return new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            }

            if (shouldSucceed)
            {
                Pipeline<TPipelineInput, TPipelineOutput> pipeline = CreatePipelineEnumerable();

                // ReSharper disable once PossibleMultipleEnumeration
                Assert.Equal(filters.Count(), pipeline.Count());

                // ReSharper disable once PossibleMultipleEnumeration
                if (!filters.Any())
                    return;

                // ReSharper disable once PossibleMultipleEnumeration
                Assert.Equal(filters.First(), pipeline.First());

                // ReSharper disable once PossibleMultipleEnumeration
                Assert.Equal(filters.Last(), pipeline.Last());
            }
            else
                Assert.Throws<InvalidFilterCollectionException>(() => CreatePipelineEnumerable());
        }

        [Fact]
        [AssertionMethod]
        public void CreatePipelineEnumerableNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new Pipeline<Object, Object>(filters: null));
        }

        [Fact]
        [AssertionMethod]
        public void CreatePipelineSequenceNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new Pipeline<Object, Object>(null));
        }

        [Fact]
        [AssertionMethod]
        public void PipelineToFilterNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => ((IPipeline<Object, Object>)null).ToFilter()); // Null can't be converted to a filter.
        }
    }
}