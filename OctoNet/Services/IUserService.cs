using System;
using System.Collections.Generic;
using OctoNet.Models;

namespace OctoNet.Services
{
    public interface IUserService
    {
        GitHubRequestAsyncHandle GetUserAsync(string user,
                                              Action<User> callback,
                                              Action<GitHubException> onError);

        GitHubRequestAsyncHandle GetAuthenticatedUserAsync(Action<User> callback,
                                                           Action<GitHubException> onError);

        GitHubRequestAsyncHandle GetFollowersAsync(string user,
                                                   int page,
                                                   Action<IEnumerable<User>> callback,
                                                   Action<GitHubException> onError);

        GitHubRequestAsyncHandle GetFollowingAsync(string user,
                                                   int page,
                                                   Action<IEnumerable<User>> callback,
                                                   Action<GitHubException> onError);

        GitHubRequestAsyncHandle FollowAsync(string user,
                                             Action callback,
                                             Action<GitHubException> onError);

        GitHubRequestAsyncHandle UnfollowAsync(string user,
                                               Action callback,
                                               Action<GitHubException> onError);

        GitHubRequestAsyncHandle IsFollowingAsync(string user,
                                                  Action<bool> callback,
                                                  Action<GitHubException> onError);

        GitHubRequestAsyncHandle GetWatchersAsync(string user,
                                                  string repo,
                                                  int page,
                                                  Action<IEnumerable<User>> callback,
                                                  Action<GitHubException> onError);
    }
}