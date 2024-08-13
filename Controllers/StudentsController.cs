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
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .ToListAsync();

            var result = students.Select(student => _mapper.Map<StudentDTO>(student)).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<StudentDTO>(student);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentDTO studentDto)
        {
            if (id != studentDto.StudentId)
            {
                return BadRequest();
            }

            var student = _mapper.Map<Student>(studentDto);
            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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
        public async Task<ActionResult<StudentDTO>> PostStudent(StudentDTO studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);

            if (student.StudentCourses == null)
            {
                student.StudentCourses = new List<StudentCourse>();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var createdStudentDto = _mapper.Map<StudentDTO>(student);
            return CreatedAtAction("GetStudent", new { id = student.StudentId }, createdStudentDto);
        }

        [HttpPost("{studentId}/courses/{courseId}")]
        public async Task<IActionResult> AddCourseToStudent(int studentId, int courseId)
        {
            var student = await _context.Students.Include(s => s.StudentCourses).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            student.StudentCourses.Add(new StudentCourse { StudentId = studentId, CourseId = courseId });
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}