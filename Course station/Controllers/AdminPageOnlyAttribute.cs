using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminPageOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var adminId = context.HttpContext.Session.GetInt32("AdminId");
        if (adminId == null)
        {
            context.Result = new RedirectToActionResult("AdminLogin", "Admin", null);
            return;
        }

        // Refresh the session
        context.HttpContext.Session.SetInt32("AdminId", adminId.Value);

        var referrer = context.HttpContext.Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referrer) || !referrer.Contains("/Admin/AdminPage"))
        {
            context.Result = new ForbidResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}
