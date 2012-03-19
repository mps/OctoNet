using OctoNet.Utility;
using RestSharp;

namespace OctoNet
{
    public class GitHubResponse<T> : GitHubResponseBase, IGitHubResponse<T>
    {
        private readonly IRestResponse<T> _response;

        public GitHubResponse(IRestResponse<T> response)
            : base(response)
        {
            Requires.ArgumentNotNull(response, "response");

            _response = response;
        }

        #region IGitHubResponse<T> Members

        public T Data
        {
            get { return _response.Data; }
        }

        #endregion
    }

    public class GitHubResponse : GitHubResponseBase, IGitHubResponse
    {
        public GitHubResponse(IRestResponse response)
            : base(response)
        {
        }
    }
}