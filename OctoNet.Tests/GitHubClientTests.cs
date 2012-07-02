using System;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using OctoNet.Authentication;
using OctoNet.Helpers;
using OctoNet.Web;
using RestSharp;
using Method = OctoNet.Web.Method;
using Parameter = OctoNet.Web.Parameter;

namespace OctoNet.Tests
{
    [TestFixture]
    public class GitHubClientTests
    {
        private readonly RestRequestAsyncHandle _testHandle = new RestRequestAsyncHandle();

        private GitHubClient CreateClient(IRestClientFactory factory = null,
                                          IResponseProcessor processor = null)
        {
            if (processor == null)
            {
                GitHubException ex = null;
                var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
                mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<IGitHubResponse>(),
                                                                    out ex))
                    .Returns(false);
                processor = mockProcessor.Object;
            }
            return new GitHubClient(factory ?? new Mock<IRestClientFactory>(MockBehavior.Strict).Object,
                                    processor);
        }

        [Test]
        public void Authenticator_ShouldBeAssignedToNullAuthenticator_WhenClientIsCreated()
        {
            GitHubClient client = CreateClient();

            Assert.IsInstanceOfType(typeof (NullAuthenticator), client.Authenticator);
        }

        [Test]
        public void Authenticator_ShouldDefaultToNullAuthenticator_WhenAssignedToNull()
        {
            GitHubClient client = CreateClient();

            client.Authenticator = null;

            Assert.IsInstanceOfType(typeof (NullAuthenticator), client.Authenticator);
        }

        [Test]
        public void Authenticator_ShouldTakeAssignedValue_WhenItIsNotNull()
        {
            var expectedAuthenticator = new SimpleAuthenticator(string.Empty, string.Empty, string.Empty, string.Empty);
            GitHubClient client = CreateClient();

            client.Authenticator = expectedAuthenticator;

            Assert.AreSame(expectedAuthenticator, client.Authenticator);
        }

        [Test]
        public void CallApiAsync_ShouldCallOnError_IfRestRequestDoesNotCompleteSuccessfully()
        {
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
            var response = new RestResponse<object>();
            var exception = new GitHubException(new GitHubResponse(response), ErrorType.NoNetwork);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>())).Returns(mockRestClient.Object);
            mockRestClient
                .Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(),
                                           It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<IRestRequest,
                    Action<RestResponse<object>,
                        RestRequestAsyncHandle>>((r, c) => c(response, _testHandle));
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<IGitHubResponse>(),
                                                                out exception))
                .Returns(true);
            GitHubClient client = CreateClient(mockFactory.Object, mockProcessor.Object);

            bool onErrorInvoked = false;
            client.CallApiAsync<object>(new GitHubRequest("foo", API.v3, Method.GET),
                                        o => { },
                                        e => onErrorInvoked = true);

            Assert.IsTrue(onErrorInvoked);
        }

        [Test]
        public void CallApiAsync_ShouldExecuteRestRequestWithGivenRequestMethod()
        {
            Method expectedMethod = Method.OPTIONS;
            var request = new GitHubRequest("foo/bar", API.v3, expectedMethod);
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(Constants.ApiV3Url))
                .Returns(mockRestClient.Object);
            mockRestClient.Setup(c => c.ExecuteAsync(
                It.Is<IRestRequest>(r => r.Method == expectedMethod.ToRestSharpMethod()),
                It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            GitHubClient githubClient = CreateClient(mockFactory.Object);

            githubClient.CallApiAsync<object>(request, o => { }, e => { });

            mockRestClient.VerifyAll();
        }

        [Test]
        public void CallApiAsync_ShouldExecuteRestRequestWithGivenRequestResource()
        {
            string expectedResource = "foo/bar";
            var request = new GitHubRequest(expectedResource, API.v3, Method.OPTIONS);
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(Constants.ApiV3Url))
                .Returns(mockRestClient.Object);
            mockRestClient.Setup(c => c.ExecuteAsync(
                It.Is<IRestRequest>(r => r.Resource == expectedResource),
                It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            GitHubClient githubClient = CreateClient(mockFactory.Object);

            githubClient.CallApiAsync<object>(request, o => { }, e => { });

            mockRestClient.VerifyAll();
        }

        [Test]
        public void CallApiAsync_ShouldInvokeCallBackMethod_WhenRequestHasCreatedStatus()
        {
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var response = new RestResponse<object> {StatusCode = HttpStatusCode.Created};
            mockRestClient
                .Setup(
                    c =>
                    c.ExecuteAsync(It.IsAny<IRestRequest>(),
                                   It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<IRestRequest,
                    Action<RestResponse<object>, RestRequestAsyncHandle>>((r, c) => c(response, _testHandle));
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(Constants.ApiV3Url)).Returns(mockRestClient.Object);

            GitHubClient client = CreateClient(mockFactory.Object);

            bool callbackInvoked = false;
            client.CallApiAsync<object>(
                new GitHubRequest("foo", API.v3, Method.GET),
                o => callbackInvoked = true,
                e => { });

            Assert.IsTrue(callbackInvoked);
        }

        [Test]
        public void CallApiAsync_ShouldInvokeCallbackMethod_WhenRestRequestCompletesSuccessfully()
        {
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var response = new RestResponse<object> {StatusCode = HttpStatusCode.OK};
            mockRestClient
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<IRestRequest,
                    Action<RestResponse<object>,
                        RestRequestAsyncHandle>>((r, c) => c(response, _testHandle));
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(Constants.ApiV3Url)).Returns(mockRestClient.Object);

            GitHubClient client = CreateClient(mockFactory.Object);

            bool callbackInvoked = false;
            client.CallApiAsync<object>(
                new GitHubRequest("foo", API.v3, Method.GET),
                o => callbackInvoked = true,
                e => { });

            Assert.IsTrue(callbackInvoked);
        }

        [Test]
        public void CallApiAsync_ShouldPassExceptionToOnError_IfThereAreResponseErrors()
        {
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockProcessor = new Mock<IResponseProcessor>(MockBehavior.Strict);
            var response = new RestResponse<object>();
            var expectedException = new GitHubException(new GitHubResponse(response), ErrorType.NoNetwork);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>())).Returns(mockRestClient.Object);
            mockRestClient
                .Setup(
                    c =>
                    c.ExecuteAsync(It.IsAny<IRestRequest>(),
                                   It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<IRestRequest,
                    Action<RestResponse<object>,
                        RestRequestAsyncHandle>>((r, c) => c(response, _testHandle));
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            mockProcessor.Setup(p => p.TryProcessResponseErrors(It.IsAny<IGitHubResponse>(),
                                                                out expectedException))
                .Returns(true);
            GitHubClient client = CreateClient(mockFactory.Object, mockProcessor.Object);

            GitHubException actualException = null;
            client.CallApiAsync<object>(new GitHubRequest("foo", API.v3, Method.GET),
                                        o => { },
                                        e => actualException = e);

            Assert.AreEqual(expectedException, actualException);
        }

        [Test]
        public void CallApiAsync_ShouldPassRequestParameters_ToRestRequest()
        {
            string expectedKey = "foo";
            string expectedValue = "bar";
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>())).Returns(mockRestClient.Object);
            mockRestClient
                .Setup(c => c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.Parameters.Count(p => (p.Name == expectedKey) &&
                                                                     ((string) p.Value == expectedValue)) == 1),
                    It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<IRestRequest,
                    Action<RestResponse<object>,
                        RestRequestAsyncHandle>>((r, c) => c(new RestResponse<object>(), _testHandle));
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            GitHubClient client = CreateClient(mockFactory.Object);

            var request = new GitHubRequest("resource", API.v3,
                                            Method.POST,
                                            new Parameter(expectedKey, expectedValue));
            client.CallApiAsync<object>(request, o => { }, e => { });

            mockRestClient.VerifyAll();
        }

        [Test]
        public void CallApiAsync_ShouldPassResponseDataToCallback_WhenRestRequestCompletesSuccessfully()
        {
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var expectedData = new object();
            var response = new RestResponse<object>
                               {
                                   StatusCode = HttpStatusCode.OK,
                                   Data = expectedData
                               };
            mockRestClient
                .Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(),
                                           It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle)
                .Callback<IRestRequest,
                    Action<RestResponse<object>,
                        RestRequestAsyncHandle>>((r, c) => c(response, _testHandle));
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(Constants.ApiV3Url)).Returns(mockRestClient.Object);

            GitHubClient client = CreateClient(mockFactory.Object);

            object actualData = null;
            client.CallApiAsync<object>(
                new GitHubRequest("foo", API.v3, Method.GET),
                r => actualData = r.Data,
                e => { });

            Assert.AreSame(expectedData, actualData);
        }

        [Test]
        public void CallApiAsync_ShouldUseGivenAuthenticator_DuringForRestClient()
        {
            var expectedAuthenticator = new NullAuthenticator();
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(It.IsAny<string>()))
                .Returns(mockRestClient.Object);
            mockRestClient
                .Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(),
                                           It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            mockRestClient.SetupSet(c => c.Authenticator = expectedAuthenticator)
                .Verifiable();
            GitHubClient client = CreateClient(mockFactory.Object);
            client.Authenticator = expectedAuthenticator;

            client.CallApiAsync<object>(new GitHubRequest("foo", API.v3, Method.GET),
                                        o => { },
                                        e => { });

            mockRestClient.Verify();
        }

        [Test]
        public void CallApiAsync_ShouldUseV2BaseUrl_WhenVersion2IsSpecified()
        {
            string expectedBaseUrl = Constants.ApiV3Url;
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(expectedBaseUrl))
                .Returns(mockRestClient.Object);
            mockRestClient.Setup(c => c.ExecuteAsync(
                It.IsAny<IRestRequest>(),
                It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            GitHubClient githubClient = CreateClient(mockFactory.Object);

            githubClient.CallApiAsync<object>(new GitHubRequest("foo", API.v3, Method.GET),
                                              o => { },
                                              e => { });

            mockFactory.VerifyAll();
        }

        [Test]
        public void CallApiAsync_ShouldUseV3BaseUrl_WhenVersion3IsSpecified()
        {
            string expectedBaseUrl = Constants.ApiV3Url;
            var mockRestClient = new Mock<IRestClient>(MockBehavior.Strict);
            var mockFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateRestClient(expectedBaseUrl))
                .Returns(mockRestClient.Object);
            mockRestClient.Setup(c => c.ExecuteAsync(It.IsAny<IRestRequest>(),
                                                     It.IsAny<Action<IRestResponse<object>, RestRequestAsyncHandle>>()))
                .Returns(_testHandle);
            mockRestClient.SetupSet(c => c.Authenticator = It.IsAny<IAuthenticator>());
            GitHubClient githubClient = CreateClient(mockFactory.Object);

            githubClient.CallApiAsync<object>(new GitHubRequest("foo", API.v3, Method.GET),
                                              o => { },
                                              e => { });

            mockFactory.VerifyAll();
        }
    }
}