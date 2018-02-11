using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        private readonly MethodInfo processPipelineSuccessDefenition = typeof(PipelineTest).GetMethod("ProcessPipelineSuccess");
        private readonly MethodInfo processPipelineFailureDefenition = typeof(PipelineTest).GetMethod("ProcessPipelineFailure");

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineSuccessData))]
        public void PipelineSuccess(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput, Object expectedPipelineOutput)
        {
            processPipelineSuccessDefenition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filters, pipelineInput, expectedPipelineOutput });
        }

        public void ProcessPipelineSuccess<TPipelineInput, TPipelineOutput>(FilterCollection filters, TPipelineInput pipelineInput, TPipelineOutput expectedPipelineOutput)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            Assert.IsTrue(createdPipeline.Execute(pipelineInput));
            Assert.AreEqual(default(Exception), createdPipeline.Exception);
            Assert.AreEqual(expectedPipelineOutput, createdPipeline.Output);
            if (!createdPipeline.IsBeginState)
            {
                Assert.AreNotEqual(default(TPipelineOutput), createdPipeline.Output);
                createdPipeline.Reset();
                Assert.IsTrue(createdPipeline.IsBeginState);
            }
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = createdPipeline.ToFilter();
            TPipelineOutput filterOutput = default(TPipelineOutput);
            Assert.DoesNotThrow(() => filterOutput = convertedFilter.Execute(pipelineInput));
            Assert.AreEqual(expectedPipelineOutput, filterOutput);
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineFailureData))]
        public void PipelineFailure(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput)
        {
            processPipelineFailureDefenition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filters, pipelineInput });
        }

        public void ProcessPipelineFailure<TPipelineInput, TPipelineOutput>(FilterCollection filters, TPipelineInput pipelineInput)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            Assert.IsFalse(createdPipeline.Execute(pipelineInput));
            Assert.AreEqual(default(TPipelineOutput), createdPipeline.Output);
            Assert.AreNotEqual(default(Exception), createdPipeline.Exception);
            Type exceptionType = createdPipeline.Exception.GetType();
            Assert.IsFalse(createdPipeline.IsBeginState);
            createdPipeline.Reset();
            Assert.IsTrue(createdPipeline.IsBeginState);
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = createdPipeline.ToFilter();
            Assert.Throws(exceptionType, () => convertedFilter.Execute(pipelineInput));
        }
    }
}