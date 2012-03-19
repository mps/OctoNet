using System;
using Moq;
using NUnit.Framework;
using OctoNet.Models;
using OctoNet.Models.Dto;
using OctoNet.Services;
using OctoNet.Tests.Helpers;

namespace OctoNet.Tests.Services
{
    [TestFixture]
    public class IssueServiceTests
    {
        [Test]
        public void CreateCommentAsync_ShouldAddComment_WithBodySetToCommentText_AsRequestBody()
        {
            string expectedBody = "fooBody";
            object requestBody = null;
            var mockClient = new Mock<IGitHubClient>(MockBehavior.Strict);
            mockClient.Setup(c => c.CallApiAsync(It.IsAny<GitHubRequest>(),
                                                 It.IsAny<Action<IGitHubResponse<Comment>>>(),
                                                 It.IsAny<Action<GitHubException>>()))
                .Callback<GitHubRequest, Action<IGitHubResponse<Comment>>, Action<GitHubException>>(
                    (req, c, e) => requestBody = req.Body)
                .Returns(TestHelpers.CreateTestHandle())
                .Verifiable();
            var svc = new IssueService(mockClient.Object);

            svc.CreateCommentAsync("foo", "bar", 1, expectedBody, c => { }, e => { });

            string actualBody = ((CommentDto) requestBody).Body;
            Assert.AreSame(expectedBody, actualBody);
        }
    }
}