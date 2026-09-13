using CryptoTrackingSystem.Infrastructure;
using CryptoTrackingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    var contentType = context.Request.ContentType;
    if (!string.IsNullOrEmpty(contentType) && !context.Request.Headers.ContainsKey("Accept"))
        context.Request.Headers.Accept = contentType;

    await next();
});

app.UseAuthorization();
app.MapControllers();
app.Run();
