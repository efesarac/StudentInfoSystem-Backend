using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StuSystem.Data;
using StuSystem.DTOs;
using StuSystem.Models;

namespace StuSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CoursesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            var courseDTOs = _mapper.Map<IEnumerable<CourseDTO>>(courses);
            return Ok(courseDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            var courseDTO = _mapper.Map<CourseDTO>(course);
            return Ok(courseDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, CourseDTO courseDTO)
        {
            if (id != courseDTO.CourseId)
            {
                return BadRequest();
            }

            var course = _mapper.Map<Course>(courseDTO);
            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<CourseDTO>> PostCourse(CourseDTO courseDTO)
        {
            var course = _mapper.Map<Course>(courseDTO);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var createdCourseDTO = _mapper.Map<CourseDTO>(course);
            return CreatedAtAction("GetCourse", new { id = course.CourseId }, createdCourseDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
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

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}