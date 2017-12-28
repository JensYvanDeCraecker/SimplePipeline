namespace SimplePipeline
{
    public interface IFilter<in TFilterInput, out TFilterOutput>
    {
        TFilterOutput Execute(TFilterInput input);
    }
}