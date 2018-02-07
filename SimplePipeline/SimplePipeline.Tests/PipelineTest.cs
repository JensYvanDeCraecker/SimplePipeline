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
                yield return new TestCaseData(new List<FilterData>() { ((Func<String, String>)(input => input.ToUpper())).ToFilter().GetData(), ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter().GetData(), ((Func<String, String>)(input => input.Substring(0, 4))).ToFilter().GetData() }, typeof(String), typeof(String), true, true, "SimplePipeline is an easy to use pipeline system.", ".MET");
                yield return new TestCaseData(new List<FilterData>() { ((Func<String, IEnumerable<Char>>)(input => input.ToCharArray())).ToFilter().GetData(), ((Func<Char[], String>)(input => new String(input))).ToFilter().GetData() }, typeof(String), typeof(String), false, false, null, null);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter()), FilterData.Create(((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException("Empty string") : false)).ToFilter()) }, typeof(String), typeof(Boolean), true, false, "    ", null);
            }
        }

        public void ProcessTest<TPipelineInput, TPipelineOutput>(IEnumerable<FilterData> filterDatas, Boolean canBuild, Boolean canExecute, TPipelineInput input, TPipelineOutput expectedOutput)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = CreatePipeline<TPipelineInput, TPipelineOutput>(filterDatas, canBuild);
            if (canBuild)
            {
                Assert.AreNotEqual(default(IPipeline<TPipelineInput, TPipelineOutput>), createdPipeline);
                if (canExecute)
                    ValidateSuccessPipeline(createdPipeline, input, expectedOutput);
                else
                    ValidateFailPipeline(createdPipeline, input);
            }
            else
                Assert.AreEqual(default(IPipeline<TPipelineInput, TPipelineOutput>), createdPipeline);
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

        public IPipeline<TPipelineInput, TPipelineOutput> CreatePipeline<TPipelineInput, TPipelineOutput>(IEnumerable<FilterData> filterDatas, Boolean canBuild)
        {
            IPipeline<TPipelineInput, TPipelineOutput> pipeline = default(IPipeline<TPipelineInput, TPipelineOutput>);
            if (canBuild)
                Assert.DoesNotThrow(() => pipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filterDatas));
            else
                Assert.Throws<ArgumentException>(() => pipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filterDatas));
            return pipeline;
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void BeginTest(IEnumerable<FilterData> filterDatas, Type pipelineInputType, Type pipelineOutputType, Boolean canBuild, Boolean canExecute, Object input, Object expectedOutput)
        {
            processTestDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filterDatas, canBuild, canExecute, input, expectedOutput });
        }
    }
}