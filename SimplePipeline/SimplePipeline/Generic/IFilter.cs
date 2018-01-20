namespace SimplePipeline.Generic
{
    public interface IFilter<in TInput, out TOutput> : IFilter
    {
        TOutput Execute(TInput input);
    }
}