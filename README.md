# asp.net-core-signalr
基于asp net core signalr 实现简单的demo,并采用授权机制。
### 1、使用JWT进行授权认证
1. 添加授权自定义策略
``` .cs
services.AddAuthorization(options =>
{
    options.AddPolicy("Hubs", policy => policy.Requirements.Add(new PolicyRequirement()));
})
```
2. 设置认证方式(cookie、bearer、openid)
``` .cs
AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
```
3. 添加JWT认证机制
    + 设置验证参数
    ``` .cs
        x.TokenValidationParameters = new TokenValidationParameters
                    {
                        LifetimeValidator = (before, expires, token, param) =>
                        {                        
                            return expires > DateTime.UtcNow;
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),//Secret
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateActor = false,
                        ValidateLifetime = true
                    };
    ```
   + 为Jwt注册事件
      ``` .cs
      x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.HttpContext.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!(string.IsNullOrWhiteSpace(accessToken))
                                && path.StartsWithSegments("/hubs/message"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            //Token expired
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            else if(context.Exception.GetType()==typeof(SecurityTokenInvalidLifetimeException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
      ```
### 2、Signal
1. 注册
``` .cs
services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.EnableDetailedErrors = true;
}).AddMessagePackProtocol(configure =>
{
    //配置支持json和MessagePack两种方式传输
});
```
2. 使用
``` .cs
app.UseSignalR(builder => builder.MapHub<MessageHub>("/hubs/message"));
```
### 3、添加HUB
``` .cs
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
```
### 4、使用javascript客户端
``` .js
let token = JSON.parse(localStorage.getItem("UserInfo")).token;
//通过HubConnectionBuilder创建连接对象
let connection = new signalR.HubConnectionBuilder().withUrl("/hubs/message", {
accessTokenFactory: () => token
}).build();
connection.on("updateCount", (count) => {
hubData.count = count;
//getclient();
});
//注册监听客户端GetCLient方法
connection.on("getClient", (values) => {
//插入前清空当前clients
hubData.clients = [];
hubData.clients=values;
});
//监听连接开始逻辑控制，catch捕获异常回调
connection.start().then((res) => {
console.log("集线器：/hubs/message。连接成功。", res);
}).catch((err) => {
console.log("集线器：/hubs/message。连接失败。", err);
});
```
### 5、项目运行
1. 登陆界面
![Image](https://github.com/CQJonnyLin/asp.net-core-signalr/blob/master/doc/login.png)
2. 主页
![Image](https://github.com/CQJonnyLin/asp.net-core-signalr/blob/master/doc/Index.png)
