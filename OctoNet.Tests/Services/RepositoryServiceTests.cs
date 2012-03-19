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
    public class RepositoryServiceTests
    {
        [Test]
        public void IsWatchingAsync_ShouldCallbackWithError_WhenResponseIsSomeRandomError()
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
            var repoService = new RepositoryService(mockClient.Object);

            GitHubException actualException = null;
            repoService.IsWatchingAsync("akilb",
                                        "ngithub",
                                        c => { },
                                        e => actualException = e);

            Assert.AreSame(expectedException, actualException);
        }

        [Test]
        public void IsWatchingAsync_ShouldCallbackWithFalse_WhenResponseIsNotFound()
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
            var repoService = new RepositoryService(mockClient.Object);

            bool isWatched = true;
            repoService.IsWatchingAsync("akilb",
                                        "ngithub",
                                        fl => isWatched = fl,
                                        e => { });

            Assert.IsFalse(isWatched);
        }

        [Test]
        public void IsWatchingAsync_ShouldCallbackWithTrue_WhenResponseIsNoContent()
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
            var repoService = new RepositoryService(mockClient.Object);

            bool isWatched = false;
            repoService.IsWatchingAsync("akilb",
                                        "ngithub",
                                        fl => isWatched = fl,
                                        e => { });

            Assert.IsTrue(isWatched);
        }
    }
}