using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using SimplePipeline.Tests.Filters;
using Xunit;

namespace SimplePipeline.Tests
{
    public class FilterDataTest
    {
        private readonly MethodInfo processCreateFilterDataDefinition = typeof(FilterDataTest).GetMethod(nameof(ProcessCreateFilterDataTest), BindingFlags.NonPublic | BindingFlags.Static);
        private readonly MethodInfo processGetGenericFilterTestDefinition = typeof(FilterDataTest).GetMethod(nameof(ProcessGetGenericFilterTest), BindingFlags.NonPublic | BindingFlags.Static);

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> CreateFilterDataTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(IEnumerable<Char>), typeof(String) };
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>) };
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(String), typeof(String) };
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> FilterDataEqualityTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                IFilter<IEnumerable<Char>, String> charEnumerableToString = new CharEnumerableToStringFilter();
                yield return new Object[] { FilterData.Create(charEnumerableToString), FilterData.Create(charEnumerableToString), true };
                yield return new Object[] { FilterData.Create(charEnumerableToString), FilterData.Create<IEnumerable<Char>, IEnumerable<Char>>(charEnumerableToString), false };
                yield return new Object[] { FilterData.Create(charEnumerableToString), FilterData.Create<String, String>(charEnumerableToString), false };
                yield return new Object[] { FilterData.Create(charEnumerableToString), FilterData.Create<String, IEnumerable<Char>>(charEnumerableToString), false };
                yield return new Object[] { FilterData.Create(charEnumerableToString), FilterData.Create(new CharEnumerableToStringFilter()), false };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> FilterDataExecuteFilterTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), true, new[] { 'S', 'i', 'm', 'p', 'l', 'e', 'P', 'i', 'p', 'e', 'l', 'i', 'n', 'e' }, "SimplePipeline", null };
                yield return new Object[] { FilterData.Create(((Func<Object, Object>)(input => throw new Exception())).ToFilter()), false, null, null, typeof(Exception) };
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<Object[]> GetGenericFilterTestData
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(IEnumerable<Char>), typeof(String), true };
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(String), typeof(String), true };
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>), true };
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(String), typeof(IEnumerable<Char>), true };
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(IEnumerable<Char>), typeof(IEnumerable<String>), false };
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(IEnumerable<String>), typeof(String), false };
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), typeof(IEnumerable<String>), typeof(IEnumerable<String>), false };
            }
        }

        [Theory]
        [MemberData(nameof(CreateFilterDataTestData))]
        [AssertionMethod]
        public void CreateFilterDataTest(Object filter, Type filterInputType, Type filterOutputType)
        {
            processCreateFilterDataDefinition.MakeGenericMethod(filterInputType, filterOutputType).Invoke(null, new[] { filter });
        }

        [Theory]
        [MemberData(nameof(FilterDataEqualityTestData))]
        [AssertionMethod]
        public void FilterDataEqualityTest(FilterData firstFilter, FilterData secondFilter, Boolean expectedResult)
        {
            Assert.Equal(expectedResult, Equals(firstFilter, secondFilter)); // Test if the equality is equal to the expected result.
            Assert.Equal(expectedResult, firstFilter == secondFilter); // Test if the equality is equal to the expected result.
            Assert.Equal(!expectedResult, firstFilter != secondFilter); // Test if the equality is equal to the expected result.
            Assert.Equal(expectedResult, firstFilter.GetHashCode() == secondFilter.GetHashCode()); // Test if the equality of the hash code is equal to the expected result.
        }

        [AssertionMethod]
        private static void ProcessCreateFilterDataTest<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            FilterData data = FilterData.Create(filter);
            Assert.Equal(typeof(TFilterInput), data.InputType); // Test if the 'InputType' property is equal to the input type of the filter.
            Assert.Equal(typeof(TFilterOutput), data.OutputType); // Test if the 'OutputType' property is equal to the output type of the filter.
            Assert.Same(filter, data.GetGenericFilter<TFilterInput, TFilterOutput>()); // Test if the 'GetGenericFilter' method is the same as the filter.
        }

        [Theory]
        [MemberData(nameof(FilterDataExecuteFilterTestData))]
        [AssertionMethod]
        public void FilterDataExecuteFilterTest(FilterData filter, Boolean shouldSucceed, Object filterInput, Object expectedFilterOutput, Type expectedExceptionType)
        {
            if (shouldSucceed)
                Assert.Equal(expectedFilterOutput, filter.Execute(filterInput)); // Test if the output of the 'Execute' method is equal to the expected output.
            else
                Assert.Throws(expectedExceptionType, () => filter.Execute(filterInput)); // Test if the expected exception is thrown when the 'Execute' method is expected to fail.
        }

        [Theory]
        [MemberData(nameof(GetGenericFilterTestData))]
        [AssertionMethod]
        public void GetGenericFilterTest(FilterData filter, Type filterInputType, Type filterOutputType, Boolean shouldSucceed)
        {
            processGetGenericFilterTestDefinition.MakeGenericMethod(filterInputType, filterOutputType).Invoke(null, new Object[] { filter, shouldSucceed });
        }

        [AssertionMethod]
        private static void ProcessGetGenericFilterTest<TFilterInput, TFilterOutput>(FilterData filter, Boolean shouldSucceed)
        {
            void GetGenericFilter()
            {
                filter.GetGenericFilter<TFilterInput, TFilterOutput>();
            }

            if (shouldSucceed)
                GetGenericFilter();
            else
                Assert.Throws<ArgumentException>(() => GetGenericFilter());
        }

        [Fact]
        [AssertionMethod]
        public void CreateFilterDataNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => FilterData.Create<Object, Object>(null)); // A 'FilterData' instance can't be created from null.
        }
    }
}