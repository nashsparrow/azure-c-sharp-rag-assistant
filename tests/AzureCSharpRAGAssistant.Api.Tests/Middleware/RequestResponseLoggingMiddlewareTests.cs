using AzureCSharpRAGAssistant.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Middleware
{
    public class RequestResponseLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<RequestResponseLoggingMiddleware>> _loggerMock;

        public RequestResponseLoggingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
        }

        [Fact]
        public async Task Test_TheNextDelegateIsCalled()
        {
            var context = new DefaultHttpContext();
            var nextCalled = false;

            RequestDelegate next = (HttpContext context) =>
            {
                nextCalled = true;
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            };


            await new RequestResponseLoggingMiddleware(next, _loggerMock.Object).InvokeAsync(context);

            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task Test_TheRequestAndResponseIsLogged()
        {
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.Path = "/api/test";

            RequestDelegate next = (HttpContext context) =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            };

            var middleware = new RequestResponseLoggingMiddleware(next, _loggerMock.Object);

            await middleware.InvokeAsync(context);

             _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("Incoming Request") &&
                        v.ToString()!.Contains("POST") &&
                        v.ToString()!.Contains("/api/query/query")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("Requet Completed") &&
                        v.ToString()!.Contains("POST") &&
                        v.ToString()!.Contains("/api/query/query") &&
                        v.ToString()!.Contains("200")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}