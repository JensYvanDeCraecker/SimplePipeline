# SimplePipeline

SimplePipeline is an easy to use pipeline system. Our code uses custom interfaces making it highly extensible.

## Introduction

### What is a pipeline?

A pipeline is a sequence of code that is independent of each other. These bits of code take an input, process the input and returns an output. This means that the input in the pipeline gets passed into the first bit of code that processes it and returns an output that the following code takes as an input.

![Pipeline example](http://tomasp.net/articles/parallel-extra-image-pipeline/pipeline.png)

### Advantages

#### Reuse code

Because code acts independently in a pipeline design, code from another pipeline can be used again and again in other pipelines. This means you will be saving time on writing the same thing.

## Getting Started

### Installing

Once the project has been successfully tested and used in example cases to prove that the concept works en all provided implementations of the interfaces works exactly expected, the project will be released en hosted on NuGet. Currently, the project is still under development.

### Demo

#### Creating filters

```cs
public class FileReadFilter : IFilter<String, String>, IFilter<String, String[]>, IFilter<String, Byte[]>, IFilter<String, FileStream>
{
	String IFilter<String, String>.Execute(String input)
	{
		if (input == null)
			throw new ArgumentNullException(nameof(input));
		return File.ReadAllText(input);
	}

	String[] IFilter<String, String[]>.Execute(String input)
	{
		if (input == null)
			throw new ArgumentNullException(nameof(input));
		return File.ReadAllLines(input);
	}

	Byte[] IFilter<String, Byte[]>.Execute(String input)
	{
		if (input == null)
			throw new ArgumentNullException(nameof(input));
		return File.ReadAllBytes(input);
	}

	FileStream IFilter<String, FileStream>.Execute(String input)
	{
		if (input == null)
			throw new ArgumentNullException(nameof(input));
		return File.OpenRead(input);
	}
}
```

```cs
public class JsonFilter<T> : IFilter<String, T>
{
	public T Execute(String input)
	{
		if (input == null)
			throw new ArgumentNullException(nameof(input));
		return JsonConvert.DeserializeObject<T>(input);
	}
}
```

#### Creating pipelines

```cs
public class DeserializePipeline<T> : IPipeline<String, T>
{
	private readonly IPipeline<String, T> innerPipeline;

	public DeserializePipeline()
	{
		innerPipeline = new Pipeline<String, T>()
		{
			(IFilter<String, String>)new FileReadFilter(),
			new JsonFilter<T>()
		};
	}

	public IEnumerator<FilterData> GetEnumerator()
	{
		return innerPipeline.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)innerPipeline).GetEnumerator();
	}

	public T Output
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

	public Boolean Execute(String input)
	{
		return innerPipeline.Execute(input);
	}

	public void Reset()
	{
		innerPipeline.Reset();
	}
}
```

#### Using pipelines

```cs
IPipeline<String, String[]> pipeline = new DeserializePipeline<String[]>();
if (pipeline.Execute("File path here..."))
	foreach (String line in pipeline.Output)
		Console.WriteLine(line);
else
	Console.WriteLine(pipeline.Exception.Message);
Console.ReadKey();
```

#### Using PipelineBuilder

Because pipelines can contain filters where the output type of the previous filter doesn't match the input type of the following filter, failures can occur.

```cs
IPipeline<String, String[]> pipeline = new Pipeline<String, String[]>()
{
	(IFilter<String, Byte[]>)new FileReadFilter(), // Pipeline fails
	new JsonFilter<String[]>()
};
```

To solve this, we've created the `IPipelineBuilder<in TPipelineInput, out TPipelineOutput>` interface that provides methods for chaining filters at compile time.

```cs
IPipeline<String, String[]> pipeline = PipelineBuilder.Start<String>().Chain<String>(new FileReadFilter()).Chain(new JsonFilter<String[]>()).Build();
```

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JensYvanDeCraecker/SimplePipeline/tags). 


## Authors

* **Jens Yvan De Craecker** - *Initial development* - [GitHub](https://github.com/JensYvanDeCraecker/)

## License

This project is licensed under the MIT [License](LICENSE).
