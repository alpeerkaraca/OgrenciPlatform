using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciPortali.Models
{
    public class Course : BaseClass
    {
        [Key]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Ders kodu alanı zorunludur.")]
        [StringLength(10, ErrorMessage = "Ders kodu en fazla 10 karakter olabilir.")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Ders adı alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ders adı en fazla 100 karakter olabilir.")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Kredi miktarı girilmesi zorunludur.")]
        [Range(1, 10, ErrorMessage = "Kredi 1 ile 10 arasında olmalıdır.")]
        public int Credits { get; set; }
        //public int RequiredYear { get; set; }
        
        /*          FK          */

        [Required(ErrorMessage = "Dersin verileceği bölümün girilmesi zorunludur.")]
        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
    }
}