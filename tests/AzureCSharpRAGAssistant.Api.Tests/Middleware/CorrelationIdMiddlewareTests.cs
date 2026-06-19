using Azure.Core;
using AzureCSharpRAGAssistant.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Middleware
{
    public class CorrelationIdMiddlewareTests
    {
        private readonly Mock<ILogger<CorrelationIdMiddleware>> _loggerMock;

        public CorrelationIdMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
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

            var middleware = new CorrelationIdMiddleware(next, _loggerMock.Object);

            await middleware.InvokeAsync(context);

            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }
    }
}