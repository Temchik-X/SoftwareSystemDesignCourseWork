using BlazorApp2.Server.Data;
using BlazorApp2.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ApplicationDbContext context, ILogger<CourseController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses(int userId)
        {
            try
            {
                var courses = await _context.Courses
                    .Where(c => c.user_id == userId).ToListAsync();
                _logger.LogInformation("Retrieved list of courses successfully.");
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Course");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Course>> AddCourse(Course course)
        {
            if (course == null)
            {
                return BadRequest("Course data is null");
            }

            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCourses), new { userId = course.user_id }, course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(int id, Course course)
        {
            if (id != course.id)
            {
                return BadRequest("Course ID mismatch");
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.id == id);
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
