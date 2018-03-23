using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SimplePipeline.Tests.Filters;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class PipelineTest
    {
        private readonly MethodInfo processCreatePipelineSequenceTestDefinition = typeof(PipelineTest).GetMethod("ProcessCreatePipelineSequenceTest", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processExecutePipelineTestDefinition = typeof(PipelineTest).GetMethod("ProcessExecutePipelineTest", BindingFlags.NonPublic | BindingFlags.Static);

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreatePipelineSequenceTestData
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
                yield return new Object[]
                {
                    new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter()
                    },
                    typeof(IEnumerable<String>), typeof(String), false
                };
                yield return new Object[] { new FilterCollection(), typeof(String), typeof(IEnumerable<Char>), true };
                yield return new Object[] { new FilterCollection(), typeof(IEnumerable<Char>), typeof(String), false };
            }
        }

        [Theory]
        [MemberData(nameof(CreatePipelineSequenceTestData))]
        [AssertionMethod]
        public void CreatePipelineSequenceTest(FilterCollection sequence, Type pipelineInputType, Type pipelineOutputType, Boolean shouldSucceed)
        {
            processCreatePipelineSequenceTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(null, new Object[] { sequence, shouldSucceed });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessCreatePipelineSequenceTest<TPipelineInput, TPipelineOutput>(FilterCollection sequence, Boolean shouldSucceed)
        {
            Pipeline<TPipelineInput, TPipelineOutput> CreatePipeline()
            {
                return new Pipeline<TPipelineInput, TPipelineOutput>(sequence);
            }

            if (shouldSucceed)
            {
                Pipeline<TPipelineInput, TPipelineOutput> pipeline = CreatePipeline();
                Assert.Equal(sequence.Count, pipeline.Count());
                if (sequence.Count <= 0)
                    return;
                Assert.Equal(sequence.FirstFilter.Filter, pipeline.First());
                Assert.Equal(sequence.LastFilter.Filter, pipeline.Last());
            }
            else
                Assert.Throws<InvalidFilterCollectionException>(() => CreatePipeline());
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> ExecutePipelineTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[]
                {
                    new Pipeline<IEnumerable<Char>, Int32>(new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    }),
                    typeof(String), typeof(Int32), "Pipeline", 8, null, true
                };
                yield return new Object[]
                {
                    new Pipeline<IEnumerable<Char>, Int32>(new FilterCollection()
                    {
                        new EnumerableToArrayFilter<Char>(),
                        new CharEnumerableToStringFilter(),
                        new EnumerableCountFilter<Char>()
                    }),
                    typeof(String), typeof(Int32), null, 0, typeof(ArgumentNullException), false
                };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterCollection()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), null, null, null, true };
                yield return new Object[] { new Pipeline<IEnumerable<Char>, IEnumerable<Char>>(new FilterCollection()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), "Pipeline", "Pipeline", null, true };
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
            Assert.Throws<ArgumentNullException>(() => new Pipeline<Object, Object>(filterDatas: null));
        }

        [Fact]
        [AssertionMethod]
        public void CreatePipelineSequenceNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => new Pipeline<Object, Object>(null));
        }
    }
}