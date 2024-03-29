﻿using System.Net;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.Http;
using Moq;
using Xunit;

namespace Eu.EDelivery.AS4.UnitTests.Http
{
    public class GivenReliableHttpClientFacts
    {
        public class Request
        {
            [Fact]
            public void CreatePostRequest()
            {
                // Arrange
                const string expectedUrl = "http://valid/url";
                const string expectedType = "application/json";
                var sut = new ReliableHttpClient();

                // Act
                HttpWebRequest request = sut.Request(expectedUrl, expectedType);

                // Assert
                Assert.Equal("POST", request.Method);
                Assert.Equal(expectedUrl, request.RequestUri.AbsoluteUri);
                Assert.Equal(expectedType, request.ContentType);
            }
        }

        public class Respond
        {
            [Fact]
            public async Task ResponseDontGetReceived_IfMissingRespondServer()
            {
                // Arrange
                var sut = new ReliableHttpClient();
                var stubRequest = new Mock<HttpWebRequest>();
                stubRequest.Setup(r => r.GetResponseAsync()).Throws<WebException>();

                // Act
                (HttpWebResponse actualResponse, WebException exception) response = await sut.Respond(stubRequest.Object);

                // Assert
                Assert.NotNull(response.exception);
            }

            [Fact]
            public async Task ResponseGetReceived_IfValidRespondServer()
            {
                // Arrange
                var sut = new ReliableHttpClient();
                const HttpStatusCode expectedStatus = HttpStatusCode.OK;
                HttpWebRequest stubRequest = CreateStubRequest(expectedStatus);

                // Act
                (HttpWebResponse actualResponse, WebException exception) response = await sut.Respond(stubRequest);

                // Assert
                Assert.Equal(expectedStatus, response.actualResponse.StatusCode);
            }

            private static HttpWebRequest CreateStubRequest(HttpStatusCode statusCode)
            {
                var stubRequest = new Mock<HttpWebRequest>();
                var stubResponse = new Mock<HttpWebResponse>();
                stubResponse.Setup(r => r.StatusCode).Returns(statusCode);
                stubRequest.Setup(r => r.GetResponseAsync()).ReturnsAsync(stubResponse.Object);

                return stubRequest.Object;
            }
        }
    }
}