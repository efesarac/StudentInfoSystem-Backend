using AutoMapper;
using StuSystem.Models;
using StuSystem.DTOs;

namespace StuSystem.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Course, CourseDTO>();
            CreateMap<Student, StudentDTO>();
            CreateMap<CourseDTO, Course>();
            CreateMap<StudentDTO, Student>();
        }
    }
}