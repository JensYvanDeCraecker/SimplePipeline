namespace SimplePipeline.Builder
{
    public static class Pipeline
    {
        public static IPipelineBuilder<TInput, TOutput> ToBuilder<TInput, TOutput>(this IPipeline<TInput, TOutput> pipeline)
        {
            return new PipelineBuilder<TInput, TOutput>(pipeline);
        }

        public static IPipelineBuilder ToBuilder(this IPipeline pipeline)
        {
            return new PipelineBuilder<object, object>(pipeline);
        }
    }
}