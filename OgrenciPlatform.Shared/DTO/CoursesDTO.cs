using System;
using System.Collections.Generic;

namespace OgrenciPlatform.Shared.DTO
{
    public class ListCoursesDTO
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int Credits { get; set; }
        public string DepartmentName { get; set; }
    }

    public class AddCourseDTO
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
        public IEnumerable<DepartmentSelectionDTO> DepartmentsList { get; set; }
    }

    public class DepartmentSelectionDTO
    {
        public string DepartmentName { get; set; }
        public Guid DepartmentId { get; set; }
    }

    public class EditCourseDTO
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public Guid DepartmentId { get; set; }
        public IEnumerable<DepartmentSelectionDTO> DepartmentsList { get; set; }
    }
}