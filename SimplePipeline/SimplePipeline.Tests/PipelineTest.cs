using System;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        private readonly MethodInfo processPipelineSuccessfulDefenition = typeof(PipelineTest).GetMethod("ProcessPipelineSuccessful", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly MethodInfo processPipelineFailureDefenition = typeof(PipelineTest).GetMethod("ProcessPipelineFailure",BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly MethodInfo processPipelineInvalidSequenceDefenition = typeof(PipelineTest).GetMethod("ProcessPipelineInvalidSequence", BindingFlags.NonPublic | BindingFlags.Instance);

        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineSuccessful<TPipelineInput, TPipelineOutput>(FilterCollection filters, TPipelineInput pipelineInput, TPipelineOutput expectedPipelineOutput)
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
            else
            {
                Assert.AreEqual(default(TPipelineOutput), createdPipeline.Output);
            }
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = createdPipeline.ToFilter();
            TPipelineOutput filterOutput = default(TPipelineOutput);
            Assert.DoesNotThrow(() => filterOutput = convertedFilter.Execute(pipelineInput));
            Assert.AreEqual(expectedPipelineOutput, filterOutput);
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineFailure<TPipelineInput, TPipelineOutput>(FilterCollection filters, TPipelineInput pipelineInput, Type expectedExceptionType)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            Assert.IsFalse(createdPipeline.Execute(pipelineInput));
            Assert.AreEqual(default(TPipelineOutput), createdPipeline.Output);
            Assert.AreNotEqual(default(Exception), createdPipeline.Exception);
            Type exceptionType = createdPipeline.Exception.GetType();
            Assert.AreEqual(expectedExceptionType, exceptionType);
            Assert.IsFalse(createdPipeline.IsBeginState);
            createdPipeline.Reset();
            Assert.IsTrue(createdPipeline.IsBeginState);
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = createdPipeline.ToFilter();
            Assert.Throws(exceptionType, () => convertedFilter.Execute(pipelineInput));
        }


        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineInvalidSequence<TPipelineInput, TPipelineOutput>(FilterCollection filters)
        {
            Assert.Throws<InvalidFilterCollectionException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineFailureData))]
        public void PipelineFailure(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput, Type expectedExceptionType)
        {
            processPipelineFailureDefenition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filters, pipelineInput, expectedExceptionType });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineInvalidSequenceData))]
        public void PipelineInvalidSequence(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType)
        {
            processPipelineInvalidSequenceDefenition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new Object[] { filters });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineSuccessfulData))]
        public void PipelineSuccessful(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput, Object expectedPipelineOutput)
        {
            processPipelineSuccessfulDefenition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filters, pipelineInput, expectedPipelineOutput });
        }
    }
}