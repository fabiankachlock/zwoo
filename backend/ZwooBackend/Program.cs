using ZwooBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(s =>
{
    s.AddPolicy("zwoo-cors", p =>
    {
        p.WithOrigins("http://localhost:8080").AllowCredentials().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Globals.Logger.Info("Adding Swagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

webSocketOptions.AllowedOrigins.Add("http://localhost");

app.UseCors("zwoo-cors");
app.UseWebSockets(webSocketOptions);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
