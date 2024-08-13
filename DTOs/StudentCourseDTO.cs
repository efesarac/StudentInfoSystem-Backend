using StuSystem.Models;

namespace StuSystem.DTOs
{
    public class StudentCourseDTO
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
    }
}
