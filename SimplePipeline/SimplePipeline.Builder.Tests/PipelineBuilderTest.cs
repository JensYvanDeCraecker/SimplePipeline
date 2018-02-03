using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Builder.Tests
{
    [TestFixture]
    public class PipelineBuilderTest
    {
        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                yield return new TestCaseData(PipelineBuilder.Start<String>(), "SimplePipeline is an easy to use pipeline system.");
                yield return new TestCaseData(PipelineBuilder.Start<List<Decimal>>(), new List<Decimal>());
                yield return new TestCaseData(PipelineBuilder.Start<Int32>(), 4646);
                yield return new TestCaseData(PipelineBuilder.Start<Boolean>(), true);
                yield return new TestCaseData(PipelineBuilder.Start<Char[]>(), new Char[0]);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void ChainBuildTest<T>(IPipelineBuilder<T, T> pipelineBuilder, T pipelineInput)
        {
            IPipelineBuilder<T, Int32> chainPipelineBuilder = pipelineBuilder.Chain(input => input.GetHashCode());
            Assert.GreaterOrEqual(chainPipelineBuilder.Count(), 1);
            IPipeline<T, Int32> chainPipeline = chainPipelineBuilder.Build();
            Assert.IsTrue(chainPipeline.SequenceEqual(chainPipelineBuilder));
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void ChainInputConvertBuildTest<T>(IPipelineBuilder<T, T> pipelineBuilder, T pipelineInput)
        {
            IPipelineBuilder<T, Int32> chainPipelineBuilder = pipelineBuilder.Chain(originalInput => originalInput, Filter.ToFilter<Object, Int32>(input => input.GetHashCode()));
            Assert.GreaterOrEqual(chainPipelineBuilder.Count(), 1);
            IPipeline<T, Int32> chainPipeline = chainPipelineBuilder.Build();
            Assert.IsTrue(chainPipeline.SequenceEqual(chainPipelineBuilder));
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void ChainOutputConvertBuildTest<T>(IPipelineBuilder<T, T> pipelineBuilder, T pipelineInput)
        {
            IPipelineBuilder<T, String> chainPipelineBuilder = pipelineBuilder.Chain(Filter.ToFilter<T, Int32>(input => input.GetHashCode()), originalOutput => originalOutput.ToString());
            Assert.GreaterOrEqual(chainPipelineBuilder.Count(), 1);
            IPipeline<T, String> chainPipeline = chainPipelineBuilder.Build();
            Assert.IsTrue(chainPipeline.SequenceEqual(chainPipelineBuilder));
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void ChainInputOutputConvertBuildTest<T>(IPipelineBuilder<T, T> pipelineBuilder, T pipelineInput)
        {
            IPipelineBuilder<T, String> chainPipelineBuilder = pipelineBuilder.Chain(originalInput => originalInput, Filter.ToFilter<Object, Int32>(input => input.GetHashCode()), originalOutput => originalOutput.ToString());
            Assert.GreaterOrEqual(chainPipelineBuilder.Count(), 1);
            IPipeline<T, String> chainPipeline = chainPipelineBuilder.Build();
            Assert.IsTrue(chainPipeline.SequenceEqual(chainPipelineBuilder));
        }
    }
}