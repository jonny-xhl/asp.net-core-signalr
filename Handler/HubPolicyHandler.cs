using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Study.SignalRdemo.Handler
{
    /// <summary>
    /// Message集线器授权策略处理类
    /// </summary>
    public class HubPolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        /// <summary>
        /// 授权方式(cookie\bearer\oauth\openid)
        /// </summary>
        private IAuthenticationSchemeProvider Schemes { get; set; }
        public HubPolicyHandler(IAuthenticationSchemeProvider scheme)
        {
            Schemes = scheme;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            //获取httpcontext上下文
            //var httpContext = (context.Resource as AuthorizationFilterContext)?.HttpContext;
            //if (httpContext != null)
            //{
            //    //获取默认授权方式
            //    var defaultScheme = await Schemes.GetDefaultAuthenticateSchemeAsync();
            //    if (defaultScheme != null)
            //    {
            //        //验证签发的授权信息
            //        var authResult = await httpContext?.AuthenticateAsync(defaultScheme.Name);
            //        if (authResult.Succeeded)
            //        {
            //            var name = httpContext.User.Identity.Name;
            //            var role = httpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
            //            context.Succeed(requirement);
            //        }
            //    }
            //}
            var user = context?.User;
            if (user!=null && user.HasClaim(c=>c.Issuer== "http://www.zlsoft.com"
            && c.Type==ClaimTypes.NameIdentifier || c.Type==ClaimTypes.Name || c.Type==ClaimTypes.Role))            
                context.Succeed(requirement);            
            else            
                context.Fail();
            return Task.CompletedTask;
        }
    }
}
