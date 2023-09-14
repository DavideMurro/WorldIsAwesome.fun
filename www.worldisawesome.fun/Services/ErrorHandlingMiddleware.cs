using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using www.worldisawesome.fun.ExceptionModels;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.Services
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext /* other dependencies */)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (ex is MyNotFoundException) code = HttpStatusCode.NotFound;
            else if (ex is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            else if (ex is MyException) code = HttpStatusCode.BadRequest;
            //else if (ex is MyPageException) return View(new ErrorViewModel{ RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            var result = JsonConvert.SerializeObject(new ErrorData() { ErrorCode = (int)code, ErrorMessage = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }

}
