using System;
using System.Collections.Generic;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortali.ViewModels
{
    public class EnrollmentPageViewModel
    {
        public List<EnrollDTO> EnrollableList { get; set; }
        public List<EnrollDTO> PendingCourses { get; set; }
    }

    public class EnrollViewModel
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

        public List<EnrollDTO> EnrollableList { get; set; } = new List<EnrollDTO>();
        public List<EnrollDTO> PendingCourses { get; set; }
    }

    public class AdvisorApprovalViewModel
    {
        public Guid StudentId { get; set; }
        public Guid OfferedCourseId { get; set; }
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string DepartmentName { get; set; }
        public string SemesterName { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public DateTime RequestDate { get; set; }

        public List<AdvisorApprovalViewModel> PendingApprovals { get; set; } = new List<AdvisorApprovalViewModel>();
    }
}