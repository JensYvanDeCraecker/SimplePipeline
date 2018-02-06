using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class PipelineTest
    {
        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                yield return new TestCaseData(new Pipeline<String, String>(((Func<String, String>)(input => input.ToUpper())).ToFilter().GetData(), ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter().GetData(), ((Func<String, String>)(input => input.Substring(0, 4))).ToFilter().GetData()) { }, "SimplePipeline is an easy to use pipeline system.").Returns(".MET");
                yield return new TestCaseData(new Pipeline<String, Char>(((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter().GetData(), ((Func<IEnumerable<IGrouping<Char, Char>>, Char>)(input => input.OrderByDescending(group => group.Count()).First().Key)).ToFilter().GetData()) { }, "SimplePipeline is an easy to use pipeline system.").Returns('e');
                yield return new TestCaseData(new Pipeline<String, Int32>(((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter().GetData(), ((Func<IEnumerable<IEnumerable<Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter().GetData()) { }, "SimplePipeline is an easy to use pipeline system.").Returns(8);
                yield return new TestCaseData(new Pipeline<IEnumerable<Int32>, Double>(((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter().GetData(), ((Func<Double, Double>)Math.Round).ToFilter().GetData()) { }, new[] { 7, 45, 78, 98, 12, 14 }).Returns(42);
                yield return new TestCaseData(new Pipeline<String, String>(((Func<String, String>)(input => !String.IsNullOrEmpty(input) ? input : throw new ArgumentNullException(nameof(input)))).ToFilter().GetData()), null).Returns(null);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
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
                Type exceptionType = pipeline.Exception.GetType();
                Assert.IsFalse(pipeline.IsBeginState);
                pipeline.Reset();
                Assert.IsTrue(pipeline.IsBeginState);
                Assert.Throws(exceptionType, () => pipeline.ToFilter().Execute(input));
            }
            return output;
        }
    }
}