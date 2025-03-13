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
// ������������� ������ ����������.

// ����������� ��������
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DefaultConnection")));
// ��������� DbContext � �������������� ���� ������ � ������.

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
// ����������� ����������� ��������.
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
// ����������� ����������� �����.
builder.Services.AddScoped<ProjectService>();
// ����������� ������� ��������.
builder.Services.AddScoped<TaskService>();
// ����������� ������� �����.

builder.Services.AddControllers();
// ���������� ������������.

builder.Services.AddSwaggerGen();
// ���������� Swagger ��� ������������ API.

var app = builder.Build();
// ������ ����������.

// ��������� middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // ������������� �������� ����������.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
        c.RoutePrefix = string.Empty;
        // Swagger ����� �������� �� ������� ��������.
    });
}

app.UseHttpsRedirection();
// ��������������� HTTP �� HTTPS.

app.MapControllers();
// ������������� ������������.

app.Run();
// ������ ����������.
