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
    public class HrProcessController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HrProcessController> _logger;

        public HrProcessController(ApplicationDbContext context, ILogger<HrProcessController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HrProcess>>> GetHrProcesses()
        {
            try
            {
                var hrProcesses = await _context.HrProcesses.ToListAsync();
                _logger.LogInformation("Retrieved list of hrProccesses successfully.");
                return Ok(hrProcesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching HR processes");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<HrProcess>> AddHrProcess(HrProcess hrProcess)
        {
            if (hrProcess == null)
            {
                return BadRequest("HrProcess data is null");
            }

            try
            {
                _context.HrProcesses.Add(hrProcess);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetHrProcesses), new { id = hrProcess.id }, hrProcess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding hrProcess");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHrProcess(int id)
        {
            try
            {
                var hrProcess = await _context.HrProcesses.FindAsync(id);
                if (hrProcess == null)
                {
                    return NotFound();
                }

                _context.HrProcesses.Remove(hrProcess);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hrProcess");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
