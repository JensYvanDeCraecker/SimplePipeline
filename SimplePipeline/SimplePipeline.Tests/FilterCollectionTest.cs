using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
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

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.PossibleFilterCollectionData))]
        public void PossibleFilterCollection(IEnumerable<Tuple<Object, Type, Type>> tuples)
        {
            FilterCollection collection = new FilterCollection();
            Assert.DoesNotThrow(() =>
            {
                foreach (Tuple<Object, Type, Type> tuple in tuples)
                {
                    addFilterDefenition.MakeGenericMethod(tuple.Item2, tuple.Item3).Invoke(this, new[] { collection,tuple.Item1 });
                }
            });
        }


        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.UnpossibleFilterCollectionData))]
        public void UnpossibleFilterCollection(IEnumerable<Tuple<Object, Type, Type>> tuples)
        {
            FilterCollection collection = new FilterCollection();
            TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() =>
             {
                 foreach (Tuple<Object, Type, Type> tuple in tuples)
                 {
                     addFilterDefenition.MakeGenericMethod(tuple.Item2, tuple.Item3).Invoke(this, new[] { collection,tuple.Item1 });
                 }
             });
            Assert.AreEqual(typeof(InvalidFilterException), exception.InnerException?.GetType());
        }
    }
}