namespace SimplePipeline.Builder
{
    public interface IPipelineBuilder
    {
        IPipelineSection Start();
    }

    public interface IPipelineBuilder<in TPipelineInputBase> : IPipelineBuilder
    {
        IPipelineSection<TPipelineInput, TPipelineInput> Start<TPipelineInput>() where TPipelineInput : TPipelineInputBase;
    }
}