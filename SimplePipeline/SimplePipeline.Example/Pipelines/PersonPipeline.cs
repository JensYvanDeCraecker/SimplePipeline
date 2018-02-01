using System;
using System.Collections;
using System.Collections.Generic;
using SimplePipeline.Example.Filters;
using SimplePipeline.Example.Models;

namespace SimplePipeline.Example.Pipelines
{
    public class PersonPipeline : IPipeline<String, IEnumerable<Person>>
    {
        private readonly IPipeline<String, IEnumerable<Person>> innerPipeline;

        public PersonPipeline(Int32 year)
        {
            //innerPipeline = PipelineBuilder.Create<String, IEnumerable<Person>>(builder => builder.Chain(new ReadFileFilter()).Chain(new ParsePersonsFilter()).Chain(new GetPersonsFromYearFilter(year)));
            innerPipeline = new Pipeline<String, IEnumerable<Person>>()
            {
                new ReadFileFilter(),
                new ParsePersonsFilter(),
                new GetPersonsFromYearFilter(year)
            };
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return innerPipeline.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<Person> Output
        {
            get
            {
                return innerPipeline.Output;
            }
        }

        public Exception Exception
        {
            get
            {
                return innerPipeline.Exception;
            }
        }

        public Boolean IsBeginState
        {
            get
            {
                return innerPipeline.IsBeginState;
            }
        }

        public Boolean Execute(String input)
        {
            return innerPipeline.Execute(input);
        }

        public void Reset()
        {
            innerPipeline.Reset();
        }
    }
}