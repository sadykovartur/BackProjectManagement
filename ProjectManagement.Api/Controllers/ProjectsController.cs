using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    // јтрибут, указывающий, что данный класс €вл€етс€ контроллером API.

    [Route("api/[controller]")]
    // ќпредел€ет маршрут дл€ запросов API (в данном случае "api/Projects").

    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        //  онструктор, инициализирующий зависимость ProjectService через внедрение зависимостей (DI).
        {
            _projectService = projectService;
        }

        [HttpGet]
        // ”казывает, что данный метод обрабатывает HTTP GET-запросы.
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        // ћетод дл€ получени€ списка всех проектов.
        {
            var projects = await _projectService.GetProjectsAsync();
            // јсинхронное обращение к сервису дл€ получени€ списка проектов.
            return Ok(projects);
            // ¬озвращает HTTP-ответ 200 (OK) с данными проектов.
        }

        [HttpGet("{id}")]
        // ”казывает, что данный метод обрабатывает HTTP GET-запросы по ID.
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            // ≈сли проект не найден, возвращаетс€ HTTP-ответ 404 (Not Found).
            return Ok(project);
        }

        [HttpGet("{id}/tasks")]
        // ”казывает, что данный метод обрабатывает HTTP GET-запросы по маршруту "/api/projects/{id}/tasks".
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksForProject(int id)
        {
            var tasks = await _projectService.GetTasksForProjectAsync(id);
            if (tasks == null || !tasks.Any())
                return NotFound($"No tasks found for project with ID {id}.");
            // ¬озвращает HTTP-ответ 404 (Not Found), если задач нет.
            return Ok(tasks);
            // ¬озвращает HTTP-ответ 200 (OK) с данными задач.
        }

        [HttpPost]
        // ”казывает, что данный метод обрабатывает HTTP POST-запросы.
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto projectDto)
        {
            await _projectService.AddProjectAsync(projectDto);
            return CreatedAtAction(nameof(GetProject), new { id = projectDto.Id }, projectDto);
            // ¬озвращает HTTP-ответ 201 (Created) с URL созданного проекта.
        }

        [HttpPut("{id}")]
        // ”казывает, что данный метод обрабатывает HTTP PUT-запросы дл€ обновлени€.
        public async Task<IActionResult> UpdateProject(int id, CreateProjectDto projectDto)
        {
            if (id != projectDto.Id) return BadRequest();
            await _projectService.UpdateProjectAsync(projectDto);
            return NoContent();
            // ¬озвращает HTTP-ответ 204 (No Content) после успешного обновлени€.
        }

        [HttpDelete("{id}")]
        // ”казывает, что данный метод обрабатывает HTTP DELETE-запросы.
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return NoContent();
                // ¬озвращает HTTP-ответ 204 (No Content) после успешного удалени€.
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
                // ¬ случае ошибки возвращает HTTP-ответ 409 (Conflict) с описанием ошибки.
            }
        }
    }
}
