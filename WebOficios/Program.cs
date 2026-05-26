using Microsoft.EntityFrameworkCore;
using Oficios.Abstractions;
using Oficios.DataAccess;
using Oficios.Repository;
using Oficios.Services;
using OficiosApplications;
using OficiosDataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbDataAccess>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            o => o.MigrationsAssembly("Oficios.WebApi"));
    options.UseLazyLoadingProxies();
});

builder.Services.AddScoped(typeof(IStringService), typeof(StringService));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IApplication<>), typeof(Application<>));
builder.Services.AddScoped(typeof(IDbContext<>), typeof(DbContext<>));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
