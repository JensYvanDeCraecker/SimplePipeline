using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimplePipeline.Tests.Filters;

namespace SimplePipeline.Tests.Pipelines
{
    public class HashingPipeline : IPipeline<String, Byte[]>
    {
        private readonly IPipeline<String, Byte[]> innerPipeline;

        public HashingPipeline()
        {
            innerPipeline = new Pipeline<String, Byte[]>()
            {
                new TrimFilter(),
                new EncodingFilter(Encoding.Unicode),
                new ComputeHashFilter()
            };
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return innerPipeline.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)innerPipeline).GetEnumerator();
        }

        public Byte[] Output
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