var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// 🔥 ANA SAYFA = index.html
app.UseDefaultFiles();
app.UseStaticFiles();

// 🔥 Swagger sadece development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // sadece /swagger
    });
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();