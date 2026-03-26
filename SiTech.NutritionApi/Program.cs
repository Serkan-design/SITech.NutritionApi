var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// 🔥 STATIC FILES
app.UseDefaultFiles();
app.UseStaticFiles();

// 🔥 ROOT ZORLA INDEX
app.MapFallbackToFile("index.html");

// 🔥 SWAGGER
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();