using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Study.SignalRdemo.Hubs;

namespace Study.SignalRdemo
{
    public class HomeController : Controller
    {
        /// <summary>
        /// MessageHub上下文，可以直接调用Hub中的方法
        /// </summary>
        private readonly IHubContext<MessageHub> _hub;
        private readonly MessageAppService _appService;
        public HomeController(IHubContext<MessageHub> hub,
            MessageAppService appService)
        {
            _hub = hub;
            _appService = appService;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }       
    }
}