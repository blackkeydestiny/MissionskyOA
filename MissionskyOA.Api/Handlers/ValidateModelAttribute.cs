using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using MissionskyOA.Models;


namespace MissionskyOA.Api.Handlers
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                var errorList = from kvp in actionContext.ModelState
                                from e in kvp.Value.Errors
                                where e.ErrorMessage!=""
                                select e.ErrorMessage;
                
                //The request is invalid.
                actionContext.Response = actionContext.Request.CreateResponse<ApiResponse>(
                    HttpStatusCode.BadRequest,new ApiResponse{
                        StatusCode =(int) HttpStatusCode.BadRequest,
                        Message = "The request is invalid.",
                        MessageDetail =string.Join("\n", errorList)
                    }
                );
            }
        }
    }
}