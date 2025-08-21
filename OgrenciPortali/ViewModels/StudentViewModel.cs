using Shared.DTO;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public class MyCoursesViewModel
    {
        [Display(Name = "Öğrenci Adı")] public string StudentName { get; set; }

        [Display(Name = "Öğrenci No")] public string StudentNumber { get; set; }


        [Required] public CourseSummaryViewModel Summary { get; set; }

        public List<CourseDetailViewModel> Courses { get; set; } = new List<CourseDetailViewModel>();
    }

    public class CourseSummaryViewModel
    {
        [Display(Name = "Toplam Dersler")]
        [Range(0, int.MaxValue, ErrorMessage = "Toplam ders sayısı 0'dan küçük olamaz.")]
        public int TotalCount { get; set; }

        [Display(Name = "Approved Courses")]
        [Range(0, int.MaxValue, ErrorMessage = "Kabul edilen ders sayısı 0'dan küçük olamaz.")]
        public int ApprovedCount { get; set; }

        [Display(Name = "Pending Courses")]
        [Range(0, int.MaxValue, ErrorMessage = "Bekleyen ders sayısı 0'dan küçük olamaz.")]
        public int PendingCount { get; set; }

        [Display(Name = "Rejected Courses")]
        [Range(0, int.MaxValue, ErrorMessage = "Reddedilen ders sayısı 0'dan küçük olamaz.")]
        public int RejectedCount { get; set; }
    }

    public class CourseDetailViewModel
    {
        [Display(Name = "Ders Kodu")]
        [Required(ErrorMessage = "Ders Kodu Gereklidir.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Ders kodu 4 - .")]
        public string CourseCode { get; set; }

        [Display(Name = "Course Name")]
        [Required(ErrorMessage = "Course Name cannot be empty.")]
        [StringLength(100)]
        public string CourseName { get; set; }

        [Display(Name = "Instructor")]
        [Required(ErrorMessage = "Instructor's name is required.")]
        public string TeacherFullName { get; set; }

        [Display(Name = "Credits")]
        [Range(0, 15, ErrorMessage = "Credits must be a positive number (typically 0-15).")]
        public int Credits { get; set; }

        [Display(Name = "Approval Status")]
        [Required]
        public string ApprovalStatusText { get; set; }

        [Display(Name = "Semester")]
        [Required]
        public string SemesterName { get; set; }

        [Display(Name = "Day")] public string DayOfWeek { get; set; }

        [Display(Name = "Time")] public string TimeSlot { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }
    }
}