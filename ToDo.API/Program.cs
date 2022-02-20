using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ToDo.API.Contexts;
using ToDo.API.Interfaces;
using ToDo.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string defaultConnectionString;
if (builder.Environment.IsDevelopment())
    defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
else
{
    // Use connection string provided at runtime by Heroku
    var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL")!;
    connectionUrl = connectionUrl.Replace("postgres://", string.Empty);
    var userPassSide = connectionUrl.Split("@")[0];
    var hostSide = connectionUrl.Split("@")[1];
    var user = userPassSide.Split(":")[0];
    var password = userPassSide.Split(":")[1];
    var host = hostSide.Split("/")[0];
    var database = hostSide.Split("/")[1].Split("?")[0];
    defaultConnectionString = $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}
builder.Services.AddDbContext<ToDoContext>(options => options.UseNpgsql(defaultConnectionString));
builder.Services.AddScoped<IToDoItemsService, ToDoItemsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo.API",
        Description = "API for ToDo application",
        Contact = new OpenApiContact { Name = "Rados³aw Makowski", Email = "", Url = new Uri("https://github.com/rmakowski/ToDo.Api") }
    });
    swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddCors(corsOptions => corsOptions.AddPolicy("cors", corsPolicyBuilder =>
{
    corsPolicyBuilder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoContext>();
        dbContext.Database.Migrate();
    }
    catch
    {
        // ignore
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("cors");
app.UseAuthorization();
app.MapControllers();
app.Run();
