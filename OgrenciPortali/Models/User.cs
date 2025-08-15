using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciPortali.Models
{
    public class User : BaseClass
    {
        [Key] 
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string Surname { get; set; }

        [NotMapped]
        public string FullName => Name + " " + Surname;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Parola en az 8 karakter ve en fazla 100 karakter olmalıdır.", MinimumLength = 8)]
        public String Password { get; set; }

        [Required]
        public Roles Role { get; set; }

        [DefaultValue(true)] 
        public bool IsActive { get; set; } = true;

        public bool IsFirstLogin { get; set; } = true;

        /* Kullanıcı = Öğrenci => */

        public string StudentNo { get; set; }
        //public int Year { get; set; }

        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public Guid? AdvisorId { get; set; }
        [ForeignKey("AdvisorId")]
        public virtual User Advisor { get; set; }

        /* Kullanıcı = Danışman => */

        [InverseProperty("Advisor")]
        public virtual ICollection<User> AdvisedStudents { get; set; }

        /*-----------------------*/

        public virtual ICollection<StudentCourse> EnrolledCourses { get; set; }

        public virtual ICollection<OfferedCourse> TaughtCourses { get; set; }

        public User()
        {
            AdvisedStudents = new HashSet<User>();
            EnrolledCourses = new HashSet<StudentCourse>();
            TaughtCourses = new HashSet<OfferedCourse>();
        }
    }
}