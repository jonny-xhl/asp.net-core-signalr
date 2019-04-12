using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Study.SignalRdemo.Dtos;
using Study.SignalRdemo.Services;

namespace Study.SignalRdemo.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// 用户相关服务
        /// </summary>
        private readonly IUserService _service;
        public LoginController(IUserService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginOn([FromBody]UserLoginDto user)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    return Accepted();
                }
                else
                {
                    var userModel=_service.Authenticate(user.UserName, user.Password);
                    if (userModel == null)
                    {
                        return BadRequest(new { Error = "用户名或密码错误" });
                    }
                    return Ok(userModel);
                }
            }
            else
            {
                return BadRequest(new { Error="模型验证失败"});
            }
        }
    }
}