using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TaskScheduler.Web.Configs;
using TaskScheduler.Web.Infrastructure.Repository;

namespace TaskScheduler.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly AppSettings _appSettings;
        public HomeController(IMemoryCache memoryCache, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _cache = memoryCache;
            _appSettings = optionsSnapshot.Value;
        }
        public IActionResult Login()
        {
            var returnUrl = HttpContext.Request.Query["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Content("<script language='javaScript' type='text/javaScript'> window.parent.location.href = '/home/login';</script>",
                    "text/html");
            }
            var msg = _cache.Get("msg")?.ToString();
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.msg = msg;
                _cache.Remove("msg");
            }
            return View();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var _token = _appSettings.Token;
            var _superToken = _appSettings.SuperToken;
            if (!string.IsNullOrEmpty(token) && (_token == token || _superToken == token))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,token),
                };
                var tokenIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userToken = new ClaimsPrincipal(tokenIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userToken, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(45)
                });
            }
            else
            {
                _cache.Set("msg", string.IsNullOrEmpty(token) ? "请输入身份令牌" : "令牌不正确");
            }
            return Redirect("/");
        }

        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/home/login");
        }

        [Authorize]
        public IActionResult List()
        {
            return View();
        }
    }
}