    using System;
    using System.ComponentModel.DataAnnotations;

    namespace OgrenciPortali.Models
{
    public class Semester : BaseClass
    {
        [Key] 
        public Guid SemesterId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Dönem adı alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Dönem adı en fazla 50 karakter olabilir.")]
        public string SemesterName { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
    }
}