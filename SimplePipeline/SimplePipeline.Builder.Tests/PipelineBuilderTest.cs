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
                yield return new TestCaseData(PipelineBuilder.Start<String>().Chain(input => input.Replace(" ", "")), "SimplePipeline is an easy to use pipeline system.");
                yield return new TestCaseData(PipelineBuilder.Start<List<Decimal>>().Chain(input => input.Distinct().ToList()), new List<Decimal>() { 45, 76456, 464658, 465468, 798465, 45646546, 464649, Decimal.MaxValue, Decimal.MinValue, 454, 212, 484, 648, 789, 648, 789, 131, 7846 });
                yield return new TestCaseData(PipelineBuilder.Start<Int32>().Chain(input => input / 2), 4646);
                yield return new TestCaseData(PipelineBuilder.Start<Boolean>().Chain(input => !input), true);
                yield return new TestCaseData(PipelineBuilder.Start<Char[]>().Chain(input => input.Where(character => !Char.IsControl(character)).ToArray()), Enumerable.Range(Char.MinValue, Char.MaxValue).Select(integer => (Char)integer).ToArray());
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
            IPipeline<T, Int32> pipeline = chainPipelineBuilder.Build();
            Assert.AreEqual(chainPipelineBuilder.Count(), pipeline.Count());
            Assert.IsTrue(pipeline.Any(Predicate));
            Assert.IsTrue(pipeline.SequenceEqual(chainPipelineBuilder));
        }
    }
}