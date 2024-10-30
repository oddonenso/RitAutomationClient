using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Data;
using SuperServerRIT.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� ���� ������
builder.Services.AddDbContext<Connection>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ���������� MediatR
builder.Services.AddMediatR(typeof(Program));

// ��������� ����������� � ������������ Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� Singleton ��� RabbitMQ � HostedService ��� ��������� ���������
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqHostedService>();

// ��������� ����� JWT �� ������������
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

// ��������� �������������� � JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // ���������� �� false ��� ���������� ������������
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
        ClockSkew = TimeSpan.Zero // ������� �������� ��� ������
    };
});

// ��������� ������ ��� ������ � JWT
builder.Services.AddScoped<JwtService>();

// ��������� CORS ��� ���������� WPF-�������
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWpfClient",
        builder => builder
            .WithOrigins("http://localhost") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); // ��������� �������� ������ ������������ (��������, ������)
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ���������� CORS
app.UseCors("AllowWpfClient");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
