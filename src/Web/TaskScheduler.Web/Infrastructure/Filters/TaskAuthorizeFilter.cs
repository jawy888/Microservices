using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskScheduler.Web.Configs;
using TaskScheduler.Web.Infrastructure.Attr;

namespace TaskScheduler.Web.Infrastructure.Filters
{
    public class TaskAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly AppSettings _appSettings;
        public TaskAuthorizeFilter(IHttpContextAccessor accessor,IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _accessor = accessor;
            _appSettings = optionsSnapshot.Value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return;
            if (((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
                .CustomAttributes.Any(x => x.AttributeType == typeof(TaskAuthorAttribute))
                && context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != _appSettings.SuperToken)
            {
                context.Result = new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(new
                    {
                        status = false,
                        msg = "您没有操作权限!"
                    }),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
        }
    }
}
