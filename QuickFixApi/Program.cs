using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 👇 Configura el contexto de base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 👇 Habilita los controladores
builder.Services.AddControllers();

// 👇 Swagger (documentación y pruebas)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 👇 CORS para permitir peticiones desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 👇 Aplica migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 👇 Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 👇 Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// 👇 Ruteo
app.MapControllers();

app.Run();
