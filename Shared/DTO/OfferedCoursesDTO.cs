using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Shared.Enums;

namespace Shared.DTO
{
    public class ListOfferedCoursesDTO
    {
        public Guid OfferedCourseId { get; set; }
        public string DepartmentName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string SemesterName { get; set; }
        public string TeacherFullName { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public string Classroom { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DaysOfWeek DayOfWeek { get; set; }
    }

    public class AddOfferedCourseDTO
    {
        public int Quota { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid CourseId { get; set; }
        public Guid SemesterId { get; set; }
        public Guid AdvisorId { get; set; }
        public DaysOfWeek DayOfWeek { get; set; }
        public int CourseYear { get; set; } = 1;
        public string Classroom { get; set; }
        public IEnumerable<SelectListItem> CourseList { get; set; }
        public IEnumerable<SelectListItem> SemesterList { get; set; }
        public IEnumerable<SelectListItem> AdvisorList { get; set; }
        public IEnumerable<SelectListItem> DaysOfWeek { get; set; }
    }

    public class EditOfferedCoursesDTO
    {
        public Guid OfferedCourseId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public Guid SemesterId { get; set; }
        public Guid AdvisorId { get; set; }
        public int Quota { get; set; }
        public int DayOfWeek { get; set; }
        public int CourseYear { get; set; }
        public string Classroom { get; set; }


        public IEnumerable<SelectListItem> CourseList { get; set; }
        public IEnumerable<SelectListItem> SemesterList { get; set; }
        public IEnumerable<SelectListItem> AdvisorList { get; set; }
        public IEnumerable<SelectListItem> DaysOfWeek { get; set; }
    }
}