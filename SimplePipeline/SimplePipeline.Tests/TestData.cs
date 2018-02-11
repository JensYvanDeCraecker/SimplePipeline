using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    public class TestData
    {
        public static IEnumerable<TestCaseData> PipelineSuccessData
        {
            get
            {
                yield return new TestCaseData(new FilterCollection() { ((Func<String, String>)(input => input.ToUpper())).ToFilter(), ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), ((Func<String, String>)(input => input.Substring(0, 4))).ToFilter() }, typeof(String), typeof(IEnumerable<Char>), "SimplePipeline is an easy to use pipeline system.", ".MET".AsEnumerable());
                yield return new TestCaseData(new FilterCollection() { ((Func<Int32, Double>)(input => Math.Sqrt(input))).ToFilter() }, typeof(Int32), typeof(Double), 4, 2);
                yield return new TestCaseData(new FilterCollection(), typeof(Int32), typeof(Int32), 16, 16);
            }
        }

        public static IEnumerable<TestCaseData> PipelineFailureData
        {
            get
            {
                yield return new TestCaseData(new FilterCollection() { ((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter(), ((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException("Empty string") : false)).ToFilter() }, typeof(String), typeof(Boolean), "    ");
            }
        }

        public static IEnumerable<TestCaseData> ExecuteFilterData
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

        public static IEnumerable<TestCaseData> BuildCollectionData
        {
            get
            {
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, String>)(input => input.ToUpper())).ToFilter()), FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()), FilterData.Create(((Func<String, String>)(input => input.Substring(0, 4))).ToFilter()) }, true);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, IEnumerable<Char>>)(input => input.ToCharArray())).ToFilter()), FilterData.Create(((Func<Char[], Int32>)(input => input.Length)).ToFilter()) }, false);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter()), FilterData.Create(((Func<IEnumerable<Char>, Int32>)(input => input.Count())).ToFilter()) }, true);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter()), FilterData.Create(((Func<Double, Int32>)(input => (Int32)input)).ToFilter()) }, false);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<String, String>)(input => input.Trim())).ToFilter()), FilterData.Create(((Func<IEnumerable<Char>, String>)(input => new String(input.ToArray()))).ToFilter()) }, true);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<Int32, IEnumerable<Boolean>>)(input => new Boolean[input])).ToFilter()), FilterData.Create(((Func<Boolean[], Boolean>)(input => input.All(value => value))).ToFilter()) }, false);
                yield return new TestCaseData(new List<FilterData>() { FilterData.Create(((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter()), FilterData.Create(((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter()) }, true);
            }
        }

        public static IEnumerable<TestCaseData> FilterDataEqualityData
        {
            get
            {
                yield return new TestCaseData(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), typeof(String), typeof(String));
                yield return new TestCaseData(((Func<String, Int32>)(input => input.Length)).ToFilter(), typeof(String), typeof(Int32));
                yield return new TestCaseData(((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter(), typeof(Int32[]), typeof(Double));
                yield return new TestCaseData(((Func<Double, Double>)Math.Round).ToFilter(), typeof(Double), typeof(Double));
                yield return new TestCaseData(((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter(), typeof(String), typeof(IEnumerable<IEnumerable<Char>>));
                yield return new TestCaseData(((Func<IEnumerable<IGrouping<Char, Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter(), typeof(IEnumerable<IGrouping<Char, Char>>), typeof(Int32));
                yield return new TestCaseData(((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter(), typeof(IEnumerable<String>), typeof(List<String>));
                yield return new TestCaseData(((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter(), typeof(IEnumerable<Object>), typeof(List<Object>));
                yield return new TestCaseData(((Func<String, IEnumerable<Char>>)(input => input.ToCharArray())).ToFilter(), typeof(String), typeof(IEnumerable<Char>));
                yield return new TestCaseData(((Func<Char[], Int32>)(input => input.Length)).ToFilter(), typeof(Char[]), typeof(Int32));
                yield return new TestCaseData(((Func<Int32, IEnumerable<Boolean>>)(input => new Boolean[input])).ToFilter(), typeof(Int32), typeof(IEnumerable<Boolean>));
                yield return new TestCaseData(((Func<Boolean[], Boolean>)(input => input.All(value => value))).ToFilter(), typeof(Boolean[]), typeof(Boolean));
            }
        }

        public static IEnumerable<TestCaseData> CompareFilterTypeData
        {
            get
            {
                yield return new TestCaseData(FilterData.Create(((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<String, Int32>)(input => input.Length)).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<Double, Double>)Math.Round).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<String, IEnumerable<IGrouping<Char, Char>>>)(input => input.GroupBy(character => character))).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<IEnumerable<IGrouping<Char, Char>>, Int32>)(input => input.OrderByDescending(group => group.Count()).First().Count())).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<String, IEnumerable<Char>>)(input => input.ToCharArray())).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<Char[], Int32>)(input => input.Length)).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<Int32, IEnumerable<Boolean>>)(input => new Boolean[input])).ToFilter()));
                yield return new TestCaseData(FilterData.Create(((Func<Boolean[], Boolean>)(input => input.All(value => value))).ToFilter()));
            }
        }
    }
}