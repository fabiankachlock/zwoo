using System.Net;
using System.Text;
using BackendHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Quartz;
using Quartz.Impl;
using ZwooBackend;
using ZwooBackend.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(s => s.AddPolicy("Zwoo",
    b => b.WithOrigins(Globals.Cors).AllowAnyHeader().AllowAnyMethod()
        .AllowCredentials()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
    o.Cookie.Name = "auth";
    o.Cookie.HttpOnly = Globals.UseSsl;
    if (Globals.UseSsl)
    {
        o.Cookie.SameSite = SameSiteMode.None;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
    o.Cookie.Domain = Globals.ZwooCookieDomain;
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Method != "OPTIONS")
    {
        Globals.HttpLogger.Info($"[{context.Request.Method}] {context.Request.Path}");
    }
    await next.Invoke();
    if (context.Response.StatusCode >= 300)
    {
        Globals.HttpLogger.Info($"sending error response: {context.Response.StatusCode} ([{context.Request.Method}] {context.Request.Path})");
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Globals.Logger.Debug("adding swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
};
webSocketOptions.AllowedOrigins.Add(Globals.Cors);

app.UseCors("Zwoo");
app.UseWebSockets(webSocketOptions);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var mail_thread = new Thread(() =>
{
    while (true)
    {
        if (!Globals.EmailQueue.IsEmpty)
        {
            EmailData? data;
            if (Globals.EmailQueue.TryDequeue(out data))
            {
                if (data.Puid == 0)
                    break;
                EmailData.SendMail(data);
            }
        }
        else
        {
            Thread.Sleep(500);
        }
    }
});

var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
scheduler.Start();
scheduler.ScheduleJob(
    JobBuilder.Create<DatabaseCleanupJob>().WithIdentity("db_cleanup", "db").Build(),
    TriggerBuilder.Create().WithCronSchedule("0 1 1 1/1 * ? *").Build()); // Every Day at 00:01 UTC+1

mail_thread.Start();

app.Run();

Globals.EmailQueue.Enqueue(new EmailData("", 0, "", ""));
mail_thread.Join();
scheduler.Shutdown();
