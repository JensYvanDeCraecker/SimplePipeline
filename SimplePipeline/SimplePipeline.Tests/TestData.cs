using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    public class TestData
    {
        public static IReadOnlyList<Object> Filters { get; } = new ReadOnlyCollection<Object>(new List<Object>()
        {
            ((Func<String, String>)(input => new String(input.Reverse().ToArray()))).ToFilter(), // 0
            ((Func<String, String>)(input => input.ToUpper())).ToFilter(), // 1
            ((Func<String, String>)(input => input.ToLower())).ToFilter(), // 2
            ((Func<String, String>)(input => input.Replace(" ", ""))).ToFilter(), // 3
            ((Func<String, Int32>)(input => input.Length)).ToFilter(), // 4
            ((Func<String, Boolean>) String.IsNullOrWhiteSpace).ToFilter(), // 5
            ((Func<String, Char[]>)(input => input.ToCharArray())).ToFilter(), // 6
            ((Func<IEnumerable<Int32>, Double>)(input => input.Average())).ToFilter(), // 7
            ((Func<IEnumerable<Char>, Char>)(input => input.GroupBy(character => character).OrderByDescending(characterGroups => characterGroups.Count()).First().Key)).ToFilter(), // 8
            ((Func<IEnumerable<String>, List<String>>)(input => input.ToList())).ToFilter(), // 9
            ((Func<IEnumerable<Object>, List<Object>>)(input => input.ToList())).ToFilter(), // 10
            ((Func<Tuple<String, Int32, Int32>, String>)(input => input.Item1.Substring(input.Item2, input.Item3))).ToFilter(), // 11
            ((Func<Boolean, Boolean>)(input => input? throw new ArgumentException() : false)).ToFilter(), // 12
            ((Func<Object, String>)(input => input.ToString())).ToFilter(), // 13
            ((Func<Object, Int32>)(input => input.GetHashCode())).ToFilter(), // 14
            ((Func<Boolean, Boolean>)(input => !input)).ToFilter() // 15
        });

        public static IEnumerable<TestCaseData> PipelineInvalidSequenceData
        {
            get
            {
                yield return new TestCaseData(new FilterCollection()
                {
                    (IFilter<Boolean, Boolean>)Filters[15],
                    (IFilter<Boolean, Boolean>)Filters[12]
                }, typeof(Object), typeof(Boolean));
                yield return new TestCaseData(new FilterCollection()
                {
                    (IFilter<Tuple<String, Int32, Int32>, String>)Filters[11],
                    (IFilter<String, String>)Filters[1],
                    (IFilter<String, Object>)Filters[0]
                }, typeof(Tuple<String, Int32, Int32>), typeof(String));
                yield return new TestCaseData(new FilterCollection(), typeof(String), typeof(String));
            }
        }

        public static IEnumerable<TestCaseData> PipelineSuccessfulData
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

        public static IEnumerable<TestCaseData> PossibleSequenceData
        {
            get
            {
                yield return new TestCaseData(new List<Object>()
                {
                    Tuple.Create(Filters[0], typeof(String), typeof(String)),
                    Tuple.Create(Filters[1], typeof(String), typeof(String)),
                    FilterData.Create((IFilter<String, Char[]>)Filters[6]),
                    Tuple.Create(Filters[8], typeof(IEnumerable<Char>), typeof(Char))
                });
                yield return new TestCaseData(new List<Object>()
                {
                    Tuple.Create(Filters[5], typeof(String), typeof(Boolean)),
                    Tuple.Create(Filters[13], typeof(Object), typeof(String))
                });
                yield return new TestCaseData(new List<Object>()
                {
                    FilterData.Create((IFilter<IEnumerable<Object>, List<Object>>)Filters[10]),
                    Tuple.Create(Filters[13], typeof(Object), typeof(String)),
                    Tuple.Create(Filters[14], typeof(Object), typeof(Int32))
                });
            }
        }

        public static IEnumerable<TestCaseData> UnpossibleSequenceData
        {
            get
            {
                yield return new TestCaseData(new List<Object>()
                {
                    FilterData.Create((IFilter<String, IEnumerable<Char>>)Filters[6]),
                    Tuple.Create(Filters[10], typeof(IEnumerable<Object>), typeof(List<Object>))
                });
                yield return new TestCaseData(new List<Object>()
                {
                    Tuple.Create(Filters[13], typeof(Object), typeof(String)),
                    Tuple.Create(Filters[12], typeof(Boolean), typeof(Boolean))
                });
                yield return new TestCaseData(new List<Object>()
                {
                    Tuple.Create(Filters[9], typeof(List<String>), typeof(IEnumerable<String>)),
                    Tuple.Create(Filters[9], typeof(List<String>), typeof(IEnumerable<String>))
                });
            }
        }

        public static IEnumerable<TestCaseData> FilterDataEqualityData
        {
            get
            {
                yield return new TestCaseData(Filters[0], typeof(String), typeof(String));
                yield return new TestCaseData(Filters[1], typeof(String), typeof(String));
                yield return new TestCaseData(Filters[2], typeof(String), typeof(String));
                yield return new TestCaseData(Filters[3], typeof(String), typeof(Object));
                yield return new TestCaseData(Filters[4], typeof(String), typeof(Int32));
                yield return new TestCaseData(Filters[5], typeof(String), typeof(Boolean));
                yield return new TestCaseData(Filters[6], typeof(String), typeof(IEnumerable<Char>));
                yield return new TestCaseData(Filters[7], typeof(Int32[]), typeof(Double));
                yield return new TestCaseData(Filters[8], typeof(List<Char>), typeof(Char));
                yield return new TestCaseData(Filters[9], typeof(List<String>), typeof(IEnumerable<String>));
                yield return new TestCaseData(Filters[10], typeof(IEnumerable<Object>), typeof(List<Object>));
                yield return new TestCaseData(Filters[11], typeof(Tuple<String, Int32, Int32>), typeof(String));
                yield return new TestCaseData(Filters[12], typeof(Boolean), typeof(Boolean));
                yield return new TestCaseData(Filters[13], typeof(Object), typeof(String));
                yield return new TestCaseData(Filters[14], typeof(Object), typeof(Int32));
                yield return new TestCaseData(Filters[15], typeof(Boolean), typeof(Boolean));
            }
        }

        public static IEnumerable<TestCaseData> CompareFilterTypeData
        {
            get
            {
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[0]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[1]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[2]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, Object>)Filters[3]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, Int32>)Filters[4]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, Boolean>)Filters[5]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, IEnumerable<Char>>)Filters[6]));
                yield return new TestCaseData(FilterData.Create((IFilter<Int32[], Double>)Filters[7]));
                yield return new TestCaseData(FilterData.Create((IFilter<List<Char>, Char>)Filters[8]));
                yield return new TestCaseData(FilterData.Create((IFilter<List<String>, IEnumerable<String>>)Filters[9]));
                yield return new TestCaseData(FilterData.Create((IFilter<IEnumerable<Object>, List<Object>>)Filters[10]));
                yield return new TestCaseData(FilterData.Create((IFilter<Tuple<String, Int32, Int32>, String>)Filters[11]));
                yield return new TestCaseData(FilterData.Create((IFilter<Boolean, Boolean>)Filters[12]));
                yield return new TestCaseData(FilterData.Create((IFilter<Object, String>)Filters[13]));
                yield return new TestCaseData(FilterData.Create((IFilter<Object, Int32>)Filters[14]));
                yield return new TestCaseData(FilterData.Create((IFilter<Boolean, Boolean>)Filters[15]));
            }
        }

        public static IEnumerable<TestCaseData> FilterDataEqualsTrueData
        {
            get
            {
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[0]), FilterData.Create((IFilter<String, String>)Filters[0]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[1]), FilterData.Create((IFilter<String, String>)Filters[1]));
            }
        }

        public static IEnumerable<TestCaseData> FilterDataEqualsFalseData
        {
            get
            {
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[0]), FilterData.Create((IFilter<String, IEnumerable<Char>>)Filters[0]));
                yield return new TestCaseData(FilterData.Create((IFilter<String, String>)Filters[1]), FilterData.Create((IFilter<String, Object>)Filters[1]));
            }
        }
    }

}