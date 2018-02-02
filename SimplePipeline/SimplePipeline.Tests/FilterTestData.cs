using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    public class FilterTestData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData((Func<String, String>)(input => new String(input.Reverse().ToArray())), "Jens Yvan De Craecker").Returns("rekcearC eD navY sneJ");
                yield return new TestCaseData((Func<String, String>)(input => input.Replace(" ", "")), "Jens Yvan De Craecker").Returns("JensYvanDeCraecker");
                yield return new TestCaseData((Func<String, String>)(input => input.ToLower()), "Jens Yvan De Craecker").Returns("jens yvan de craecker");
                yield return new TestCaseData((Func<String, String>)(input => input.Substring(0, 4)), "Jens Yvan De Craecker").Returns("Jens");
            }
        }
    }
}