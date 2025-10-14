using Implementacion.Api.Context;
using Implementacion.Api.Flyweight;
using Implementacion.Api.Singleton;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Agregamos Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Implementacion.Api",
        Version = "v1",
        Description = "API para gestión de productos, categorías y proveedores.",
        Contact = new OpenApiContact
        {
            Name = "Johann Yamil Jimenez Perez",
            Email = "23300022@uttt.edu.mx"
        }
    });
});

// Configurar EF Core con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));





builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ✅ Inicializar Singleton del Flyweight Factory
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SingletonConstruct<CategoryFlyweightFactory>.Initialize(dbContext);
}

// Activamos Swagger solo en desarrollo (o siempre si quieres)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Implementacion.Api v1");
        c.RoutePrefix = string.Empty; // Muestra Swagger en la raíz: https://localhost:xxxx/
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
