using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.DTO
{
    public class AdvisorApprovalDTO
    {
        public List<StudentApprovalGroupDto> PendingStudentGroups { get; set; }
    }
    public class StudentApprovalGroupDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentNo { get; set; }
        public int TotalPendingCredits { get; set; } // Öğrencinin bekleyen toplam kredisi
        public List<ApprovalRequestDto> CourseRequests { get; set; }
    }

    public class ApprovalRequestDto
    {
        public Guid StudentId { get; set; }
        public Guid OfferedCourseId { get; set; }

        public string StudentNo { get; set; }
        public string StudentName { get; set; }

        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public string SemesterName { get; set; }

        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int ApprovalStatus { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public class StudentInfoDTO
    {
        public Guid StudentId { get; set; }
        public string StudentFullName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentDepartmentName { get; set; }
        public DateTime StudentCreateDate { get; set; }
        public string StudentNo { get; set; }
    }

    public class AdvisedStudentsDTO
    {
        public List<StudentInfoDTO> AdvisedStudents { get; set; }
    }

    public class ApprovalRequestModel
    {
        public List<Guid> StudentIds { get; set; }
        public List<Guid> OfferedCourseIds { get; set; }
        public ApprovalStatus NewStatus { get; set; }
    }
}