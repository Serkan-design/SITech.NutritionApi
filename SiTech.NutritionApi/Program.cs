var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// 🔥 STATIC FILE
app.UseDefaultFiles();
app.UseStaticFiles();

// ❌ SWAGGER YOK ARTIK

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();