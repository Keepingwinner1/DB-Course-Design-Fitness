using System;
using Oracle.ManagedDataAccess.Client;
using Fitness.DAL.Core;
using System.Data;
using Fitness.BLL;
using Fitness.BLL.Interfaces;
using Fitness.BLL.Core;
using Fitness.Models;
using Fitness.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add BLL Singleton
builder.Services.AddSingleton<IFoodPlanBLL, FoodPlanBLL>();
builder.Services.AddSingleton<IMealRecordBLL, MealRecordBLL>();
builder.Services.AddSingleton<IFitnessALGuideBLL, FitnessAIGuideBLL>();
builder.Services.AddSingleton<IUserBLL, UserBLL>();
builder.Services.AddSingleton<IVigorTokenBLL, VigorTokenBLL>();
builder.Services.AddSingleton<ICourseBLL, CourseBLL>();
builder.Services.AddSingleton<IPostBLL, PostBLL>();
builder.Services.AddSingleton<ICommentBLL, CommentBLL>();
builder.Services.AddSingleton<IMessageBLL, MessageBLL>();
builder.Services.AddSingleton<IVigorTokenBLL, VigorTokenBLL>(); // 这行是重复的，可能需要去掉

// 添加 IMemoryCache 服务
builder.Services.AddMemoryCache();

// 注册 SignalR 服务
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    //加配置可以传给客户端对象，否则只能传字符串
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});
;

// 注册 VerificationHelper 服务
builder.Services.AddSingleton<VerificationHelper>();

// 配置 CORS（跨域资源共享）: 因为 SignalR 需要支持跨域请求。
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // 必须允许凭据来支持SignalR
    });
});

// 注册 DatabasePingService
builder.Services.AddHostedService<DatabasePingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
};

// 允许的域名
/*webSocketOptions.AllowedOrigins.Add("https://app.example.com");
webSocketOptions.AllowedOrigins.Add("https://www.app.example.com");
*/
// 开发环境的域名
webSocketOptions.AllowedOrigins.Add("http://localhost:5173");

app.UseWebSockets(webSocketOptions);

// 配置中间件顺序
app.UseRouting();      // 先定义路由

app.UseCors();         // 使用全局CORS策略

// 如果有身份验证，应该在这里添加 app.UseAuthentication();

app.UseAuthorization(); // 授权中间件

// 配置 SignalR 路由
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub"); // 映射 SignalR hub 到指定路径
    endpoints.MapControllers();            // 映射控制器
});

// 运行应用
app.Run();
