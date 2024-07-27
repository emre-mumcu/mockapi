using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MockAPI.AppData.Entities;
using MockAPI.AppLib.Common;
using MockAPI.AppLib.Extensions;
using MockAPI.AppLib.Services;

namespace MockAPI.AppLib.Filters
{
    /// <summary>
    /// [ActionLogFilter]
    /// https://www.c-sharpcorner.com/article/learn-about-custom-action-filter-in-asp-net/
    /// </summary>
    public class ActionLogFilter : ActionFilterAttribute, IExceptionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Items.Add(Literals.TraceId, filterContext.HttpContext.TraceIdentifier.ToString());

                ActionLog actionLog = new ActionLog()
                {
                    EventName = "OnActionExecuting",
                    EventData = JsonSerializer.Serialize(filterContext.ActionArguments),
                    ConnectionId = filterContext.HttpContext.Connection.Id.ToString(),
                    TraceId = filterContext.HttpContext.TraceIdentifier.ToString(),
                    Host = filterContext.HttpContext.Request.Host.ToString(),
                    Port = filterContext.HttpContext.Request.Host.Port.ToString() ?? "0",
                    Path = filterContext.HttpContext.Request.Path.ToString(),
                    Method = filterContext.HttpContext.Request.Method.ToString(),
                    Scheme = filterContext.HttpContext.Request.Scheme.ToString(),
                    Protocol = filterContext.HttpContext.Request.Protocol.ToString(),
                    RouteData = filterContext.RouteData.Values.ToStringEx(),
                    // RouteValues = filterContext.HttpContext.Request.RouteValues.ToStringEx(),
                    ModelState = filterContext.ModelState.ToStringEx(),
                    Query = filterContext.HttpContext.Request.Query.ToStringEx(),
                    QueryString = filterContext.HttpContext.Request.QueryString.ToString(),
                    ActionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName,
                    ControllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ControllerName,
                    LogTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-ffffff"),
                    RequestIp = filterContext.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>().GetClientIP()
                };

                var logService = filterContext.HttpContext.RequestServices.GetRequiredService<IActionLogService>();

                if (logService != null) logService.InsertLog(actionLog);
            }
            catch
            {
                // TODO: 
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                if(filterContext == null) return;

                if(filterContext.Exception != null) return;

                var traceId = filterContext.HttpContext?.Items?[Literals.TraceId]?.ToString() ?? string.Empty;

                if(traceId != filterContext.HttpContext?.TraceIdentifier.ToString()) traceId += filterContext.HttpContext?.TraceIdentifier.ToString();
                
                var result = string.Empty;

                if(filterContext.Result != null && filterContext.Result is JsonResult jsonResult)
                {
                    result = JsonSerializer.Serialize(jsonResult.Value);
                }
                else if(filterContext.Result != null && filterContext.Result is ObjectResult objectResult)                
                {
                    result = JsonSerializer.Serialize(objectResult.Value);
                }
                else if(filterContext.Result != null)
                {
                    result = $"Unknown result type: {filterContext?.Result?.GetType()}";
                }
                else
                {
                    result = "null";
                }

                ActionLog actionLog = new ActionLog()
                {
                    EventName = "OnActionExecuted",
                    EventData = JsonSerializer.Serialize(result),
                    ConnectionId = filterContext?.HttpContext?.Connection.Id.ToString(),
                    TraceId = filterContext?.HttpContext?.TraceIdentifier.ToString(),
                    Host = filterContext?.HttpContext?.Request?.Host.ToString() ?? "0",
                    Port = filterContext?.HttpContext?.Request?.Host.Port.ToString() ?? "0",
                    Path = filterContext?.HttpContext?.Request?.Path.ToString() ?? "0",
                    Method = filterContext?.HttpContext?.Request?.Method.ToString() ?? "0",
                    Scheme = filterContext?.HttpContext?.Request?.Scheme.ToString() ?? "0",
                    Protocol = filterContext?.HttpContext?.Request?.Protocol?.ToString() ?? "0",
                    RouteData = filterContext?.RouteData.Values.ToStringEx(),
                    // RouteValues = filterContext.HttpContext.Request.RouteValues.ToStringEx(),
                    ModelState = filterContext?.ModelState.ToStringEx(),
                    Query = filterContext?.HttpContext?.Request.Query.ToStringEx(),
                    QueryString = filterContext?.HttpContext?.Request.QueryString.ToString(),
                    ActionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext!.ActionDescriptor).ActionName,
                    ControllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext!.ActionDescriptor).ControllerName,
                    LogTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-ffffff"),
                    RequestIp = filterContext?.HttpContext?.RequestServices.GetRequiredService<IHttpContextAccessor>()?.GetClientIP() ?? "0"
                };               

                var logService = filterContext?.HttpContext?.RequestServices.GetRequiredService<IActionLogService>();

                if(logService != null) logService.InsertLog(actionLog);
            }
            catch 
            {
                // TODO: 
            }

            base.OnActionExecuted(filterContext!);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        public async void OnException(ExceptionContext filterContext)
        {
            try
            {
                var traceId = filterContext.HttpContext?.Items?[Literals.TraceId]?.ToString() ?? string.Empty;

                ExceptionLog exceptionLog = new ExceptionLog {
                    TraceId = traceId,
                    Source = filterContext.Exception?.Source ?? string.Empty,
                    Message = filterContext.Exception?.Message ?? string.Empty,
                    InnerException = filterContext.Exception?.InnerException?.Message,
                    StackTrace = filterContext.Exception?.StackTrace ?? string.Empty
                };

                var logService = filterContext.HttpContext?.RequestServices.GetRequiredService<IActionLogService>();

                if(logService != null) await logService.InsertLog(exceptionLog);                
            }
            catch 
            {
                // TODO:
            }
        }
    }

    // public class ActionLogFilter : IActionFilter
    // {
    //     // https://code-maze.com/action-filters-aspnetcore/
    //     // [ServiceFilter(typeof(MosipActionFilter))]
    //     public void OnActionExecuting(ActionExecutingContext context)
    //     {
    //         // our code before action executes
    //     }
    //     public void OnActionExecuted(ActionExecutedContext context)
    //     {
    //         // our code after action executes
    //     }
    // }

    // public class ActionLogFilterAsync : IAsyncActionFilter
    // {
    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        // execute any code before the action executes
    //        var result = await next();
    //        // execute any code after the action executes
    //    }
    // }
}