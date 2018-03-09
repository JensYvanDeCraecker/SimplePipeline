using System;
using System.Collections.Generic;
using System.Reflection;
using SimplePipeline.Tests.Filters;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterDataTest
    {
        private readonly MethodInfo processCreateFilterDataDefinition = typeof(FilterDataTest).GetMethod("ProcessCreateFilterDataTest", BindingFlags.NonPublic | BindingFlags.Instance);

        // Syntax: IFilter<in TInput, out TOutput>, TInput type, TOutput type
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

        // Syntax: FilterData, FilterData, Boolean
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
            }
        }

        // Syntax: FilterData, Boolean, Object, Object, Type
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

        [Theory]
        [MemberData(nameof(CreateFilterDataTestData))]
        public void CreateFilterDataTest(Object filter, Type filterInputType, Type filterOutputType)
        {
            processCreateFilterDataDefinition.MakeGenericMethod(filterInputType, filterOutputType).Invoke(this, new[] { filter });
        }

        [Theory]
        [MemberData(nameof(FilterDataEqualityTestData))]
        public void FilterDataEqualityTest(FilterData firstData, FilterData secondData, Boolean expectedResult)
        {
            Assert.Equal(expectedResult, Equals(firstData, secondData));
            Assert.Equal(expectedResult, Equals(firstData.GetHashCode(), secondData.GetHashCode()));
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessCreateFilterDataTest<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            FilterData data = FilterData.Create(filter);
            Assert.Equal(typeof(IFilter<TFilterInput, TFilterOutput>), data.FilterType);
            Assert.Equal(typeof(TFilterInput), data.InputType);
            Assert.Equal(typeof(TFilterOutput), data.OutputType);
        }

        [Theory]
        [MemberData(nameof(FilterDataExecuteFilterTestData))]
        public void FilterDataExecuteFilterTest(FilterData data, Boolean shouldSucceed, Object filterInput, Object expectedFilterOutput, Type expectedExceptionType)
        {
            if (shouldSucceed)
                Assert.Equal(expectedFilterOutput, data.ExecuteFilter(filterInput));
            else
                Assert.Throws(expectedExceptionType, () => data.ExecuteFilter(filterInput));
        }

        [Fact]
        public void CreateFilterDataNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => FilterData.Create<Object, Object>(null));
        }
    }
}