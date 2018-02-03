using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Builder.Tests
{
    [TestFixture]
    public class PipelineBuilderTest
    {
        public static IEnumerable<TestCaseData> ChainTestData
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
        [TestCaseSource(nameof(ChainTestData))]
        public void ChainBuildTest<T>(IPipelineBuilder<T, T> sourceBuilder, T pipelineInput)
        {
            IPipeline<T, Decimal> pipeline = sourceBuilder.Chain(input => input.GetHashCode()).Chain(originalInput => originalInput.ToString(), Filter.ToFilter<String, Char[]>(input => input.ToCharArray())).Chain(originalInput => originalInput.Select(character => character.ToString()), Filter.ToFilter<IEnumerable<String>, IEnumerable<IGrouping<String, String>>>(input => input.GroupBy(character => character)), originalOutput => Int32.Parse(originalOutput.First().Key)).Chain(Filter.ToFilter<Int32, Double>(input => Math.Exp(input)), originalOutput => (Decimal)originalOutput).Build();
            Assert.GreaterOrEqual(pipeline.Count(), 4);
            Assert.IsTrue(pipeline.Execute(pipelineInput));
        }
    }
}