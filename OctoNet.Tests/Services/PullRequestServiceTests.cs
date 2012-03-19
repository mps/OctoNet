using System;
using System.Net;
using Moq;
using NUnit.Framework;
using OctoNet.Services;
using OctoNet.Tests.Helpers;
using OctoNet.Web;

namespace OctoNet.Tests.Services
{
    [TestFixture]
    public class PullRequestServiceTests
    {
        [Test]
        public void IsPullRequestMergedAsync_ShouldCallbackWithError_WhenResponseIsSomeRandomError()
        {
            var mockResponse = new Mock<IGitHubResponse<object>>(MockBehavior.Strict);
            var mockClient = new Mock<IGitHubClient>(MockBehavior.Strict);
            mockResponse.Setup(r => r.ErrorException)
                .Returns(new Exception());
            mockResponse.Setup(r => r.StatusCode)
                .Returns(HttpStatusCode.Forbidden);
            var expectedException = new GitHubException(mockResponse.Object,
                                                        ErrorType.Unauthorized);
            mockClient.Setup(c => c.CallApiAsync(It.IsAny<GitHubRequest>(),
                                                 It.IsAny<Action<IGitHubResponse<object>>>(),
                                                 It.IsAny<Action<GitHubException>>()))
                .Callback<GitHubRequest,
                    Action<IGitHubResponse<object>>,
                    Action<GitHubException>>((req, c, e) => { e(expectedException); })
                .Returns(TestHelpers.CreateTestHandle());
            var pullReqSvc = new PullRequestService(mockClient.Object);

            GitHubException actualException = null;
            pullReqSvc.IsPullRequestMergedAsync("akilb",
                                                "ngithub",
                                                1,
                                                c => { },
                                                e => actualException = e);

            Assert.AreSame(expectedException, actualException);
        }

        [Test]
        public void IsPullRequestMergedAsync_ShouldCallbackWithFalse_WhenResponseIsNotFound()
        {
            var mockResponse = new Mock<IGitHubResponse<object>>(MockBehavior.Strict);
            var mockClient = new Mock<IGitHubClient>(MockBehavior.Strict);
            mockResponse.Setup(r => r.ErrorException)
                .Returns(new Exception());
            mockResponse.Setup(r => r.StatusCode)
                .Returns(HttpStatusCode.NotFound);
            mockClient.Setup(c => c.CallApiAsync(It.IsAny<GitHubRequest>(),
                                                 It.IsAny<Action<IGitHubResponse<object>>>(),
                                                 It.IsAny<Action<GitHubException>>()))
                .Callback<GitHubRequest,
                    Action<IGitHubResponse<object>>,
                    Action<GitHubException>>(
                        (req, c, e) => { e(new GitHubException(mockResponse.Object, ErrorType.ResourceNotFound)); })
                .Returns(TestHelpers.CreateTestHandle());
            var pullReqSvc = new PullRequestService(mockClient.Object);

            bool isMerged = true;
            pullReqSvc.IsPullRequestMergedAsync("akilb",
                                                "ngithub",
                                                16,
                                                fl => isMerged = fl,
                                                e => { });

            Assert.IsFalse(isMerged);
        }

        [Test]
        public void IsPullRequestMergedAsync_ShouldCallbackWithTrue_WhenResponseIsNoContent()
        {
            var mockResponse = new Mock<IGitHubResponse<object>>(MockBehavior.Strict);
            var mockClient = new Mock<IGitHubClient>(MockBehavior.Strict);
            mockResponse.Setup(r => r.ErrorException)
                .Returns(new Exception());
            mockResponse.Setup(r => r.StatusCode)
                .Returns(HttpStatusCode.NoContent);
            mockClient.Setup(c => c.CallApiAsync(It.IsAny<GitHubRequest>(),
                                                 It.IsAny<Action<IGitHubResponse<object>>>(),
                                                 It.IsAny<Action<GitHubException>>()))
                .Callback<GitHubRequest,
                    Action<IGitHubResponse<object>>,
                    Action<GitHubException>>(
                        (req, c, e) => { e(new GitHubException(mockResponse.Object, ErrorType.Unknown)); })
                .Returns(TestHelpers.CreateTestHandle());
            var pullReqSvc = new PullRequestService(mockClient.Object);

            bool isMerged = false;
            pullReqSvc.IsPullRequestMergedAsync("akilb",
                                                "ngithub",
                                                6,
                                                fl => isMerged = fl,
                                                e => { });

            Assert.IsTrue(isMerged);
        }
    }
}