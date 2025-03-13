using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Инициализация сборки приложения.

// Регистрация сервисов
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Настройка DbContext с использованием базы данных в памяти.

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
        };

        //// Обработка события 401
        //options.Events = new JwtBearerEvents
        //{
        //    OnChallenge = async context =>
        //    {
        //        context.HandleResponse();
        //        context.Response.StatusCode = 401;
        //        context.Response.ContentType = "application/json";
        //        await context.Response.WriteAsync("{\"error\":\"Unauthorized access\"}");
        //    }
        //};
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    options.AddPolicy("RequireRoleUser", policy => policy.RequireRole("User"));
});

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
// Регистрация репозитория проектов.
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
// Регистрация репозитория задач.
builder.Services.AddScoped<ProjectService>();
// Регистрация сервиса проектов.
builder.Services.AddScoped<TaskService>();
// Регистрация сервиса задач.

builder.Services.AddControllers();
// Добавление контроллеров.

builder.Services.AddSwaggerGen();
// Добавление Swagger для документации API.

var app = builder.Build();
// Сборка приложения.

// Настройка middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Использование страницы разработки.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
        c.RoutePrefix = string.Empty;
        // Swagger будет доступен на главной странице.
    });
}

app.UseHttpsRedirection();
// Перенаправление HTTP на HTTPS.

app.UseAuthorization();

app.MapControllers();
// Маршрутизация контроллеров.

// Вызов инициализации предопределенных данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}

app.Run();
// Запуск приложения.
