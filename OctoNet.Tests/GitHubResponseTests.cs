using System;
using System.Net;
using Moq;
using NUnit.Framework;
using RestSharp;
using ResponseStatus = OctoNet.Web.ResponseStatus;

namespace OctoNet.Tests
{
    [TestFixture]
    public class GitHubResponseTests
    {
        [Test]
        public void ContentType_ShouldReturnResponseContentType()
        {
            string expectedContentType = "foo";
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.ContentType)
                .Returns(expectedContentType);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreEqual(expectedContentType, resp.ContentType);
        }

        [Test]
        public void Content_ShouldReturnResponseContent()
        {
            string expectedContent = "foo";
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.Content)
                .Returns(expectedContent);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreEqual(expectedContent, resp.Content);
        }

        [Test]
        public void Data_ShouldContainTheResponseData()
        {
            var expectedData = new object();
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.Data)
                .Returns(expectedData);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreSame(expectedData, resp.Data);
        }

        [Test]
        public void ErrorException_ShouldContainTheResponseErrorException()
        {
            var expectedException = new Exception();
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.ErrorException)
                .Returns(expectedException);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreEqual(expectedException, resp.ErrorException);
        }

        [Test]
        public void ErrorMessage_ShouldContainTheResponseErrorMessage()
        {
            string expectedErrorMessage = "foo";
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.ErrorMessage)
                .Returns(expectedErrorMessage);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreEqual(expectedErrorMessage, resp.ErrorMessage);
        }

        [Test]
        public void ResponseStatus_ShouldReturnTheConvertedResponseResponseStatus()
        {
            ResponseStatus expectedResponseStatus = ResponseStatus.Completed;
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.ResponseStatus)
                .Returns(RestSharp.ResponseStatus.Completed);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreEqual(expectedResponseStatus, resp.ResponseStatus);
        }

        [Test]
        public void StatusCode_ShouldContainResponseStatus()
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.Conflict;
            var mockResp = new Mock<IRestResponse<object>>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode)
                .Returns(expectedStatusCode);
            var resp = new GitHubResponse<object>(mockResp.Object);

            Assert.AreEqual(expectedStatusCode, resp.StatusCode);
        }
    }
}