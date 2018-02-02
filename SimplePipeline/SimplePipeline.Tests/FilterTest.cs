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
        public static IEnumerable<TestCaseData> ExecuteFilterData
        {
            get
            {
                yield return new TestCaseData(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), "Jens Yvan De Craecker").Returns("rekcearC eD navY sneJ");
                yield return new TestCaseData(((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter(), "Jens Yvan De Craecker").Returns("JensYvanDeCraecker");
                yield return new TestCaseData(((Func<String, String>)(input => input.ToLower())).ToFilter(), "Jens Yvan De Craecker").Returns("jens yvan de craecker");
                yield return new TestCaseData(((Func<String, String>)(input => input.Substring(0, 4))).ToFilter(), "Jens Yvan De Craecker").Returns("Jens");
                yield return new TestCaseData(((Func<String, String>)(input => input?.Substring(0, 4))).ToFilter(), null).Returns(null);
                yield return new TestCaseData(((Func<String, String>)(input => Convert.ToBase64String(new SHA256CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(input))))).ToFilter(), "Jens Yvan De Craecker").Returns("lTeckkBgmzBUOz8vc+hsBCZKSwJWdnPpP+d92vbo1os=");
                yield return new TestCaseData(((Func<String, String>)(input => input.ToUpper())).ToFilter(), "Jens Yvan De Craecker").Returns("JENS YVAN DE CRAECKER");
                yield return new TestCaseData(((Func<String, Int32>)(input => input.Length)).ToFilter(), "Jens Yvan De Craecker").Returns(21);
            }
        }

        [Test]
        [TestCaseSource(nameof(ExecuteFilterData))]
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