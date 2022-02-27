using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using ToDo.API.Contexts;
using ToDo.API.Interfaces;
using ToDo.API.Models;
using ToDo.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string defaultConnectionString;
JwtSettings jwtSettings;
if (builder.Environment.IsDevelopment())
{
    jwtSettings = new JwtSettings
    {
        Audience = builder.Configuration.GetValue<string>("Jwt:Audience"),
        Issuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
        Key = builder.Configuration.GetValue<string>("Jwt:Key"),
        ExpireMinutes = builder.Configuration.GetValue<int>("Jwt:ExpireMinutes")
    };
    defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    jwtSettings = new JwtSettings
    {
        Audience = Environment.GetEnvironmentVariable("Jwt:Audience") ?? throw new NullReferenceException("Jwt:Audience is missing"),
        Issuer = Environment.GetEnvironmentVariable("Jwt:Issuer") ?? throw new NullReferenceException("Jwt:Issuer is missing"),
        Key = Environment.GetEnvironmentVariable("Jwt:Key") ?? throw new NullReferenceException("Jwt:Key is missing"),
        ExpireMinutes = builder.Configuration.GetValue<int>("Jwt:ExpireMinutes")
    };
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
builder.Services.AddScoped(_ => jwtSettings);
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
    swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddCors(corsOptions => corsOptions.AddPolicy("cors", corsPolicyBuilder =>
{
    corsPolicyBuilder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
