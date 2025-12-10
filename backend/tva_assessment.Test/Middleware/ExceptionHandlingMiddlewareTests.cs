using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using tva_assessment.Middleware;

namespace tva_assessment.Test.Middleware
{
    [TestFixture]
    public class ExceptionHandlingMiddlewareTests
    {
        [Test]
        public async Task InvokeAsync_WhenNextThrowsInvalidOperationException_ShouldReturnBadRequestWithMessage()
        {
            // Arrange
            var exception = new InvalidOperationException("Something went wrong.");
            var middleware = CreateMiddlewareThatThrows(exception);
            var context = CreateContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            context.Response.ContentType.Should().Be("application/json");

            var (statusCode, message) = ReadErrorResponse(context);
            statusCode.Should().Be(StatusCodes.Status400BadRequest);
            message.Should().Be("Something went wrong.");
        }

        [Test]
        public async Task InvokeAsync_WhenNextThrowsKeyNotFoundException_ShouldReturnNotFoundWithMessage()
        {
            // Arrange
            var exception = new KeyNotFoundException("Not found.");
            var middleware = CreateMiddlewareThatThrows(exception);
            var context = CreateContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            context.Response.ContentType.Should().Be("application/json");

            var (statusCode, message) = ReadErrorResponse(context);
            statusCode.Should().Be(StatusCodes.Status404NotFound);
            message.Should().Be("Not found.");
        }

        [Test]
        public async Task InvokeAsync_WhenNextThrowsUnexpectedException_ShouldReturnInternalServerErrorWithGenericMessage()
        {
            // Arrange
            var exception = new Exception("Boom.");
            var middleware = CreateMiddlewareThatThrows(exception);
            var context = CreateContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            context.Response.ContentType.Should().Be("application/json");

            var (statusCode, message) = ReadErrorResponse(context);
            statusCode.Should().Be(StatusCodes.Status500InternalServerError);
            message.Should().Be("An unexpected error occurred.");
        }

        [Test]
        public async Task InvokeAsync_WhenNextSucceeds_ShouldNotChangeSuccessfulResponse()
        {
            // Arrange
            var middleware = new ExceptionHandlingMiddleware(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("OK");
            });

            var context = CreateContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var body = await reader.ReadToEndAsync();
            body.Should().Be("OK");
        }

        private static ExceptionHandlingMiddleware CreateMiddlewareThatThrows(Exception exception)
        {
            return new ExceptionHandlingMiddleware(_ => throw exception);
        }

        private static HttpContext CreateContext()
        {
            var context = new DefaultHttpContext
            {
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            return context;
        }

        private static (int statusCode, string message) ReadErrorResponse(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(context.Response.Body);
            var json = reader.ReadToEnd();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var statusCode = root.GetProperty("statusCode").GetInt32();
            var message = root.GetProperty("message").GetString() ?? string.Empty;

            return (statusCode, message);
        }
    }
}
