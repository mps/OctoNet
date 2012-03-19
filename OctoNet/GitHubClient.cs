﻿using System;
using OctoNet.Authentication;
using OctoNet.Helpers;
using OctoNet.Services;
using OctoNet.Utility;
using OctoNet.Web;
using RestSharp;

namespace OctoNet
{
    public class GitHubClient : IGitHubClient
    {
        private readonly IRestClientFactory _factory;
        private readonly IIssueService _issues;
        private readonly IOrganizationService _organizations;
        private readonly IResponseProcessor _processor;
        private readonly IPullRequestService _pullRequests;
        private readonly IRepositoryService _repositories;
        private readonly IUserService _users;

        private IAuthenticator _authenticator;

        public GitHubClient()
            : this(new RestClientFactory(), new ResponseProcessor())
        {
        }

        public GitHubClient(IRestClientFactory factory,
                            IResponseProcessor processor)
        {
            Requires.ArgumentNotNull(factory, "factory");
            Requires.ArgumentNotNull(processor, "processor");

            _factory = factory;
            _processor = processor;

            Authenticator = new NullAuthenticator();
            _users = new UserService(this);
            _issues = new IssueService(this);
            _repositories = new RepositoryService(this);
            _pullRequests = new PullRequestService(this);
            _organizations = new OrganizationService(this);
        }

        #region IGitHubClient Members

        public IUserService Users
        {
            get { return _users; }
        }

        public IOrganizationService Organizations
        {
            get { return _organizations; }
        }

        public IIssueService Issues
        {
            get { return _issues; }
        }

        public IRepositoryService Repositories
        {
            get { return _repositories; }
        }

        public IPullRequestService PullRequests
        {
            get { return _pullRequests; }
        }

        public IAuthenticator Authenticator
        {
            get { return _authenticator; }
            set { _authenticator = value ?? new NullAuthenticator(); }
        }

        public GitHubRequestAsyncHandle CallApiAsync<TResponseData>(
            GitHubRequest request,
            Action<IGitHubResponse<TResponseData>> callback,
            Action<GitHubException> onError) where TResponseData : new()
        {
            Requires.ArgumentNotNull(request, "request");
            
            var restRequest = new RestRequest
                                  {
                                      Resource = request.Resource,
                                      Method = request.Method.ToRestSharpMethod(),
                                      RequestFormat = DataFormat.Json
                                  };
            foreach (var p in request.Parameters)
            {
                restRequest.AddParameter(p.Name, p.Value);
            }

            if (request.Body != null)
            {
                restRequest.AddBody(request.Body);
            }

            var baseUrl = (request.Version == API.v3) ? Constants.ApiV3Url : Constants.ApiV2Url;
            var restClient = _factory.CreateRestClient(baseUrl);
            restClient.Authenticator = Authenticator;

            var handle = restClient.ExecuteAsync<TResponseData>(
                restRequest,
                (r, h) =>
                {
                    var response = new GitHubResponse<TResponseData>(r);

                    GitHubException ex;
                    if (_processor.TryProcessResponseErrors(response, out ex))
                    {
                        if (onError != null)
                            onError(ex);
                        return;
                    }

                    if (callback != null)
                        callback(response);
                });

            return new GitHubRequestAsyncHandle(request, handle);
        }

        #endregion
    }
}