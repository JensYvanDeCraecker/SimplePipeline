using System;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        private readonly MethodInfo processPipelineSuccessfulDefinition = typeof(PipelineTest).GetMethod("ProcessPipelineSuccessful", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly MethodInfo processPipelineFailureDefinition = typeof(PipelineTest).GetMethod("ProcessPipelineFailure", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly MethodInfo processPipelineInvalidSequenceDefinition = typeof(PipelineTest).GetMethod("ProcessPipelineInvalidSequence", BindingFlags.NonPublic | BindingFlags.Instance);

        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineSuccessful<TPipelineInput, TPipelineOutput>(FilterCollection filters, TPipelineInput pipelineInput, TPipelineOutput expectedPipelineOutput)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            Assert.IsTrue(createdPipeline.Execute(pipelineInput)); // Because we're testing a pipeline here which should be executed without any errors with the provided input, the execute method should return true.
            Assert.AreEqual(default(Exception), createdPipeline.Exception); // When an execution is successful, there shouldn't be any exception.
            Assert.AreEqual(expectedPipelineOutput, createdPipeline.Output); // We're checking if the output of the execution is equal to the expected output.
            if (!createdPipeline.IsBeginState) // Because a successful execution can return the default value, we're testing if the pipeline is still in its begin state. When a pipeline is not in its begin state, we know that the output isn't the default value.
            {
                Assert.AreNotEqual(default(TPipelineOutput), createdPipeline.Output); // The pipeline isn't in its begin state, so the output shouldn't be the default value.
                createdPipeline.Reset();
                Assert.IsTrue(createdPipeline.IsBeginState); // When a pipeline is reset and there haven't been any execution since the reset, the begin state should be true.
            }
            else
                Assert.AreEqual(default(TPipelineOutput), createdPipeline.Output); // When the pipeline is still in its begin state after a successful execution, the output should be the default value.
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = createdPipeline.ToFilter();
            TPipelineOutput filterOutput = default(TPipelineOutput);
            Assert.DoesNotThrow(() => filterOutput = convertedFilter.Execute(pipelineInput)); // When an execution with the same input was true earlier, it should be successful this time.
            Assert.AreEqual(expectedPipelineOutput, filterOutput); // We're testing if the converted pipeline still returns the same output.
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineFailure<TPipelineInput, TPipelineOutput>(FilterCollection filters, TPipelineInput pipelineInput, Type expectedExceptionType)
        {
            IPipeline<TPipelineInput, TPipelineOutput> createdPipeline = new Pipeline<TPipelineInput, TPipelineOutput>(filters);
            Assert.IsFalse(createdPipeline.Execute(pipelineInput)); // Because we're testing a pipeline here which should be executed incorrectly with the provided input, the execute method should return false.
            Assert.AreEqual(default(TPipelineOutput), createdPipeline.Output); // When an execution is unsuccessful, there shouldn't be an output.
            Assert.AreNotEqual(default(Exception), createdPipeline.Exception); // When an execution is unsuccessful, there should be an exception describing the source of the failure.
            Type exceptionType = createdPipeline.Exception.GetType();
            Assert.AreEqual(expectedExceptionType, exceptionType); // We're testing if the type of the thrown exception is equal to the expected exception type.
            Assert.IsFalse(createdPipeline.IsBeginState); // A failed execution should have an exception so it shouldn't be in the begin state.
            createdPipeline.Reset();
            Assert.IsTrue(createdPipeline.IsBeginState); // When a pipeline is reset and there haven't been any execution since the reset, the begin state should be true.
            IFilter<TPipelineInput, TPipelineOutput> convertedFilter = createdPipeline.ToFilter();
            Assert.Throws(exceptionType, () => convertedFilter.Execute(pipelineInput)); // We're testing if the converted pipeline throws the same exception.
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
            processPipelineFailureDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filters, pipelineInput, expectedExceptionType });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineInvalidSequenceData))]
        public void PipelineInvalidSequence(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType)
        {
            processPipelineInvalidSequenceDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new Object[] { filters });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PipelineSuccessfulData))]
        public void PipelineSuccessful(FilterCollection filters, Type pipelineInputType, Type pipelineOutputType, Object pipelineInput, Object expectedPipelineOutput)
        {
            processPipelineSuccessfulDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { filters, pipelineInput, expectedPipelineOutput });
        }
    }
}