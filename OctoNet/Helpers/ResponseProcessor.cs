using System.Net;
using OctoNet.Utility;
using OctoNet.Web;

namespace OctoNet.Helpers
{
    public interface IResponseProcessor
    {
        bool TryProcessResponseErrors(IGitHubResponse response,
                                      out GitHubException exception);
    }

    public class ResponseProcessor : IResponseProcessor
    {
        #region IResponseProcessor Members

        public bool TryProcessResponseErrors(IGitHubResponse response,
                                             out GitHubException exception)
        {
            Requires.ArgumentNotNull(response, "response");

            if (response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Created)
            {
                exception = null;
                return false;
            }

            ErrorType errorType = ErrorType.Unknown;
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                errorType = ErrorType.NoNetwork;
            }
            else if (response.StatusCode == HttpStatusCode.BadGateway)
            {
                errorType = ErrorType.ServerError;
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                errorType = ErrorType.ApiLimitExceeded;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                errorType = ErrorType.ResourceNotFound;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                errorType = ErrorType.Unauthorized;
            }
            // TODO: Other error types

            exception = new GitHubException(response, errorType);
            return true;
        }

        #endregion
    }
}