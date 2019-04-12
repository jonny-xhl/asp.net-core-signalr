# asp.net-core-signalr
基于asp net core signalr 实现简单的demo,并采用授权机制。
### 使用JWT进行授权认证
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
