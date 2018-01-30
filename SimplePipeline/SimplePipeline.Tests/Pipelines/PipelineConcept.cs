using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplePipeline.Builder
{
    public abstract class PipelineConcept<TPipelineInput, TPipelineOutput> : IPipeline<TPipelineInput, TPipelineOutput>
    {
        private readonly IPipeline<TPipelineInput, TPipelineOutput> innerPipeline;

        protected PipelineConcept()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            innerPipeline = Configure(new PipelineBuilder<TPipelineInput>()).Build();
        }

        protected abstract IPipelineBuilder<TPipelineInput, TPipelineOutput> Configure(IPipelineBuilder<TPipelineInput, TPipelineInput> pipelineBuilder);

        public IEnumerator<Object> GetEnumerator()
        {
            return innerPipeline.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TPipelineOutput Output
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

        public Boolean Execute(TPipelineInput input)
        {
            return innerPipeline.Execute(input);
        }

        public void Reset()
        {
            innerPipeline.Reset();
        }
    }
}