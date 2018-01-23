using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Models;

namespace MissionskyOA.Api.Handlers
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public static readonly log4net.ILog log =log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;
            if (exception is ApiBadRequestException)
            {
                context.Response = GenerateResponse(context.Request, HttpStatusCode.BadRequest, "bad request", exception.Message);
            }
            else if (exception is ApiDataValidationException)
            {
                context.Response = GenerateResponse(context.Request, HttpStatusCode.BadRequest, "data validation error", exception.Message);
            }
            else if (exception is ApiAuthException)
            {
                context.Response = GenerateResponse(context.Request, HttpStatusCode.Unauthorized, "unauthorized", exception.Message);
            }
            else if (exception is ApiNotfoundException)
            {
                context.Response = GenerateResponse(context.Request, HttpStatusCode.NotFound, "resource not found", exception.Message);
            }
            else
            {
                context.Response = GenerateResponse(context.Request, HttpStatusCode.InternalServerError, exception.Message, null);
            }

            log.Error(exception.Message, exception);
        }

        private HttpResponseMessage GenerateResponse(HttpRequestMessage request, HttpStatusCode statusCode,  string message, string detail)
        {
            return request.CreateResponse<ApiResponse>(HttpStatusCode.OK, new ApiResponse
            {
                StatusCode = (int)statusCode,
                Message = message,
                MessageDetail = detail,
            });
        }
    }
}