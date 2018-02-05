using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterTest
    {
        public static IEnumerable<TestCaseData> TestData
        {
            get
            {
                yield return new TestCaseData(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns(".metsys enilepip esu ot ysae na si enilepiPelpmiS");
                yield return new TestCaseData(((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns("SimplePipelineisaneasytousepipelinesystem.");
                yield return new TestCaseData(((Func<String, String>)(input => input.ToLower())).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns("simplepipeline is an easy to use pipeline system.");
                yield return new TestCaseData(((Func<String, String>)(input => input.Substring(0, 14))).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns("SimplePipeline");
                yield return new TestCaseData(((Func<String, String>)(input => input?.Substring(0, 4))).ToFilter(), null).Returns(null);
                yield return new TestCaseData(((Func<String, String>)(input => Convert.ToBase64String(new SHA256CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(input))))).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns("e1ycnKOGI560ktH4fJatgi3j8ktK0JZwxmuSEQ79hWk=");
                yield return new TestCaseData(((Func<String, String>)(input => input.ToUpper())).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns("SIMPLEPIPELINE IS AN EASY TO USE PIPELINE SYSTEM.");
                yield return new TestCaseData(((Func<String, Int32>)(input => input.Length)).ToFilter(), "SimplePipeline is an easy to use pipeline system.").Returns(49);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
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