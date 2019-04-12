using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.SignalRdemo.Dtos
{
    /// <summary>
    /// 登陆传输对象
    /// </summary>
    public class UserLoginDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
