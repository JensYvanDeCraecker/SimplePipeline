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
        private readonly MethodInfo processFunctionToFilterDefinition = typeof(FilterTest).GetMethod("ProcessFunctionToFilterTest", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly MethodInfo processPipelineToFilterDefinition = typeof(FilterTest).GetMethod("ProcessPipelineToFilterTest", BindingFlags.NonPublic | BindingFlags.Instance);

        // Syntax: Func<in T, out TResult>, T type, TResult type
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> FunctionToFilterTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[] { (Func<String, Int32>)Int32.Parse, typeof(String), typeof(Int32) };
                yield return new Object[] { (Func<String, Char[]>)(input => input.ToCharArray()), typeof(String), typeof(IEnumerable<Char>) };
                yield return new Object[] { (Func<IEnumerable<Char>, Int32>)(input => input.Count()), typeof(String), typeof(Int32) };
                yield return new Object[] { (Func<IEnumerable<Char>, String>)(input => new String(input.ToArray())), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        // Syntax: IPipeline<in TInput, out TOutput>, TInput type, TOutput type
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> PipelineToFilterTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(IEnumerable<Char>), typeof(Char[]) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(String), typeof(Char[]) };
                yield return new Object[] { new EnumerableToArrayPipeline<Char>(), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        [Theory]
        [MemberData(nameof(FunctionToFilterTestData))]
        public void FunctionToFilterTest(Delegate function, Type functionInputType, Type functionOutputType)
        {
            processFunctionToFilterDefinition.MakeGenericMethod(functionInputType, functionOutputType).Invoke(this, new Object[] { function });
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessFunctionToFilterTest<TFunctionInput, TFunctionOutput>(Func<TFunctionInput, TFunctionOutput> function)
        {
            Assert.NotNull(function.ToFilter());
        }

        [Theory]
        [MemberData(nameof(PipelineToFilterTestData))]
        public void PipelineToFilterTest(Object pipeline, Type pipelineInputType, Type pipelineOutputType)
        {
            processPipelineToFilterDefinition.MakeGenericMethod(pipelineInputType, pipelineOutputType).Invoke(this, new[] { pipeline });
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessPipelineToFilterTest<TPipelineInput, TPipelineOutput>(IPipeline<TPipelineInput, TPipelineOutput> pipeline)
        {
            Assert.NotNull(pipeline.ToFilter());
        }

        [Fact]
        public void FunctionToFilterNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => ((Func<Object, Object>)null).ToFilter());
        }

        [Fact]
        public void PipelineToFilterNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => ((IPipeline<Object, Object>)null).ToFilter());
        }
    }
}