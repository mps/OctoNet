using System;
using Moq;
using NUnit.Framework;
using OctoNet.Web;

namespace OctoNet.Tests
{
    [TestFixture]
    public class GitHubExceptionTests
    {
        [Test]
        public void ErrorType_ShouldBeTheGivenErrorType()
        {
            ErrorType expectedErrorType = ErrorType.ServerError;
            var mockResponse = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResponse.Setup(r => r.ErrorException).Returns<Exception>(null);
            var ex = new GitHubException(mockResponse.Object, expectedErrorType);

            Assert.AreEqual(expectedErrorType, ex.ErrorType);
        }

        [Test]
        public void InnerException_ShouldBeTheErrorExceptionOfTheResponse()
        {
            var expectedInnerException = new Exception();
            var mockResponse = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResponse.Setup(e => e.ErrorException)
                .Returns(expectedInnerException);
            var ex = new GitHubException(mockResponse.Object, ErrorType.Unknown);

            Assert.AreSame(expectedInnerException, ex.InnerException);
        }

        [Test]
        public void Response_ShouldBeTheGivenResponse()
        {
            var mockResponse = new Mock<IGitHubResponse>(MockBehavior.Strict);
            mockResponse.Setup(r => r.ErrorException).Returns<Exception>(null);
            IGitHubResponse expectedResponse = mockResponse.Object;
            var ex = new GitHubException(expectedResponse, ErrorType.Unknown);

            Assert.AreSame(expectedResponse, ex.Response);
        }
    }
}