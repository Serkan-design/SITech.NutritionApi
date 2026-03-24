var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ❌ Swagger sadece developmentta
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔥 FRONTEND
app.UseDefaultFiles();   // index.html açar
app.UseStaticFiles();   // wwwroot aktif

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();