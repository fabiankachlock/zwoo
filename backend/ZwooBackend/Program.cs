using System.Net;
using System.Text;
using BackendHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZwooBackend;
using ZwooBackend.Controllers;

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
    o.Cookie.HttpOnly = true;
    o.Cookie.Domain = Globals.ZwooCookieDomain;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Globals.Logger.Info("Adding Swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Zwoo");
app.UseHttpsRedirection();
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

mail_thread.Start();

app.Run();

Globals.EmailQueue.Enqueue(new EmailData("", 0, "", ""));
mail_thread.Join();
