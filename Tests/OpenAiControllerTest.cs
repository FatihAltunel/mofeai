using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace MovieGPT.ApplicationServices.Tests
{
    public class OpenAiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly OpenAiService _openAiService;

        public OpenAiServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _openAiService = new OpenAiService("test-api-key")
            {
                _httpClient = httpClient
            };
        }

        [Fact]
        public async Task GetCompletionAsync_ShouldReturnValidResponse_WhenApiCallSucceeds()
        {
            // Arrange
            var expectedResponse = "{\"choices\":[{\"message\":{\"content\":\"test-response\"}}]}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _openAiService.GetCompletionAsync("test-prompt");

            // Assert
            Assert.Equal("test-response", result);
        }

        [Fact]
        public async Task GetCompletionAsync_ShouldReturnErrorMessage_WhenApiCallFails()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            var result = await _openAiService.GetCompletionAsync("test-prompt");

            // Assert
            Assert.Equal("Error: BadRequest", result);
        }

        [Fact]
        public async Task GetResult_ShouldReturnValidMovieJson_WhenApiCallSucceeds()
        {
            // Arrange
            var apiResponse = "{\"choices\":[{\"message\":{\"content\":\"{\\\"Movie title\\\":\\\"Inception\\\",\\\"Genre\\\":\\\"Sci-Fi\\\",\\\"Rating\\\":8.8,\\\"Image URL\\\":\\\"https://wikipedia.org/Inception.jpg\\\"}\"}}]}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(apiResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _openAiService.GetResult("Inception");

            // Assert
            Assert.Contains("\"Movie title\":\"Inception\"", result);
        }

        [Fact]
        public async Task GetMoreThanOneResult_ShouldReturnValidMoviesJson_WhenApiCallSucceeds()
        {
            // Arrange
            var apiResponse = "{\"choices\":[{\"message\":{\"content\":\"[{\\\"Movie title\\\":\\\"Inception\\\",\\\"Genre\\\":\\\"Sci-Fi\\\",\\\"Rating\\\":8.8,\\\"Image URL\\\":\\\"https://wikipedia.org/Inception.jpg\\\"}]\"}}]}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(apiResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _openAiService.GetMoreThanOneResult("Inception");

            // Assert
            Assert.Contains("\"Movie title\":\"Inception\"", result);
        }

        [Fact]
        public async Task GetMoreThanOneResult_ShouldReturnEmptyMoviesJson_WhenApiCallFails()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            var result = await _openAiService.GetMoreThanOneResult("Inception");

            // Assert
            Assert.Equal("[]", result);
        }
    }

    public class Movie
    {
        public string MovieTitle { get; set; } = "";
        public string Genre { get; set; } = "";
        public double Rating { get; set; } = 0.0;
        public string ImageUrl { get; set; } = "";
    }
}
