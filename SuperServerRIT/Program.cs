using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Data;
using SuperServerRIT.Services;
using MediatR;
using SuperServerRIT.Commands;
using Microsoft.OpenApi.Models;
using System.Reflection; // Для получения пути к XML-документации
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Настройка контекста базы данных
builder.Services.AddDbContext<Connection>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);

// Регистрация репозитория
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

// Настройка контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger с учетом XML-документации
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; // Получаем имя файла на основе имени сборки
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Проверяем, существует ли файл
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SuperServerRitAPI V1", Version = "v1" });
});

// Регистрация сервисов для RabbitMQ
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqHostedService>();

// Настройка аутентификации JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Регистрация сервиса для JWT
builder.Services.AddScoped<JwtService>();

// Настройка CORS для локального WPF-клиента
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWpfClient", builder =>
        builder.WithOrigins("http://localhost")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

var app = builder.Build();

// Конфигурация для среды разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SuperServerRitAPI V1");
        c.RoutePrefix = "swagger";
    });
}

// Подключаем CORS
app.UseCors("AllowWpfClient");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
