using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace TimeSnapBackend_MySql.Middlewares
{
    public class ErrorResponseDto
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public List<ValidationErrorDto>? Errors { get; set; } // Nullable to handle different cases
    }

    public class ValidationErrorDto
    {
        public string? PropertyName { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorDetails = new ErrorResponseDto
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred."
            };

            switch (exception)
            {
                case BadHttpRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Invalid request."
                    };
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Unauthorized access."
                    };
                    break;

                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Resource not found."
                    };
                    break;

                case FluentValidation.ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var validationErrors = validationEx.Errors
                        .Select(e => new ValidationErrorDto { PropertyName = e.PropertyName, ErrorMessage = e.ErrorMessage })
                        .ToList();

                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Validation failed.",
                        Errors = validationErrors
                    };
                    break;

                case System.ComponentModel.DataAnnotations.ValidationException dataAnnotationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = dataAnnotationEx.Message
                    };
                    break;

                case DbUpdateException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Database update failed."
                    };
                    break;

                case SqlException:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Database error occurred."
                    };
                    break;

                case TimeoutException:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Request timed out."
                    };
                    break;

                case NotImplementedException:
                    response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "This feature is not implemented."
                    };
                    break;

                case OperationCanceledException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Operation was canceled."
                    };
                    break;

                case ArgumentNullException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "A required argument was null."
                    };
                    break;

                case ArgumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Invalid argument provided."
                    };
                    break;

                case FormatException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "Invalid format."
                    };
                    break;

                case JsonException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails = new ErrorResponseDto
                    {
                        StatusCode = response.StatusCode,
                        Message = "JSON serialization/deserialization error."
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var errorResponse = JsonSerializer.Serialize(errorDetails);
            await response.WriteAsync(errorResponse);
        }
    }
}
