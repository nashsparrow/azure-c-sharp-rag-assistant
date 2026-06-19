using AzureCSharpRAGAssistant.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        public ExceptionHandlingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        }

        [Fact]
        public async void Test_InvokeAsync_CallNext_WhenNoException()
        {
            var context = new DefaultHttpContext();
            var nextCalled = false;

            RequestDelegate next = (HttpContext context) =>
            {
                nextCalled = true;
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            };

            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            await middleware.InvokeAsync(context);

            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        [Theory]
        [InlineData(typeof(ArgumentException), StatusCodes.Status400BadRequest)]
        [InlineData(typeof(InvalidDataException), StatusCodes.Status400BadRequest)]
        [InlineData(typeof(InvalidCastException), StatusCodes.Status500InternalServerError)]
        [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async void Test_InvokeAsync_ReturnsRelatedHttpStatus_WhenException(Type exceptionType, int statusCode)
        {
            var context = new DefaultHttpContext();

            RequestDelegate next = (HttpContext context) => 
                throw (Exception)Activator.CreateInstance(exceptionType, "Exception Message")!;

            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object);

            await middleware.InvokeAsync(context);

            Assert.Equal(statusCode, context.Response.StatusCode);
        }
    }
}