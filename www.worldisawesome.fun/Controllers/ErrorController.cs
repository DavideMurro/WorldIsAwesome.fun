using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using www.worldisawesome.fun.ExceptionModels;
using www.worldisawesome.fun.Models;

namespace www.worldisawesome.fun.Controllers
{
    public class ErrorController : Controller
    {
        /*
        private readonly TelemetryClient _telemetryClient;
        public ErrorController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }
        */


        public IActionResult Index()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exceptionHandlerPathFeature?.Error is MyNotFoundException) code = HttpStatusCode.NotFound;
            else if (exceptionHandlerPathFeature?.Error is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            else if (exceptionHandlerPathFeature?.Error is MyException) code = HttpStatusCode.BadRequest;


            ViewBag.Error = new ErrorData() { ErrorCode = (int)code, ErrorMessage = exceptionHandlerPathFeature?.Error.Message };

            return View();
        }

        /*
        [Route("500")]
        public IActionResult AppError()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _telemetryClient.TrackException(exceptionHandlerPathFeature.Error);
            _telemetryClient.TrackEvent("Error.ServerError", new Dictionary<string, string>
            {
                ["originalPath"] = exceptionHandlerPathFeature.Path,
                ["error"] = exceptionHandlerPathFeature.Error.Message
            });
            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            _telemetryClient.TrackEvent("Error.PageNotFound", new Dictionary<string, string>
            {
                ["originalPath"] = originalPath
            });
            return View();
        }
        */
    }
}
