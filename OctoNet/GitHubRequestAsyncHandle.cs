using OctoNet.Utility;
using RestSharp;

namespace OctoNet
{
    public class GitHubRequestAsyncHandle
    {
        private readonly RestRequestAsyncHandle _handle;
        private readonly GitHubRequest _request;

        public GitHubRequestAsyncHandle(GitHubRequest request,
                                        RestRequestAsyncHandle handle)
        {
            Requires.ArgumentNotNull(request, "request");
            Requires.ArgumentNotNull(handle, "handle");

            _request = request;
            _handle = handle;
        }

        public GitHubRequest Request
        {
            get { return _request; }
        }

        public void Abort()
        {
            _handle.Abort();
        }
    }
}