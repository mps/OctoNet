using System;
using System.Net;
using Moq;
using NUnit.Framework;
using OctoNet.Helpers;
using OctoNet.Web;

namespace OctoNet.Tests.Helpers
{
    [TestFixture]
    public class ResponseProcessorTests
    {
        [Test]
        public void TryProcessError_ShouldReturnException_WithLimitExceedErrorType_IfResponseStatusCodeIsForbidden()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode).Returns(HttpStatusCode.Forbidden);
            mockResp.Setup(r => r.ErrorException).Returns<Exception>(null);
            mockResp.Setup(r => r.ResponseStatus).Returns(ResponseStatus.Completed);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            processor.TryProcessResponseErrors(mockResp.Object, out ex);

            Assert.AreEqual(ErrorType.ApiLimitExceeded, ex.ErrorType);
        }

        [Test]
        public void TryProcessError_ShouldReturnException_WithNoNetworkErrorType_IfResponseStatusIsError()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode).Returns(HttpStatusCode.RequestTimeout);
            mockResp.Setup(r => r.ErrorException).Returns<Exception>(null);
            mockResp.Setup(r => r.ResponseStatus).Returns(ResponseStatus.Error);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            processor.TryProcessResponseErrors(mockResp.Object, out ex);

            Assert.AreEqual(ErrorType.NoNetwork, ex.ErrorType);
        }

        [Test]
        public void TryProcessError_ShouldReturnException_WithNotFoundErrorType_IfResponseStatusCodeIsNotFound()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode).Returns(HttpStatusCode.NotFound);
            mockResp.Setup(r => r.ErrorException).Returns<Exception>(null);
            mockResp.Setup(r => r.ResponseStatus).Returns(ResponseStatus.Completed);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            processor.TryProcessResponseErrors(mockResp.Object, out ex);

            Assert.AreEqual(ErrorType.ResourceNotFound, ex.ErrorType);
        }

        [Test]
        public void TryProcessError_ShouldReturnException_WithServerErrorErrorType_IfResponseStatusIsBadGateway()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode).Returns(HttpStatusCode.BadGateway);
            mockResp.Setup(r => r.ErrorException).Returns<Exception>(null);
            mockResp.Setup(r => r.ResponseStatus).Returns(ResponseStatus.Completed);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            processor.TryProcessResponseErrors(mockResp.Object, out ex);

            Assert.AreEqual(ErrorType.ServerError, ex.ErrorType);
        }

        [Test]
        public void TryProcessError_ShouldReturnException_WithUnauthorizedErrorType_IfResponseStatusIsUnauthorized()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode).Returns(HttpStatusCode.Unauthorized);
            mockResp.Setup(r => r.ErrorException).Returns<Exception>(null);
            mockResp.Setup(r => r.ResponseStatus).Returns(ResponseStatus.Completed);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            processor.TryProcessResponseErrors(mockResp.Object, out ex);

            Assert.AreEqual(ErrorType.Unauthorized, ex.ErrorType);
        }

        [Test]
        public void TryProcessError_ShouldReturnFalse_IfResponseStatusCodeIsCreated()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode)
                .Returns(HttpStatusCode.Created);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            Assert.IsFalse(processor.TryProcessResponseErrors(mockResp.Object, out ex));
        }

        [Test]
        public void TryProcessError_ShouldReturnFalse_IfResponseStatusCodeIsOk()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode)
                .Returns(HttpStatusCode.OK);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            Assert.IsFalse(processor.TryProcessResponseErrors(mockResp.Object, out ex));
        }

        [Test]
        public void TryProcessError_ShouldReturnTrue_IfResponseStatusCodeIsNotOKOrCreated()
        {
            var mockResp = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResp.Setup(r => r.StatusCode).Returns(HttpStatusCode.Forbidden);
            mockResp.Setup(r => r.ErrorException).Returns<Exception>(null);
            mockResp.Setup(r => r.ResponseStatus).Returns(ResponseStatus.Completed);
            var processor = new ResponseProcessor();

            GitHubException ex = null;
            Assert.IsTrue(processor.TryProcessResponseErrors(mockResp.Object, out ex));
        }
    }
}