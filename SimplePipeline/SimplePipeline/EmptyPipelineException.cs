using System;

namespace SimplePipeline
{
    public class EmptyPipelineException : Exception
    {
        public Type PipelineInputType { get; }

        public Type PipelineOutputType { get; }

        public EmptyPipelineException(String message, Type pipelineInputType, Type pipelineOutputType) : base(message)
        {
            PipelineInputType = pipelineInputType;
            PipelineOutputType = pipelineOutputType;
        }
    }
}