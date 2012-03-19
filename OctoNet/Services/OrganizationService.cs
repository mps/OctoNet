using System;
using System.Collections.Generic;
using OctoNet.Models;
using OctoNet.Utility;
using OctoNet.Web;

namespace OctoNet.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IGitHubClient _client;

        public OrganizationService(IGitHubClient gitHubClient)
        {
            Requires.ArgumentNotNull(gitHubClient, "gitHubClient");

            _client = gitHubClient;
        }

        #region IOrganizationService Members

        public GitHubRequestAsyncHandle GetMembersAsync(string organization,
                                                        int page,
                                                        Action<IEnumerable<User>> callback,
                                                        Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(organization, "organization");

            string resource = string.Format("/orgs/{0}/members", organization);
            var request = new GitHubRequest(resource,
                                            API.v3,
                                            Method.GET,
                                            Parameter.Page(page));
            return _client.CallApiAsync<List<User>>(request,
                                                    r => callback(r.Data),
                                                    onError);
        }

        public GitHubRequestAsyncHandle GetOrganizationsAsync(string user,
                                                              int page,
                                                              Action<IEnumerable<User>> callback,
                                                              Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");

            string resource = string.Format("/users/{0}/orgs", user);
            var request = new GitHubRequest(resource,
                                            API.v3,
                                            Method.GET,
                                            Parameter.Page(page));
            return _client.CallApiAsync<List<User>>(request,
                                                    r => callback(r.Data),
                                                    onError);
        }

        #endregion
    }
}