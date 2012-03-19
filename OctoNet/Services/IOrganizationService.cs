using System;
using System.Collections.Generic;
using OctoNet.Models;

namespace OctoNet.Services
{
    public interface IOrganizationService
    {
        GitHubRequestAsyncHandle GetMembersAsync(string organization,
                                                 int page,
                                                 Action<IEnumerable<User>> callback,
                                                 Action<GitHubException> onError);

        GitHubRequestAsyncHandle GetOrganizationsAsync(string user,
                                                       int page,
                                                       Action<IEnumerable<User>> callback,
                                                       Action<GitHubException> onError);
    }
}