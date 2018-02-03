using System;
using System.Collections.Generic;
using System.Globalization;
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
                yield return new TestCaseData(PipelineBuilder.Start<List<Decimal>>(), new List<decimal>());
                yield return new TestCaseData(PipelineBuilder.Start<Int32>(), 4646);
                yield return new TestCaseData(PipelineBuilder.Start<Boolean>(), true);
                yield return new TestCaseData(PipelineBuilder.Start<char[]>(), new char[0]);
            }
        }

        [Test]
        [TestCaseSource(nameof(ChainTestData))]
        public void ChainBuildTest<T>(IPipelineBuilder<T, T> sourceBuilder, T pipelineInput)
        {
      
        }
    }
}