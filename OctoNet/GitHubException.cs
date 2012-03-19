using System;
using OctoNet.Utility;
using OctoNet.Web;

namespace OctoNet
{
    public class GitHubException : Exception
    {
        private readonly ErrorType _errorType;
        private readonly IGitHubResponse _response;

        public GitHubException(IGitHubResponse response,
                               ErrorType errorType)
            : base(string.Empty, response.ErrorException)
        {
            Requires.ArgumentNotNull(response, "response");

            _response = response;
            _errorType = errorType;
        }

        public IGitHubResponse Response
        {
            get { return _response; }
        }

        public ErrorType ErrorType
        {
            get { return _errorType; }
        }
    }
}