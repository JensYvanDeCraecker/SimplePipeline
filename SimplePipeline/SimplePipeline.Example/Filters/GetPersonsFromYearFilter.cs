using System;
using System.Collections.Generic;
using System.Linq;
using SimplePipeline.Example.Models;

namespace SimplePipeline.Example.Filters
{
    public class GetPersonsFromYearFilter : IFilter<IEnumerable<Person>, IEnumerable<Person>>
    {
        private readonly Int32 year;

        public GetPersonsFromYearFilter(Int32 year)
        {
            this.year = year;
        }

        public IEnumerable<Person> Execute(IEnumerable<Person> input)
        {
            return input.Where(person => person.BirthDate.Year == year);
        }
    }
}