using System;
using NUnit.Framework;

namespace SimplePipeline.Tests
{
    [TestFixture]
    public class FilterTest
    {
        [Test]
        public void ToFilterFuncNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((Func<String, String>)null).ToFilter());
        }

        [Test]
        public void ToFilterPipelineNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((IPipeline<String, String>)null).ToFilter());
        }
    }
}