var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 🔥 SWAGGER EKLE
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔥 STATIC FILE
app.UseDefaultFiles();
app.UseStaticFiles();

// 🔥 SWAGGER AKTİF
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();