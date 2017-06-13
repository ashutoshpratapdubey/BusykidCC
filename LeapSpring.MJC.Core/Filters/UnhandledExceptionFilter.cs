using System;
using System.Net;
using Elmah;
using System.Web.Http.Filters;
using System.Net.Http;

namespace LeapSpring.MJC.Core.Filters
{
    /// <summary>
    /// Represents a throw unhandled exception filter
    /// </summary>
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            var exType = context.Exception.GetType();

            if (exType == typeof(UnauthorizedAccessException))
                status = HttpStatusCode.Unauthorized;
            else if (exType == typeof(InvalidParameterException) || exType == typeof(InvalidOperationException))
                status = HttpStatusCode.BadRequest;
            else if (exType == typeof(ObjectNotFoundException))
                status = HttpStatusCode.NotFound;

            // create a new response and attach our ApiError object
            // which now gets returned on ANY exception result
            var errorResponse =
               context.Request.CreateErrorResponse(status, context.Exception.Message);
            context.Response = errorResponse;

            if (context.Exception != null)
                ErrorSignal.FromCurrentContext().Raise(context.Exception);

            base.OnException(context);
        }
    }
}