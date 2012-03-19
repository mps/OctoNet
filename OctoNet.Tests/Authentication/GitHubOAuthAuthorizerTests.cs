using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using OctoNet.Authentication;
using OctoNet.Helpers;
using OctoNet.Web;
using RestSharp;
using Method = RestSharp.Method;

namespace OctoNet.Tests.Authentication
{
    [TestFixture]
    public class GitHubOAuthAuthorizerTests
    {
        private readonly RestRequestAsyncHandle _testHandle = new RestRequestAsyncHandle();

        private GitHubOAuthAuthorizer CreateAuthorizer(IRestClientFactory factory = null,
                                                       IResponseProcessor processor = null)
        {
            return new GitHubOAuthAuthorizer(factory ?? new Mock<IRestClientFactory>(MockBehavior.Strict).Object,
                                             processor ?? new Mock<IResponseProcessor>(MockBehavior.Strict).Object);
        }

        [Test]
        public void BuildAuthenticationUrl_WithAScopeDefined()
        {
            string clientId = "foo";
            string redirectUrl = "bar";
            var scopes = new[] {Scope.PublicRepo};
            string expectedUrl = string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}",
                                               Constants.AuthenticationUrl,
                                               clientId,
                                               redirectUrl,
                                               "public_repo");
            var auth = new GitHubOAuthAuthorizer();

            string actualUrl = auth.BuildAuthenticationUrl(clientId, redirectUrl, scopes);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [Test]
        public void BuildAuthenticationUrl_WithDuplicateScopesDefined()
        {
            string clientId = "foo";
            string redirectUrl = "bar";
            var scopes = new[] {Scope.PublicRepo, Scope.PublicRepo};
            string expectedUrl = string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}",
                                               Constants.AuthenticationUrl,
                                               clientId,
                                               redirectUrl,
                                               "public_repo");
            var auth = new GitHubOAuthAuthorizer();

            string actualUrl = auth.BuildAuthenticationUrl(clientId, redirectUrl, scopes);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [Test]
        public void BuildAuthenticationUrl_WithMultipleScopesDefined()
        {
            string clientId = "foo";
            string redirectUrl = "bar";
            var scopes = new[] {Scope.PublicRepo, Scope.Repo, Scope.Gists, Scope.User};
            string expectedUrl = string.Format("{0}?client_id={1}&redirect_uri={2}&scope={3}",
                                               Constants.AuthenticationUrl,
                                               clientId,
                                               redirectUrl,
                                               "public_repo,repo,gists,user");
            var auth = new GitHubOAuthAuthorizer();

            string actualUrl = auth.BuildAuthenticationUrl(clientId, redirectUrl, scopes);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [Test]
        public void BuildAuthenticationUrl_WithNoScopesDefined()
        {
            string clientId = "foo";
            string redirectUrl = "bar";
            string expectedUrl = string.Format("{0}?client_id={1}&redirect_uri={2}",
                                               Constants.AuthenticationUrl,
                                               clientId,
                                               redirectUrl);
            var auth = new GitHubOAuthAuthorizer();

            string actualUrl = auth.BuildAuthenticationUrl(clientId, redirectUrl);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [Test]
        public void GetAccessTokenAsync_ShouldAddClientIdToRequestParameters()
        {
            string expectedClientId = "id";
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Parameters.Any(p => p.Name == "client_id" &&
                                                                                               (string) p.Value ==
                                                                                               expectedClientId)),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Verifiable();
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object);

            auth.GetAccessTokenAsync(expectedClientId, "bar", "baz", s => { }, e => { });

            mockClient.Verify();
        }

        [Test]
        public void GetAccessTokenAsync_ShouldAddClientSecretToRequestParameters()
        {
            string expectedClientSecret = "secret";
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(
                c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Parameters.Any(p => p.Name == "client_secret" &&
                                                                                  (string) p.Value ==
                                                                                  expectedClientSecret)),
                                    It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Verifiable();
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object);

            auth.GetAccessTokenAsync("foo", expectedClientSecret, "baz", s => { }, e => { });

            mockClient.Verify();
        }

        [Test]
        public void GetAccessTokenAsync_ShouldAddCodeToRequestParameters()
        {
            string expectedCode = "code";
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Parameters.Any(p => p.Name == "code" &&
                                                                                               (string) p.Value ==
                                                                                               expectedCode)),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Verifiable();
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object);

            auth.GetAccessTokenAsync("foo", "bar", expectedCode, s => { }, e => { });

            mockClient.Verify();
        }

        [Test]
        public void GetAccessTokenAsync_ShouldBuildRestClient_WithBaseAuthorizationUrl()
        {
            string expectedBaseUrl = Constants.AuthorizeUrl;
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(expectedBaseUrl))
                .Returns(mockClient.Object)
                .Verifiable();
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object);

            auth.GetAccessTokenAsync("foo", "bar", "baz", s => { }, e => { });

            mockFactory.Verify();
        }

        [Test]
        public void GetAccessTokenAsync_ShouldBuildRestRequest_WithExpectedResource()
        {
            string expectedResource = "/access_token";
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object)
                .Verifiable();
            mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Resource == expectedResource),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object);

            auth.GetAccessTokenAsync("foo", "bar", "baz", s => { }, e => { });

            mockClient.Verify();
        }

        [Test]
        public void GetAccessTokenAsync_ShouldCallCallback_IfRequestSucceeds()
        {
            bool callbackCalled = false;
            string responseContent = "access_token=something";
            GitHubException ex = null;
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<RestRequest, Action<RestResponse, RestRequestAsyncHandle>>(
                    (req, c) => c(new RestResponse {Content = responseContent}, _testHandle));
            mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<GitHubResponse>(), out ex))
                .Returns(false);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object, mockProcessor.Object);

            auth.GetAccessTokenAsync("foo", "bar", "baz", t => callbackCalled = true, e => { });

            Assert.IsTrue(callbackCalled);
        }

        [Test]
        public void GetAccessTokenAsync_ShouldCallOnError_IfRequestFails()
        {
            bool onErrorCalled = false;
            GitHubException ex = null;
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<RestRequest, Action<RestResponse, RestRequestAsyncHandle>>(
                    (req, c) => c(new RestResponse(), _testHandle));
            mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<GitHubResponse>(), out ex))
                .Returns(true);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object, mockProcessor.Object);

            auth.GetAccessTokenAsync("foo", "bar", "baz", s => { }, e => onErrorCalled = true);

            Assert.IsTrue(onErrorCalled);
        }

        [Test]
        public void GetAccessTokenAsync_ShouldPassAccessTokenToCallback_IfRequestSucceeds()
        {
            string expectedToken = "token";
            string responseContent = string.Format("access_token={0}&other_stuff=some%20crap", expectedToken);
            GitHubException ex = null;
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<RestRequest, Action<RestResponse, RestRequestAsyncHandle>>(
                    (req, c) => c(new RestResponse {Content = responseContent}, _testHandle));
            mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<GitHubResponse>(), out ex))
                .Returns(false);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object, mockProcessor.Object);

            string actualToken = null;
            auth.GetAccessTokenAsync("foo", "bar", "baz", t => actualToken = t, e => { });

            Assert.AreEqual(expectedToken, actualToken);
        }

        [Test]
        public void GetAccessTokenAsync_ShouldPassExpectedError_IfRequestFails()
        {
            var expectedException = new GitHubException(new GitHubResponse(new RestResponse()), ErrorType.Unknown);
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object);
            mockClient.Setup(c => c.ExecuteAsync(It.IsAny<RestRequest>(),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<RestRequest, Action<RestResponse, RestRequestAsyncHandle>>(
                    (req, c) => c(new RestResponse(), _testHandle));
            mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<GitHubResponse>(), out expectedException))
                .Returns(true);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object, mockProcessor.Object);

            GitHubException actualException = null;
            auth.GetAccessTokenAsync("foo", "bar", "baz", s => { }, e => actualException = e);

            Assert.AreSame(expectedException, actualException);
        }

        [Test]
        public void GetAccessTokenAsync_ShouldPostRestRequest()
        {
            var mockClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockClient.Object)
                .Verifiable();
            mockClient.Setup(c => c.ExecuteAsync(It.Is<RestRequest>(r => r.Method == Method.POST),
                                                 It.IsAny<Action<RestResponse, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            GitHubOAuthAuthorizer auth = CreateAuthorizer(mockFactory.Object);

            auth.GetAccessTokenAsync("foo", "bar", "baz", s => { }, e => { });

            mockClient.Verify();
        }
    }
}