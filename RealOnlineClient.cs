using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.SignalRdemo
{
    public class RealOnlineClient
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// 身份认证名称
        /// </summary>
        public string IdentityName { get; set; }
        /// <summary>
        /// 连接实时服务的时间
        /// </summary>
        public DateTime ConnecServerTime { get; set; }
    }
}
