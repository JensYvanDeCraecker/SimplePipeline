namespace SimplePipeline.Builder
{
    public static class Pipeline
    {
        public static IPipelineBuilder<TInput, TOutput> ToBuilder<TInput, TOutput>(this IPipeline<TInput, TOutput> pipeline)
        {
            return new PipelineBuilder<TInput, TOutput>(pipeline);
        }
    }
}