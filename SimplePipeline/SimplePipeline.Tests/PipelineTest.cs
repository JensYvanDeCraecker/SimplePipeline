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
        private readonly MethodInfo processPipelineToFilterTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessPipelineToFilterTest), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processCreatePipelineTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessCreatePipelineTest), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processExecutePipelineTestDefinition = typeof(PipelineTest).GetMethod(nameof(ProcessExecutePipelineTest), BindingFlags.NonPublic | BindingFlags.Static);

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

        [Theory]
        [MemberData(nameof(CreatePipelineSequenceTestData))]
        [AssertionMethod]
        public void CreatePipelineTest(FilterSequence sequence, Type pipelineInputType, Type pipelineOutputType, Boolean shouldSucceed)
        {
            processCreatePipelineTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new Object[] { sequence, shouldSucceed });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessCreatePipelineTest<TPipelineInput, TPipelineOutput>(FilterSequence sequence, Boolean shouldSucceed)
        {
            Pipeline<TPipelineInput, TPipelineOutput> CreatePipelineSequence()
            {
                return new Pipeline<TPipelineInput, TPipelineOutput>(sequence);
            }

            Pipeline<TPipelineInput, TPipelineOutput> CreatePipelineEnumerable()
            {
                return new Pipeline<TPipelineInput, TPipelineOutput>(filters: sequence);
            }

            void ValidatePipeline(Func<Pipeline<TPipelineInput, TPipelineOutput>> creator)
            {
                if (shouldSucceed)
                {
                    Pipeline<TPipelineInput, TPipelineOutput> pipeline = creator.Invoke();
                    Assert.Equal(sequence.Count, pipeline.Count());
                    if (sequence.Count <= 0)
                        return;
                    Assert.Equal(sequence.FirstFilter, pipeline.First());
                    Assert.Equal(sequence.LastFilter, pipeline.Last());
                }
                else
                    Assert.Throws<InvalidFilterCollectionException>(() => CreatePipelineSequence());
            }

            ValidatePipeline(CreatePipelineSequence);
            ValidatePipeline(CreatePipelineEnumerable);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> PipelineToFilterTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(IEnumerable<Char>), typeof(Char[]) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(String), typeof(Char[]) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        [Theory]
        [MemberData(nameof(PipelineToFilterTestData))]
        [AssertionMethod]
        public void PipelineToFilterTest(Object pipeline, Type pipelineInputType, Type pipelineOutputType)
        {
            processPipelineToFilterTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new[] { pipeline });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessPipelineToFilterTest<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline)
        {
            Assert.NotNull(pipeline.ToFilter()); // Test if the 'ToFilter' method returns an instance.
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> ExecutePipelineTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new Pipeline<IEnumerable<Char>, Int32>(new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    }),
                    typeof(String), typeof(Int32), "Pipeline", 8, null, true
                };
                yield return new Object[]
                {
                    new Pipeline<IEnumerable<Char>, Int32>(new FilterSequence()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    }),
                    typeof(String), typeof(Int32), null, 0, typeof(ArgumentNullException), false
                };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterSequence()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), null, null, null, true };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterSequence()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), "Pipeline", "Pipeline", null, true };
            }
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
            if (shouldSucceed)
            {
                Assert.Null(pipeline.Exception);
                if (pipeline.IsBeginState)
                    Assert.Equal(default(TPipelineOutput), pipeline.Output);
                else
                    Assert.NotEqual(default(TPipelineOutput), pipeline.Output);
                Assert.Equal(expectedPipelineOutput, pipeline.Output);
            }
            else
            {
                Assert.False(pipeline.IsBeginState);
                Assert.Equal(default(TPipelineOutput), pipeline.Output);
                Assert.NotNull(pipeline.Exception);
                Assert.IsType(expectedPipelineExceptionType, pipeline.Exception);
                pipeline.Reset();
                Assert.True(pipeline.IsBeginState);
            }
            if (pipeline.IsBeginState)
                return;
            pipeline.Reset();
            Assert.True(pipeline.IsBeginState);
            Assert.Equal(default(TPipelineOutput), pipeline.Output);
            Assert.Null(pipeline.Exception);
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