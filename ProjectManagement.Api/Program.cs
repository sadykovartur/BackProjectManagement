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
// ������������� ������ ����������.

// ����������� ��������
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// ��������� DbContext � �������������� ���� ������ � ������.

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

        //// ��������� ������� 401
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

app.UseAuthorization();

app.MapControllers();
// ������������� ������������.

// ����� ������������� ���������������� ������
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}

app.Run();
// ������ ����������.
