var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// 🔥 STATIC FILE
app.UseDefaultFiles();
app.UseStaticFiles();

// ❌ swagger sadece development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
    });
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();