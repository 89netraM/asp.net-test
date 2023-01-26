using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaintBot.Server.Features.Errors;

public class ErrorExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ErrorException errorException)
        {
            context.Result = new JsonResult(new ErrorResponse(errorException.ErrorCode, errorException.Details))
            {
                StatusCode = errorException.ErrorCode.ToStatusCode(),
            };
            context.ExceptionHandled = true;
        }
    }
}

public static class ErrorExceptionFilterExtension
{
    public static MvcOptions AddErrorExcpetionFilter(this MvcOptions options)
    {
        options.Filters.Add<ErrorExceptionFilter>();
        return options;
    }
}
