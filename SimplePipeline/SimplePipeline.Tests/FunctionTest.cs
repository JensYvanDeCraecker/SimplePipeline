using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Xunit;

namespace SimplePipeline.Tests
{
    public class FunctionTest
    {
        private readonly MethodInfo processFunctionToFilterDefinition = typeof(FunctionTest).GetMethod(nameof(ProcessFunctionToFilterTest), BindingFlags.NonPublic | BindingFlags.Static);

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

        [Theory]
        [MemberData(nameof(FunctionToFilterTestData))]
        [AssertionMethod]
        public void FunctionToFilterTest(Delegate func, Type funcInputType, Type funcOutputType)
        {
            processFunctionToFilterDefinition.MakeGenericMethod(funcInputType, funcOutputType).Invoke(null, new Object[] { func });
        }

        // ReSharper disable once UnusedMember.Local
        [AssertionMethod]
        private static void ProcessFunctionToFilterTest<TFunctionInput, TFunctionOutput>(Func<TFunctionInput, TFunctionOutput> func)
        {
            Assert.NotNull(func.ToFilter()); // Test if the 'ToFilter' method returns an instance.
        }

        [Fact]
        [AssertionMethod]
        public void FunctionToFilterNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => ((Func<Object, Object>)null).ToFilter()); // Null can't be converted to a filter.
        }
    }
}