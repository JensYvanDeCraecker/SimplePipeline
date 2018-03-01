using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplePipeline.Tests.Pipelines;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterTest
    {
        private readonly MethodInfo processFunctionToFilterDefinition = typeof(FilterTest).GetMethod("ProcessFunctionToFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly MethodInfo processPipelineToFilterDefinition = typeof(FilterTest).GetMethod("ProcessPipelineToFilter", BindingFlags.NonPublic | BindingFlags.Instance);

        public static IEnumerable<Object[]> Functions
        {
            get
            {
                yield return new Object[] { (Func<String, Int32>)Int32.Parse, typeof(String), typeof(Int32) };
                yield return new Object[] { (Func<String, Char[]>)(input => input.ToCharArray()), typeof(String), typeof(IEnumerable<Char>) };
                yield return new Object[] { (Func<IEnumerable<Char>, Int32>)(input => input.Count()), typeof(String), typeof(Int32) };
                yield return new Object[] { (Func<IEnumerable<Char>, String>)(input => new String(input.ToArray())), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        public static IEnumerable<Object[]> Pipelines
        {
            get
            {
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(IEnumerable<Char>), typeof(Char[]) };
                yield return new Object[]{new EnumerableToArrayPipeline<Char>(), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>) };
                yield return new Object[]{new EnumerableToArrayPipeline<Char>(), typeof(String), typeof(Char[]) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        [Fact]
        public void FunctionToFilterNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Func<Object, Object>)null).ToFilter());
        }

        [Fact]
        public void PipelineToFilterNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((IPipeline<Object, Object>)null).ToFilter());
        }

        [Theory]
        [MemberData(nameof(Functions))]
        public void FunctionToFilter(Delegate function, Type functionInputType, Type functionOutputType)
        {
            processFunctionToFilterDefinition.MakeGenericMethod(functionInputType, functionOutputType).Invoke(this, new Object[] { function });
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessFunctionToFilter<TFunctionInput, TFunctionOutput>(Func<TFunctionInput, TFunctionOutput> function)
        {
            Assert.NotNull(function.ToFilter());
        }

        [Theory]
        [MemberData(nameof(Pipelines))]
        public void PipelineToFilter(Object pipeline, Type pipelineInputType, Type pipelineOutputType)
        {
            processPipelineToFilterDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { pipeline });
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineToFilter<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline)
        {
            Assert.NotNull(pipeline.ToFilter());
        }

    }
}