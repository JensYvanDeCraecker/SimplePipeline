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

The current version is 1.0.0, installing it is possible by using NuGet package manager console or download the library directly from [GitHub](https://github.com/JensYvanDeCraecker/SimplePipeline/releases).

```
PM> Install-Package SimplePipeline -Version 1.0.0
```

### Documentation

Documentation and examples for this code can be found on the [Wiki page](https://github.com/JensYvanDeCraecker/SimplePipeline/wiki).

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JensYvanDeCraecker/SimplePipeline/tags).

## Authors

* **Jens Yvan De Craecker** - *Initial development* - [GitHub](https://github.com/JensYvanDeCraecker/)

## License

This project is licensed under the MIT [License](LICENSE.txt).
