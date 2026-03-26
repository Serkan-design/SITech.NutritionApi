var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// ✅ STATIC FILES ÖNE AL
app.UseDefaultFiles();
app.UseStaticFiles();

// 🔥 SWAGGER EN ALTA
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();