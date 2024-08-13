namespace StuSystem.DTOs
{
    public class StudentDTO
    {

        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public ICollection<StudentCourseDTO> Courses { get; set; }
    }
}
