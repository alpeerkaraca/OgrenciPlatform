using System;
using System.Collections.Generic;

namespace OgrenciPlatform.Shared.DTO
{
    public class EnrollDTO
    {
        public Guid OfferedCourseId { get; set; }
        public Guid? CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string DepartmentName { get; set; }
        public string TeacherName { get; set; }
        public string SemesterName { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Quota { get; set; }
        public int Credit { get; set; }
        public int CurrentUserCount { get; set; } = 0;
    }

    public class EnrollPageDTO
    {
        public List<EnrollDTO> EnrollableList { get; set; }
        public List<EnrollDTO> PendingCourses { get; set; }
    }

    public class MyCoursesDTO
    {
        public SummaryDto Summary { get; set; }
        public List<MyCourseDto> Courses { get; set; }
    }

    public class SummaryDto
    {
        public int TotalCount { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }
    }

    public class MyCourseDto
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }

        public string TeacherFullName { get; set; }
        public string SemesterName { get; set; }

        public string DayOfWeek { get; set; }
        public string TimeSlot { get; set; }

        public int ApprovalStatus { get; set; }
        public string ApprovalStatusText { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StudentDetailDto
    {
        public string FullName { get; set; }
        public string StudentNo { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<StudentCourseInfoDto> StudentCourses { get; set; }
    }

    public class StudentCourseInfoDto
    {
        public Guid StudentId { get; set; }
        public Guid OfferedCourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string SemesterName { get; set; }

        public int ApprovalStatus { get; set; }
        public DateTime RequestDate { get; set; }
    }
}