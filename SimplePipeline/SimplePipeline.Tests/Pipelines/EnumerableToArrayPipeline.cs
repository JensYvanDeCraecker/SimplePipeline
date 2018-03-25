using System;
using System.Collections;
using System.Collections.Generic;
using SimplePipeline.Tests.Filters;

namespace SimplePipeline.Tests.Pipelines
{
    public class EnumerableToArrayPipeline<T> : IPipeline<IEnumerable<T>, T[]>
    {
        private readonly IPipeline<IEnumerable<T>, T[]> innerPipeline;

        public EnumerableToArrayPipeline()
        {
            innerPipeline = new Pipeline<IEnumerable<T>, T[]>(new FilterSequence()
            {
                new EnumerableToArrayFilter<T>()
            });
        }

        public IEnumerator<FilterData> GetEnumerator()
        {
            return innerPipeline.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] Output
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

        public Boolean Execute(IEnumerable<T> input)
        {
            return innerPipeline.Execute(input);
        }

        public void Reset()
        {
            innerPipeline.Reset();
        }
    }
}