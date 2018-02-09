using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        private readonly MethodInfo processTestDefinition = typeof(PipelineTest).GetMethod("ProcessTest");

        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                yield return new TestCaseData(new FilterCollection() { ((Func<String, String>)(input => input.ToUpper())).ToFilter(), ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), ((Func<String, String>)(input => input.Substring(0, 4))).ToFilter() }, typeof(String), typeof(String), true, "SimplePipeline is an easy to use pipeline system.", ".MET");
                yield return new TestCaseData(new FilterCollection() { ((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter(), ((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException("Empty string") : false)).ToFilter() }, typeof(String), typeof(Boolean), false, "    ", null);
                yield return new TestCaseData(new FilterCollection() { ((Func<Int32, Double>)(input => Math.Sqrt(input))).ToFilter() }, typeof(Int32), typeof(Double), true, 4, 2);
                yield return new TestCaseData(new FilterCollection(), typeof(Int32), typeof(Int32), true, 16, 16);
            }
        }

        public void ProcessTest<TPipelineInput, TPipelineOutput>(FilterCollection filterCollection, Boolean canExecute, TPipelineInput input, TPipelineOutput expectedOutput)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filterCollection);
            Assert.AreNotEqual(default(IPipeline<TPipelineInput, TPipelineOutput>), createdPipeline);
            if (canExecute)
                ValidateSuccessPipeline(createdPipeline, input, expectedOutput);
            else
                ValidateFailPipeline(createdPipeline, input);
        }

        public void ValidateFailPipeline<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline, TPipelineInput input)
        {
            Assert.IsFalse(pipeline.Execute(input));
            Assert.AreEqual(default(TPipelineOutput), pipeline.Output);
            Assert.AreNotEqual(default(Exception), pipeline.Exception);
            Type exceptionType = pipeline.Exception.GetType();
            Assert.IsFalse(pipeline.IsBeginState);
            pipeline.Reset();
            Assert.IsTrue(pipeline.IsBeginState);
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = pipeline.ToFilter();
            Assert.Throws(exceptionType, () => convertedFilter.Execute(input));
        }

        public void ValidateSuccessPipeline<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline, TPipelineInput input, TPipelineOutput expectedOutput)
        {
            Assert.IsTrue(pipeline.Execute(input));
            Assert.AreEqual(default(Exception), pipeline.Exception);
            Assert.AreEqual(expectedOutput, pipeline.Output);
            if (!pipeline.IsBeginState)
            {
                Assert.AreNotEqual(default(TPipelineOutput), pipeline.Output);
                pipeline.Reset();
                Assert.IsTrue(pipeline.IsBeginState);
            }
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = pipeline.ToFilter();
            TPipelineOutput filterOutput = default(TPipelineOutput);
            Assert.DoesNotThrow(() => filterOutput = convertedFilter.Execute(input));
            Assert.AreEqual(expectedOutput, filterOutput);
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void BeginTest(FilterCollection filterDatas, Type pipelineInputType, Type pipelineOutputType, Boolean canExecute, Object input, Object expectedOutput)
        {
            processTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filterDatas, canExecute, input, expectedOutput });
        }
    }
}