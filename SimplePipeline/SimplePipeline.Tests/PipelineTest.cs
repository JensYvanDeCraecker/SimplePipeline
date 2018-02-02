using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        [Test]
        [TestCaseSource(nameof(ExecutePipelineData))]
        public TOutput ExecutePipeline<TInput, TOutput>(IPipeline<TInput, TOutput> pipeline, TInput input)
        {
            return pipeline.Execute(input) ? pipeline.Output : default(TOutput);
        }

        public static IEnumerable<TestCaseData> ExecutePipelineData
        {
            get
            {
                yield return new TestCaseData(new Pipeline<String, String>()
                {
                    ((Func<String, String>)(input => input.ToLower())).ToFilter(),
                    ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(),
                    ((Func<String, String>)(input => input.Substring(0, 4))).ToFilter()
                }, "Jens Yvan De Craecker").Returns("rekc");
            }
        }
    }
}