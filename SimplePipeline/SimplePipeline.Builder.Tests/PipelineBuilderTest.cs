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
                yield return new TestCaseData(PipelineBuilder.Start<Int32>().Chain(input => input / 2), 4646);
                yield return new TestCaseData(PipelineBuilder.Start<Boolean>().Chain(input => !input), true);
                yield return new TestCaseData(PipelineBuilder.Start<Char[]>(), new Char[0]);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void ChainBuild<T>(IPipelineBuilder<T, T> pipelineBuilder, T pipelineInput)
        {
            Boolean Predicate(FilterData filterData)
            {
                return filterData.InputType == typeof(T) && filterData.OutputType == typeof(Int32);
            }

            IPipelineBuilder<T, Int32> chainPipelineBuilder = pipelineBuilder.Chain(input => input.GetHashCode());
            Assert.GreaterOrEqual(chainPipelineBuilder.Count(), 1);
            Assert.IsTrue(chainPipelineBuilder.Any(Predicate));
            IPipeline<T, Int32> chainPipeline = chainPipelineBuilder.Build();
            Assert.GreaterOrEqual(chainPipeline.Count(), 1);
            Assert.IsTrue(chainPipeline.Any(Predicate));
            Assert.IsTrue(chainPipeline.SequenceEqual(chainPipelineBuilder));
            Assert.DoesNotThrow(() => chainPipeline.Execute(pipelineInput));
        }
    }
}