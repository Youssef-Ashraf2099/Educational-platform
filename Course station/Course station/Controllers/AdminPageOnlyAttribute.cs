using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class AdminPageOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var referrer = context.HttpContext.Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referrer) || !referrer.Contains("/Admin/AdminPage"))
        {
            context.Result = new ForbidResult();
        }
        base.OnActionExecuting(context);
    }
}