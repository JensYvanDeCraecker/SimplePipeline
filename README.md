# SimplePipeline

SimplePipeline is an easy to use pipelining system. Our code uses custom interfaces making it highly extensible.

## Getting Started

### Demo

Pipeline creation is based around method chaining called a Fluent API, this makes it easy to read.

```csharp
IPipeline<String, Byte[]> demoPipelineLambda = PipelineBuilder.Create<String, Byte[]>(builder => builder.Chain(new TrimFilter()).Chain(((Func<String, Byte[]>)(input => Encoding.Unicode.GetBytes(input))).ToFilter()).Chain(new HashingFilter()));
IPipeline<String, Byte[]> demoPipeline = new PipelineBuilder<String>().Chain(new TrimFilter()).Chain(((Func<String, Byte[]>)(input => Encoding.Unicode.GetBytes(input))).ToFilter()).Chain(new HashingFilter()).Build();
```
