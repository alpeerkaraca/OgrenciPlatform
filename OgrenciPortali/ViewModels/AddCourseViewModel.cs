using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OgrenciPortali.ViewModels
{
    public class AddCourseViewModel
    {
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Ders Adı Girilmesi Zorunludur")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Ders Kodu Girilmesi Zorunludur")]
        [StringLength(10, ErrorMessage = "Ders kodu en fazla 10 karakter olabilir.")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Kredi Miktarı Girilmesi Zorunludur")]
        [Range(1, 10, ErrorMessage = "Kredi 1 ile 10 arasında olmalıdır.")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Dersin verileceği bölümün girilmesi zorunludur.")]
        [Display(Name = "Bölüm")]
        public Guid DepartmentId { get; set; }

        public SelectList DepartmentList { get; set; }
    }

    public class EditCourseViewModel
    {
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Ders Adı Girilmesi Zorunludur")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Ders Kodu Girilmesi Zorunludur")]
        [StringLength(10, ErrorMessage = "Ders kodu en fazla 10 karakter olabilir.")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Kredi Miktarı Girilmesi Zorunludur")]
        [Range(1, 10, ErrorMessage = "Kredi 1 ile 10 arasında olmalıdır.")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Dersin verileceği bölümün girilmesi zorunludur.")]
        [Display(Name = "Bölüm")]
        public Guid DepartmentId { get; set; }

        public SelectList DepartmentList { get; set; }
        public string Message { get; set; }
    }
}