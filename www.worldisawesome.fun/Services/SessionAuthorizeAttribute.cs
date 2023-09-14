using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using www.worldisawesome.fun.ExceptionModels;

namespace www.worldisawesome.fun.Services
{
    internal class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Claims?.Any() != true) throw new MyUnauthorizedException("Session expired");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO: LET S CHANGE THE LAST ACCESS USER ON DB
        }
    }
}