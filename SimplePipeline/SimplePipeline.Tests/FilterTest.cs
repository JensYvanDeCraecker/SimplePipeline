using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterTest
    {
        public static IEnumerable ExecuteFilterData
        {
            get
            {
                yield return new TestCaseData(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), "Jens Yvan De Craecker").Returns("rekcearC eD navY sneJ");
                yield return new TestCaseData(((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter(), "Jens Yvan De Craecker").Returns("JensYvanDeCraecker");
                yield return new TestCaseData(((Func<String, String>)(input => input.ToLower())).ToFilter(), "Jens Yvan De Craecker").Returns("jens yvan de craecker");
                yield return new TestCaseData(((Func<String, String>)(input => input.Substring(0, 4))).ToFilter(), "Jens Yvan De Craecker").Returns("Jens");
            }
        }

        [Test]
        [TestCaseSource(nameof(ExecuteFilterData))]
        public String ExecuteFilter(IFilter<String, String> filter, String input)
        {
            return filter.Execute(input);
        }
    }
}