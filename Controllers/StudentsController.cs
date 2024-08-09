using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StuSystem.Data;
using StuSystem.Models;

namespace StuSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .ToListAsync();

            var result = students.Select(s => new
            {
                s.StudentId,
                s.Name,
                s.Email,
                s.DateOfBirth,
                s.EnrollmentDate,
                StudentCourses = s.StudentCourses.Select(sc => new
                {
                    sc.CourseId,
                    CourseName = sc.Course.Name,
                    CourseCredits = sc.Course.Credits,
                    CourseDescription = sc.Course.Description
                }).ToList()
            }).ToList();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            var result = new
            {
                student.StudentId,
                student.Name,
                student.Email,
                student.DateOfBirth,
                student.EnrollmentDate,
                StudentCourses = student.StudentCourses.Select(sc => new
                {
                    sc.CourseId,
                    CourseName = sc.Course.Name,
                    CourseCredits = sc.Course.Credits,
                    CourseDescription = sc.Course.Description
                }).ToList()
            };

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
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

            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
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