# SimplePipeline

SimplePipeline is an easy to use pipeline system. Our code uses custom interfaces making it highly extensible.

## Getting Started

### What is a pipeline?

A pipeline is a sequence of code that is independent of each other. These bits of code take an input, process the input and returns an output. This means that the input in the pipeline gets passed into the first bit of code that processes it and returns an output that the following code takes as an input.

![Pipeline example](http://tomasp.net/articles/parallel-extra-image-pipeline/pipeline.png)

### Advantages

#### Reuse code

Because code acts independently in a pipeline design, code from another pipeline can be used again and again in other pipelines. This means you will be saving time on writing the same thing.

### Demo

Pipeline creation is based on method chaining called a Fluent API, this makes it easy to read.

```csharp
IPipeline<String, Byte[]> demoPipeline = PipelineBuilder.Create<String, Byte[]>(builder => builder.Chain(new TrimFilter()).Chain(((Func<String, Byte[]>)(input => Encoding.Unicode.GetBytes(input))).ToFilter()).Chain(new HashingFilter()));
```
