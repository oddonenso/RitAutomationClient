using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Data;
using SuperServerRIT.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Настройка контекста базы данных
builder.Services.AddDbContext<Connection>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Подключаем MediatR
builder.Services.AddMediatR(typeof(Program));

// Добавляем контроллеры и документацию Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем Singleton для RabbitMQ и HostedService для обработки сообщений
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqHostedService>();

// Настройка ключа JWT из конфигурации
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

// Настройка аутентификации с JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Установите на false для локального тестирования
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
        ClockSkew = TimeSpan.Zero // Убираем задержку для тестов
    };
});

// Добавляем сервис для работы с JWT
builder.Services.AddScoped<JwtService>();

// Настройка CORS для локального WPF-клиента
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWpfClient",
        builder => builder
            .WithOrigins("http://localhost") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); // Разрешаем передачу данных пользователя (например, токены)
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Подключаем CORS
app.UseCors("AllowWpfClient");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
