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
    public class IndividualGoalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndividualGoalController> _logger;

        public IndividualGoalController(ApplicationDbContext context, ILogger<IndividualGoalController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<IEnumerable<IndividualGoal>>> GetIndividualGoalsByEmployeeId(int employeeId)
        {
            try
            {
                var individualGoals = await _context.IndividualGoals
                    .Where(g => g.employee_id == employeeId)
                    .ToListAsync();

                return Ok(individualGoals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting individualGoals");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<IndividualGoal>> AddIndividualGoal(IndividualGoal individualGoal)
        {
            if (individualGoal == null)
            {
                return BadRequest("Individual goal data is null");
            }

            try
            {
                _context.IndividualGoals.Add(individualGoal);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetIndividualGoalsByEmployeeId), new { employeeId = individualGoal.employee_id }, individualGoal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding individualGoal");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIndividualGoal(int id)
        {
            try
            {
                var individualGoal = await _context.IndividualGoals.FindAsync(id);
                if (individualGoal == null)
                {
                    return NotFound();
                }

                _context.IndividualGoals.Remove(individualGoal);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting individualGoal");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
