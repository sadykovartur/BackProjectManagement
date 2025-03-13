using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Инициализация сборки приложения.

// Регистрация сервисов
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DefaultConnection")));
// Настройка DbContext с использованием базы данных в памяти.

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

app.MapControllers();
// Маршрутизация контроллеров.

app.Run();
// Запуск приложения.
