using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimplePipeline.Example.Models;

namespace SimplePipeline.Example.Filters
{
    public class ParsePersonsFilter : IFilter<String, IEnumerable<Person>>
    {
        public IEnumerable<Person> Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return JsonConvert.DeserializeObject<IEnumerable<Person>>(input);
        }
    }
}