namespace StuSystem.Models
{
    public class Course
    {

        public int CourseId { get; set; }
        public int Credits { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();


    }
}
