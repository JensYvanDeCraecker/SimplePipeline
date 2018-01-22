namespace SimplePipeline
{
    public interface IPipelineBuilder
    {
        IPipelineSection Start();
    }

    public interface IPipelineBuilder<TPipeInput> : IPipelineBuilder
    {
        new IPipelineSection<TPipeInput, TPipeInput> Start();
    }
}