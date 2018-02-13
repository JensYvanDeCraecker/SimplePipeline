using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    public class TestData
    {
        public static IReadOnlyList<Object> Filters
        {
            get
            {
                return new ReadOnlyCollection<Object>(new List<Object>()
                {
                    ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), // 0
                    ((Func<String, String>)(input => input.ToUpper())).ToFilter(), // 1
                    ((Func<String, String>)(input => input.ToLower())).ToFilter(), // 2
                    ((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter(), // 3
                    ((Func<String, Int32>)(input => input.Length)).ToFilter(), // 4
                    ((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter(), // 5
                    ((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter(), // 6
                    ((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter(), // 7
                    ((Func<IEnumerable<Char>, Char>)(input => input.GroupBy(character => character).OrderByDescending(characterGroups => characterGroups.Count()).First().Key)).ToFilter(), // 8
                    ((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter(), // 9
                    ((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter(), // 10
                    ((Func<Tuple<String, Int32, Int32>, String>)(input => input.Item1.Substring(input.Item2, input.Item3))).ToFilter(), // 11
                    ((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException() : false)).ToFilter(), // 12
                    ((Func<Object, String>)(input => input.ToString())).ToFilter(), // 13
                    ((Func<Object, Int32>)(input => input.GetHashCode())).ToFilter(), // 14
                    ((Func<Boolean, Boolean>)(input => !input)).ToFilter() // 15
                });
            }
        }

        public static IEnumerable<TestCaseData> PipelineSuccessData
        {
            get
            {
                yield return new TestCaseData(new FilterCollection()
                {
                    (IFilter<Tuple<String, Int32, Int32>, String>)Filters[11],
                    (IFilter<String, String>)Filters[1],
                    (IFilter<String, String>)Filters[0]
                }, typeof(Tuple<String, Int32, Int32>), typeof(String), Tuple.Create("SimplePipeline is an easy to use pipeline system.", 0, 4), "PMIS");
            }
        }

        public static IEnumerable<TestCaseData> PipelineFailureData
        {
            get
            {
                yield return new TestCaseData(new FilterCollection()
                {
                    (IFilter<Boolean, Boolean>)Filters[15],
                    (IFilter<Boolean, Boolean>)Filters[12]
                }, typeof(Boolean), typeof(Boolean), false, typeof(ArgumentException));
            }
        }

        public static IEnumerable<TestCaseData> PossibleFilterCollectionData
        {
            get
            {
                yield return new TestCaseData(new List<Tuple<Object, Type, Type>>()
                {
                    Tuple.Create(Filters[0], typeof(String), typeof(String)), // ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()
                    Tuple.Create(Filters[1], typeof(String), typeof(String)), // ((Func<String, String>)(input => input.ToUpper())).ToFilter()
                    Tuple.Create(Filters[6], typeof(String), typeof(Char[])), // ((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter()
                    Tuple.Create(Filters[8], typeof(IEnumerable<Char>), typeof(Char)) // ((Func<IEnumerable<Char>, Char>)(input => input.GroupBy(character => character).OrderByDescending(characterGroups => characterGroups.Count()).First().Key)).ToFilter()
                });
                yield return new TestCaseData(new List<Tuple<Object, Type, Type>>()
                {
                    Tuple.Create(Filters[5], typeof(String), typeof(Boolean)), // ((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter()
                    Tuple.Create(Filters[13], typeof(Object), typeof(String)) // ((Func<Object, String>)(input => input.ToString())).ToFilter()
                });
                yield return new TestCaseData(new List<Tuple<Object, Type, Type>>()
                {
                    Tuple.Create(Filters[10], typeof(IEnumerable<Object>), typeof(List<Object>)), // ((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter()
                    Tuple.Create(Filters[13], typeof(Object), typeof(String)), // ((Func<Object, String>)(input => input.ToString())).ToFilter()
                    Tuple.Create(Filters[14], typeof(Object), typeof(Int32)) // ((Func<Object, Int32>)(input => input.GetHashCode())).ToFilter()
                });
            }
        }

        public static IEnumerable<TestCaseData> UnpossibleFilterCollectionData
        {
            get
            {
                yield return new TestCaseData(new List<Tuple<Object, Type, Type>>()
                {
                    Tuple.Create(Filters[6], typeof(String), typeof(IEnumerable<Char>)), // ((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter()
                    Tuple.Create(Filters[10], typeof(IEnumerable<Object>), typeof(List<Object>)) // ((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter()
                });
                yield return new TestCaseData(new List<Tuple<Object, Type, Type>>()
                {
                    Tuple.Create(Filters[13], typeof(Object), typeof(String)), // ((Func<Object, String>)(input => input.ToString())).ToFilter()
                    Tuple.Create(Filters[12], typeof(Boolean), typeof(Boolean)) // ((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException() : false)).ToFilter()
                });
                yield return new TestCaseData(new List<Tuple<Object, Type, Type>>()
                {
                    Tuple.Create(Filters[9], typeof(List<String>), typeof(IEnumerable<String>)), // ((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter()
                    Tuple.Create(Filters[9], typeof(List<String>), typeof(IEnumerable<String>)) // ((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter()
                });
            }
        }

        public static IEnumerable<TestCaseData> FilterDataEqualityData
        {
            get
            {
                yield return new TestCaseData(Filters[0], typeof(String), typeof(String)); // ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()
                yield return new TestCaseData(Filters[1], typeof(String), typeof(String)); // ((Func<String, String>)(input => input.ToUpper())).ToFilter()
                yield return new TestCaseData(Filters[2], typeof(String), typeof(String)); // ((Func<String, String>)(input => input.ToLower())).ToFilter()
                yield return new TestCaseData(Filters[3], typeof(String), typeof(Object)); // ((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter()
                yield return new TestCaseData(Filters[4], typeof(String), typeof(Int32)); // ((Func<String, Int32>)(input => input.Length)).ToFilter()
                yield return new TestCaseData(Filters[5], typeof(String), typeof(Boolean)); // ((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter()
                yield return new TestCaseData(Filters[6], typeof(String), typeof(IEnumerable<Char>)); // ((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter()
                yield return new TestCaseData(Filters[7], typeof(Int32[]), typeof(Double)); // ((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter()
                yield return new TestCaseData(Filters[8], typeof(List<Char>), typeof(Char)); // ((Func<IEnumerable<Char>, Char>)(input => input.GroupBy(character => character).OrderByDescending(characterGroups => characterGroups.Count()).First().Key)).ToFilter()
                yield return new TestCaseData(Filters[9], typeof(List<String>), typeof(IEnumerable<String>)); // ((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter()
                yield return new TestCaseData(Filters[10], typeof(IEnumerable<Object>), typeof(List<Object>)); // ((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter()
                yield return new TestCaseData(Filters[11], typeof(Tuple<String, Int32, Int32>), typeof(String)); //  ((Func<Tuple<String, Int32, Int32>, String>)(input => input.Item1.Substring(input.Item2, input.Item3))).ToFilter()
                yield return new TestCaseData(Filters[12], typeof(Boolean), typeof(Boolean)); // ((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException() : false)).ToFilter()
                yield return new TestCaseData(Filters[13], typeof(Object), typeof(String)); // ((Func<Object, String>)(input => input.ToString())).ToFilter()
                yield return new TestCaseData(Filters[14], typeof(Object), typeof(Int32)); // ((Func<Object, Int32>)(input => input.GetHashCode())).ToFilter()
                yield return new TestCaseData(Filters[15], typeof(Boolean), typeof(Boolean)); // ((Func<Boolean, Boolean>)(input => !input)).ToFilter()
            }
        }

        public static IEnumerable<TestCaseData> CompareFilterTypeData
        {
            get
            {
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[0])); // ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[1])); // ((Func<String, String>)(input => input.ToUpper())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[2])); // ((Func<String, String>)(input => input.ToLower())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<String, Object>)Filters[3])); // ((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<String, Int32>)Filters[4])); // ((Func<String, Int32>)(input => input.Length)).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<String, Boolean>)Filters[5])); // ((Func<String, Boolean>)String.IsNullOrWhiteSpace).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<String, IEnumerable<Char>>)Filters[6])); // ((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<Int32[], Double>)Filters[7])); // ((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<List<Char>, Char>)Filters[8])); // ((Func<IEnumerable<Char>, Char>)(input => input.GroupBy(character => character).OrderByDescending(characterGroups => characterGroups.Count()).First().Key)).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<List<String>, IEnumerable<String>>)Filters[9])); // ((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<IEnumerable<Object>, List<Object>>)Filters[10])); // ((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<Tuple<String, Int32, Int32>, String>)Filters[11])); //  ((Func<Tuple<String, Int32, Int32>, String>)(input => input.Item1.Substring(input.Item2, input.Item3))).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<Boolean, Boolean>)Filters[12])); // ((Func<Boolean, Boolean>)(input => input ? throw new ArgumentException() : false)).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<Object, String>)Filters[13])); // ((Func<Object, String>)(input => input.ToString())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<Object, Int32>)Filters[14])); // ((Func<Object, Int32>)(input => input.GetHashCode())).ToFilter()
                yield return new TestCaseData(FilterData.Create((IFilter<Boolean, Boolean>)Filters[15])); // ((Func<Boolean, Boolean>)(input => !input)).ToFilter()
            }
        }
    }
}