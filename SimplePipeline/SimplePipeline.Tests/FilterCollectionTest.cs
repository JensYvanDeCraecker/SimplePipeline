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

        public void AddFilter<TInput, TOutput>(FilterCollection collection, IFilter<TInput, TOutput> filter)
        {
            collection.Add(filter);
        }

        public void FillSequence(IEnumerable<Object> items)
        {
            try
            {
                FilterCollection collection = new FilterCollection();
                foreach (Object item in items)
                    if (item is Tuple<Object, Type, Type> tuple)
                        addFilterDefenition.MakeGenericMethod(tuple.Item2, tuple.Item3).Invoke(this, new[] { collection, tuple.Item1 });
                    else
                        collection.Add((FilterData)item);
            }
            catch (TargetInvocationException e)
            {
                // ReSharper disable once PossibleNullReferenceException
                throw e.InnerException;
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PossibleFilterCollectionData))]
        public void PossibleFilterCollection(IEnumerable<Object> items)
        {
            Assert.DoesNotThrow(() =>
            {
                FillSequence(items);
            });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.UnpossibleFilterCollectionData))]
        public void UnpossibleFilterCollection(IEnumerable<Object> items)
        {
            Assert.Throws<InvalidFilterException>(() =>
            {
                FillSequence(items);
            });
        }
    }
}