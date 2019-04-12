using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Study.SignalRdemo.Hubs
{
    /// <summary>
    /// 添加授权认证
    /// </summary>
    [Authorize(Policy = "Hubs")]
    public class MessageHub : Hub
    {
        private readonly MessageAppService _messageApp;

        public MessageHub(MessageAppService messageApp)
        {
            _messageApp = messageApp;
        }
        

        /// <summary>
        /// 重写Hub连接时方法
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            var connId = Context.ConnectionId;
            var name = Context.User.Identity.Name;
            var real = _messageApp.IsOnline(name);
            var client = new RealOnlineClient
            {
                ConnectionId = connId,
                IdentityName = name,
                ConnecServerTime = DateTime.Now
            };
            if (real == null)
                _messageApp.AddClient(connId, client);
            else
            {
                //1、移除
                _messageApp.RemoveClient(real.ConnectionId);
                //2、新增
                _messageApp.AddClient(connId,client);
            }               
            base.OnConnectedAsync();
            //向客户端的updateCount推送消息
            Clients.All.SendAsync("updateCount", _messageApp.ClientsCount());
            //向客户端的getClient推送消息
            Clients.All.SendAsync("getClient", _messageApp.GetClients().Values.ToArray());
            return Task.CompletedTask;
        }

        /// <summary>
        /// 重写客户端断开方法
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connId = Context.ConnectionId;
            _messageApp.RemoveClient(connId);
            base.OnDisconnectedAsync(exception);
            //向客户端的getClient推送消息
            Clients.All.SendAsync("getClient", _messageApp.GetClients().Values.ToArray());
            return Task.CompletedTask;
        }
        
    }
}
