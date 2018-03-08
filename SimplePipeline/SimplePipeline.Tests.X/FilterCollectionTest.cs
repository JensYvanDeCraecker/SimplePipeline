using System;
using Xunit;

namespace SimplePipeline.Tests.X
{
    public class FilterCollectionTest
    {
        [Fact]
        public void CanCreatePipelineParametersNull()
        {
            FilterCollection sequence = new FilterCollection();
            Assert.False(sequence.CanCreatePipeline(null, typeof(Object)));
            Assert.False(sequence.CanCreatePipeline(typeof(Object), null));
            Assert.False(sequence.CanCreatePipeline(null, null));
        }
    }
}