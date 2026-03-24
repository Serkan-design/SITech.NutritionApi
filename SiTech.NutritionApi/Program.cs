var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔥 Swagger sadece development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// 🔥 STATIC FILE (index.html garanti)
app.UseDefaultFiles();
app.UseStaticFiles();

// 🔥 Swagger sadece development ve /swagger altında
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // 🔥 ROOT'U ELE GEÇİREMEZ
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nutrition API V1");
    });
}

// 🔥 HTTPS KAPALI (docker için doğru)
/// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();