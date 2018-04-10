using System;
using System.Collections;
using System.Collections.Generic;
using SimplePipeline.Tests.Filters;

namespace SimplePipeline.Tests.Pipelines
{
    public class CountElementsPipeline<T> : IPipeline<IEnumerable<T>, Int32>
    {
        private readonly IPipeline<IEnumerable<T>, Int32> innerPipeline;

        public CountElementsPipeline()
        {
            innerPipeline = new Pipeline<IEnumerable<T>, Int32>(new FilterSequence()
            {
                new EnumerableToArrayPipeline<T>(),
                new EnumerableCountFilter<T>()
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

        public Int32 Output
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