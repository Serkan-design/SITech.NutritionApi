var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// 🔥 STATIC FILES
app.UseDefaultFiles();
app.UseStaticFiles();

// 🔥 ROOT → index.html zorla
app.MapFallbackToFile("index.html");

// 🔥 SWAGGER AYRI YOLDA
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();