using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterCollectionTest
    {
        private readonly MethodInfo addFilterDefinition = typeof(FilterCollectionTest).GetMethod("AddFilter", BindingFlags.NonPublic | BindingFlags.Instance);

        // ReSharper disable once UnusedMember.Local
        private void AddFilter<TInput, TOutput>(FilterCollection collection, IFilter<TInput, TOutput> filter)
        {
            collection.Add(filter);
        }

        private void FillSequence(IEnumerable<Object> items)
        {
            try
            {
                FilterCollection collection = new FilterCollection();
                foreach (Object item in items)
                    if (item is Tuple<Object, Type, Type> tuple)
                        addFilterDefinition.MakeGenericMethod(tuple.Item2, tuple.Item3).Invoke(this, new[] { collection, tuple.Item1 });
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
        [TestCaseSource(typeof(TestData), nameof(TestData.PossibleSequenceData))]
        public void PossibleSequence(IEnumerable<Object> items)
        {
            Assert.DoesNotThrow(() =>
            {
                FillSequence(items);
            });
        }

        [Test]
        [TestCaseSource(typeof(TestData), nameof(TestData.UnpossibleSequenceData))]
        public void UnpossibleSequence(IEnumerable<Object> items)
        {
            Assert.Throws<InvalidFilterException>(() =>
            {
                FillSequence(items);
            });
        }
    }
}