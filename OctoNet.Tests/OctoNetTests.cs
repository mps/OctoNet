using NUnit.Framework;
using Shouldly;

namespace OctoNet.Tests
{
    [TestFixture]
    public class OctoNetTests
    {
        [Test]
        public void New()
        {
            var octoNet = new OctoNet();
            octoNet.GetType().ShouldBe(typeof(OctoNet));
        }
    }
}