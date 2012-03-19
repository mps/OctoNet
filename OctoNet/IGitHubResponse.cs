using System;
using System.Net;
using OctoNet.Web;

namespace OctoNet
{
    public interface IGitHubResponse<T> : IGitHubResponse
    {
        T Data { get; }
    }

    public interface IGitHubResponse
    {
        string ErrorMessage { get; }
        Exception ErrorException { get; }
        HttpStatusCode StatusCode { get; }
        ResponseStatus ResponseStatus { get; }
    }
}