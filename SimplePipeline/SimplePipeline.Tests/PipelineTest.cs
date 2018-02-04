using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        public static IEnumerable<TestCaseData> ExecutePipelineData
        {
            get
            {
                yield return new TestCaseData(new Pipeline<String, String>()
                {
                    ((Func<String, String>)(input => input.ToUpper())).ToFilter(),
                    ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(),
                    ((Func<String, String>)(input => input.Substring(0, 4))).ToFilter()
                }, "SimplePipeline is an easy to use pipeline system.").Returns(".MET");
                yield return new TestCaseData(new Pipeline<String, Char>()
                {
                    ((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter(),
                    ((Func<IEnumerable<IGrouping<Char, Char>>, Char>)(input => input.OrderByDescending(group => group.Count()).First().Key)).ToFilter()
                }, "SimplePipeline is an easy to use pipeline system.").Returns('e');
                yield return new TestCaseData(new Pipeline<String, Int32>()
                {
                    ((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter(),
                    ((Func<IEnumerable<IGrouping<Char, Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter()
                }, "SimplePipeline is an easy to use pipeline system.").Returns(8);
                yield return new TestCaseData(new Pipeline<IEnumerable<Int32>, Double>()
                {
                    ((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter(),
                    ((Func<Double, Double>)Math.Round).ToFilter()
                }, new[] { 7, 45, 78, 98, 12, 14 }).Returns(42);
                yield return new TestCaseData(new Pipeline<String, String>()
                {
                    ((Func<String, String>)(input => !String.IsNullOrEmpty(input) ? input : throw new ArgumentNullException(nameof(input)))).ToFilter()
                }, null).Returns(null);
            }
        }

        [Test]
        [TestCaseSource(nameof(ExecutePipelineData))]
        public TOutput ExecutePipeline<TInput, TOutput>(IPipeline<TInput, TOutput> pipeline, TInput input)
        {
            Assert.IsTrue(pipeline.IsBeginState);
            TOutput output = default(TOutput);
            if (pipeline.Execute(input))
            {
                output = pipeline.Output;
                pipeline.Reset();
                Assert.IsTrue(pipeline.IsBeginState);
                Assert.DoesNotThrow(() => pipeline.ToFilter().Execute(input));
            }
            else
            {
                Assert.IsNotNull(pipeline.Exception);
                Exception exception = pipeline.Exception;
                Assert.IsFalse(pipeline.IsBeginState);
                pipeline.Reset();
                Assert.IsTrue(pipeline.IsBeginState);
                Assert.Throws(exception.GetType(), () => pipeline.ToFilter().Execute(input));
            }
            return output;
        }
    }
}