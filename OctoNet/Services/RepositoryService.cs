﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using OctoNet.Models;
using OctoNet.Utility;
using OctoNet.Web;

namespace OctoNet.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IGitHubClient _client;

        public RepositoryService(IGitHubClient gitHubClient)
        {
            Requires.ArgumentNotNull(gitHubClient, "gitHubClient");

            _client = gitHubClient;
        }

        #region IRepositoryService Members

        public GitHubRequestAsyncHandle GetRepositoryAsync(string user,
                                                           string repo,
                                                           Action<Repository> callback,
                                                           Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");

            string resource = string.Format("/repos/{0}/{1}", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.GET);
            return _client.CallApiAsync<Repository>(request,
                                                    r => callback(r.Data),
                                                    onError);
        }

        public GitHubRequestAsyncHandle GetRepositoriesAsync(string user,
                                                             int page,
                                                             Action<IEnumerable<Repository>> callback,
                                                             Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");

            string resource = string.Format("/users/{0}/repos", user);
            return GetRepositoriesAsyncInternal(resource, page, callback, onError);
        }

        public GitHubRequestAsyncHandle GetWatchedRepositoriesAsync(string user,
                                                                    int page,
                                                                    Action<IEnumerable<Repository>> callback,
                                                                    Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");

            string resource = string.Format("/users/{0}/watched", user);
            return GetRepositoriesAsyncInternal(resource, page, callback, onError);
        }

        public GitHubRequestAsyncHandle ForkAsync(string user,
                                                  string repo,
                                                  Action<Repository> callback,
                                                  Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");

            // TODO: Allow for forking into an Org...
            string resource = string.Format("/repos/{0}/{1}/forks", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.POST);
            return _client.CallApiAsync<Repository>(request,
                                                    r => callback(r.Data),
                                                    onError);
        }

        public GitHubRequestAsyncHandle GetForksAsync(string user,
                                                      string repo,
                                                      int page,
                                                      Action<IEnumerable<Repository>> callback,
                                                      Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");

            string resource = string.Format("/repos/{0}/{1}/forks", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.GET);
            return GetRepositoriesAsyncInternal(resource, page, callback, onError);
        }

        public GitHubRequestAsyncHandle WatchAsync(string user,
                                                   string repo,
                                                   Action callback,
                                                   Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");

            string resource = string.Format("/user/watched/{0}/{1}", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.PUT);
            return _client.CallApiAsync<object>(request,
                                                r => callback(),
                                                onError);
        }

        public GitHubRequestAsyncHandle UnwatchAsync(string user,
                                                     string repo,
                                                     Action callback,
                                                     Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");

            string resource = string.Format("/user/watched/{0}/{1}", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.DELETE);
            return _client.CallApiAsync<object>(request,
                                                r => callback(),
                                                onError);
        }

        public GitHubRequestAsyncHandle IsWatchingAsync(string user,
                                                        string repo,
                                                        Action<bool> callback,
                                                        Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");


            string resource = string.Format("/user/watched/{0}/{1}", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.GET);

            return _client.CallApiAsync<object>(
                request,
                r =>
                    {
                        Debug.Assert(false, "all responses should be errors");
                        callback(true);
                    },
                e =>
                    {
                        if (e.Response.StatusCode == HttpStatusCode.NoContent)
                        {
                            callback(true);
                            return;
                        }

                        if (e.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            callback(false);
                            return;
                        }

                        onError(e);
                    });
        }

        public GitHubRequestAsyncHandle GetBranchesAsync(string user,
                                                         string repo,
                                                         int page,
                                                         Action<IEnumerable<Branch>> callback,
                                                         Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");

            string resource = string.Format("/repos/{0}/{1}/branches", user, repo);
            var request = new GitHubRequest(resource, API.v3, Method.GET);
            return _client.CallApiAsync<List<Branch>>(request,
                                                      r => callback(r.Data),
                                                      onError);
        }

        public GitHubRequestAsyncHandle GetCommitAsync(string user,
                                                       string repo,
                                                       string sha,
                                                       Action<Commit> callback,
                                                       Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");
            Requires.ArgumentNotNull(sha, "sha");

            string resource = string.Format("/repos/{0}/{1}/commits/{2}", user, repo, sha);
            var request = new GitHubRequest(resource,
                                            API.v3,
                                            Method.GET);
            return _client.CallApiAsync<Commit>(request,
                                                r => callback(r.Data),
                                                onError);
        }

        public GitHubRequestAsyncHandle GetCommitsAsync(string user,
                                                        string repo,
                                                        string branch,
                                                        int page,
                                                        Action<IEnumerable<Commit>> callback,
                                                        Action<GitHubException> onError)
        {
            Requires.ArgumentNotNull(user, "user");
            Requires.ArgumentNotNull(repo, "repo");
            Requires.ArgumentNotNull(branch, "branch");
            Requires.IsTrue(page > 0, "page");

            string resource = string.Format("/repos/{0}/{1}/commits", user, repo);
            var request = new GitHubRequest(resource,
                                            API.v3,
                                            Method.GET,
                                            Parameter.Page(page),
                                            Parameter.Sha(branch));
            return _client.CallApiAsync<List<Commit>>(request,
                                                      r => callback(r.Data),
                                                      onError);
        }

        #endregion

        private GitHubRequestAsyncHandle GetRepositoriesAsyncInternal(
            string resource,
            int page,
            Action<IEnumerable<Repository>> callback,
            Action<GitHubException> onError)
        {
            var request = new GitHubRequest(resource,
                                            API.v3,
                                            Method.GET,
                                            Parameter.Page(page));
            return _client.CallApiAsync<List<Repository>>(request,
                                                          r => callback(r.Data),
                                                          onError);
        }
    }
}