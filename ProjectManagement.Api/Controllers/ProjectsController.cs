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
    // �������, �����������, ��� ������ ����� �������� ������������ API.

    [Route("api/[controller]")]
    // ���������� ������� ��� �������� API (� ������ ������ "api/Projects").

    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        // �����������, ���������������� ����������� ProjectService ����� ��������� ������������ (DI).
        {
            _projectService = projectService;
        }

        [HttpGet]
        // ���������, ��� ������ ����� ������������ HTTP GET-�������.
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        // ����� ��� ��������� ������ ���� ��������.
        {
            var projects = await _projectService.GetProjectsAsync();
            // ����������� ��������� � ������� ��� ��������� ������ ��������.
            return Ok(projects);
            // ���������� HTTP-����� 200 (OK) � ������� ��������.
        }

        [HttpGet("{id}")]
        // ���������, ��� ������ ����� ������������ HTTP GET-������� �� ID.
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            // ���� ������ �� ������, ������������ HTTP-����� 404 (Not Found).
            return Ok(project);
        }

        [HttpGet("{id}/tasks")]
        // ���������, ��� ������ ����� ������������ HTTP GET-������� �� �������� "/api/projects/{id}/tasks".
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksForProject(int id)
        {
            var tasks = await _projectService.GetTasksForProjectAsync(id);
            if (tasks == null || !tasks.Any())
                return NotFound($"No tasks found for project with ID {id}.");
            // ���������� HTTP-����� 404 (Not Found), ���� ����� ���.
            return Ok(tasks);
            // ���������� HTTP-����� 200 (OK) � ������� �����.
        }

        [HttpPost]
        // ���������, ��� ������ ����� ������������ HTTP POST-�������.
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto projectDto)
        {
            await _projectService.AddProjectAsync(projectDto);
            return CreatedAtAction(nameof(GetProject), new { id = projectDto.Id }, projectDto);
            // ���������� HTTP-����� 201 (Created) � URL ���������� �������.
        }

        [HttpPut("{id}")]
        // ���������, ��� ������ ����� ������������ HTTP PUT-������� ��� ����������.
        public async Task<IActionResult> UpdateProject(int id, CreateProjectDto projectDto)
        {
            if (id != projectDto.Id) return BadRequest();
            await _projectService.UpdateProjectAsync(projectDto);
            return NoContent();
            // ���������� HTTP-����� 204 (No Content) ����� ��������� ����������.
        }

        [HttpDelete("{id}")]
        // ���������, ��� ������ ����� ������������ HTTP DELETE-�������.
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return NoContent();
                // ���������� HTTP-����� 204 (No Content) ����� ��������� ��������.
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
                // � ������ ������ ���������� HTTP-����� 409 (Conflict) � ��������� ������.
            }
        }
    }
}
