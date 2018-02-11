using System;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterTest
    {
        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.ExecuteFilterData))]
        public TOutput ExecuteFilter<TInput, TOutput>(IFilter<TInput, TOutput> filter, TInput input)
        {
            try
            {
                return filter.Execute(input);
            }
            catch (Exception)
            {
                return default(TOutput);
            }
        }
    }
}