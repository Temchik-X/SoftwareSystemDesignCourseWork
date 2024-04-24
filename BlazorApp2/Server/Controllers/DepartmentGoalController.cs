using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp2.Server.Data;
using BlazorApp2.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazorApp2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentGoalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepartmentGoalController> _logger;

        public DepartmentGoalController(ApplicationDbContext context, ILogger<DepartmentGoalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentGoal>>> GetDepartmentGoals()
        {
            try
            {
                var departmentGoals = await _context.DepartmentGoals.ToListAsync();
                _logger.LogInformation("Retrieved list of departmentGoals successfully.");
                return Ok(departmentGoals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching DepartmentGoal");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<DepartmentGoal>> AddDepartmentGoal(DepartmentGoal departmentGoal)
        {
            if (departmentGoal == null)
            {
                return BadRequest("Department goal data is null");
            }

            try
            {
                _context.DepartmentGoals.Add(departmentGoal);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDepartmentGoals), new { id = departmentGoal.id }, departmentGoal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding departmentGoal");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentGoal(int id)
        {
            try
            {
                var departmentGoal = await _context.DepartmentGoals.FindAsync(id);
                if (departmentGoal == null)
                {
                    return NotFound();
                }

                _context.DepartmentGoals.Remove(departmentGoal);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting departmentGoal");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
