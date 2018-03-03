using System;
using System.Collections.Generic;
using System.Reflection;
using SimplePipeline.Tests.Filters;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterDataTest
    {
        private readonly MethodInfo processCreateFilterDataDefinition = typeof(FilterDataTest).GetMethod("ProcessCreateFilterData", BindingFlags.NonPublic | BindingFlags.Instance);

        // Syntax: IFilter<in TInput, out TOutput>, TInput type, TOutput type
        public static IEnumerable<Object[]> Filters
        {
            get
            {
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(IEnumerable<Char>), typeof(String) };
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(IEnumerable<Char>), typeof(IEnumerable<Char>) };
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(String), typeof(String) };
                yield return new Object[] { new CharEnumerableToStringFilter(), typeof(String), typeof(IEnumerable<Char>) };
            }
        }

        // Syntax: FilterData, FilterData, Boolean
        public static IEnumerable<Object[]> FilterDataPairs
        {
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
        public static IEnumerable<Object[]> FilterDatas
        {
            get
            {
                yield return new Object[] { FilterData.Create(new CharEnumerableToStringFilter()), true, new[] { 'S', 'i', 'm', 'p', 'l', 'e', 'P', 'i', 'p', 'e', 'l', 'i', 'n', 'e' }, "SimplePipeline", null };
                yield return new Object[] { FilterData.Create(((Func<Object, Object>)(input => throw new Exception())).ToFilter()), false, null, null, typeof(Exception) };
            }
        }

        [Theory]
        [MemberData(nameof(Filters))]
        public void CreateFilterData(Object filter, Type filterInputType, Type filterOutputType)
        {
            processCreateFilterDataDefinition.MakeGenericMethod(filterInputType, filterOutputType).Invoke(this, new[] { filter });
        }

        [Theory]
        [MemberData(nameof(FilterDataPairs))]
        public void FilterDataEquality(FilterData firstData, FilterData secondData, Boolean expectedResult)
        {
            Assert.Equal(expectedResult, Equals(firstData, secondData));
            Assert.Equal(expectedResult, Equals(firstData.GetHashCode(), secondData.GetHashCode()));
        }

        // ReSharper disable once UnusedMember.Local
        private void ProcessCreateFilterData<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            FilterData data = FilterData.Create(filter);
            Assert.Equal(typeof(IFilter<TFilterInput, TFilterOutput>), data.FilterType);
            Assert.Equal(typeof(TFilterInput), data.InputType);
            Assert.Equal(typeof(TFilterOutput), data.OutputType);
        }

        [Theory]
        [MemberData(nameof(FilterDatas))]
        public void FilterDataExecuteFilter(FilterData data, Boolean shouldSucceed, Object filterInput, Object expectedFilterOutput, Type expectedExceptionType)
        {
            if (shouldSucceed)
                Assert.Equal(expectedFilterOutput, data.ExecuteFilter(filterInput));
            else
                Assert.Throws(expectedExceptionType, () => data.ExecuteFilter(filterInput));
        }

        [Fact]
        public void CreateFilterDataNull()
        {
            Assert.Throws<ArgumentNullException>(() => FilterData.Create<Object, Object>(null));
        }
    }
}