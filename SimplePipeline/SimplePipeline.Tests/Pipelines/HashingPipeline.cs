using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimplePipeline.Builder;
using SimplePipeline.Tests.Filters;

namespace SimplePipeline.Tests.Pipelines
{
    public class HashingPipeline : PipelineConcept<String, String>
    {
        protected override IPipelineBuilder<String, String> Configure(IPipelineBuilder<String, String> pipelineBuilder)
        {
            return pipelineBuilder.Chain(new TrimFilter()).Chain(Encoding.Unicode.GetBytes, new ComputeHashFilter(), Convert.ToBase64String);
        }
    }
}