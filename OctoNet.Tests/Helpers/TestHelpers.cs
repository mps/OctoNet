using RestSharp;
using Method = OctoNet.Web.Method;

namespace OctoNet.Tests.Helpers
{
    public static class TestHelpers
    {
        public static GitHubRequestAsyncHandle CreateTestHandle()
        {
            return new GitHubRequestAsyncHandle(new GitHubRequest("foo", API.v3, Method.GET),
                                                new RestRequestAsyncHandle());
        }
    }
}