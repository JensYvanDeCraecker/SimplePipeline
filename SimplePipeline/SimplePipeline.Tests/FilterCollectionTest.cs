using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterCollectionTest
    {
        private readonly MethodInfo addFilterDefenition = typeof(FilterCollectionTest).GetMethod("AddFilter");
        private readonly Type filterType = typeof(IFilter<,>);

        public void AddFilter<TInput, TOutput>(FilterCollection collection, IFilter<TInput, TOutput> filter)
        {
            collection.Add(filter);
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PossibleFilterCollectionData))]
        public void PossibleFilterCollection(IEnumerable<Object> items)
        {
            FilterCollection collection = new FilterCollection();
            Assert.DoesNotThrow(() =>
            {
                foreach (Object item in items)
                    if (item is Tuple<Object, Type, Type> tuple)
                        addFilterDefenition.MakeGenericMethod(tuple.Item2, tuple.Item3).Invoke(this, new[] { collection, tuple.Item1 });
                    else
                        collection.Add((FilterData)item);
            });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.UnpossibleFilterCollectionData))]
        public void UnpossibleFilterCollection(IEnumerable<Object> items)
        {
            FilterCollection collection = new FilterCollection();
            Assert.Throws<InvalidFilterException>(() =>
            {
                try
                {
                    foreach (Object item in items)
                        if (item is Tuple<Object, Type, Type> tuple)
                            addFilterDefenition.MakeGenericMethod(tuple.Item2, tuple.Item3).Invoke(this, new[] { collection, tuple.Item1 });
                        else
                            collection.Add((FilterData)item);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException ?? new Exception();
                }
            });
        }
    }
}