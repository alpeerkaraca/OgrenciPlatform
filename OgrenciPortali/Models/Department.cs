    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OgrenciPortali.Models
{
    public class Department : BaseClass
    {
        [Key]
        public Guid DepartmentId { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(10, ErrorMessage = "Bölüm kodu en fazla 10 karakter olabilir.")]
        public string DepartmentCode { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Bölüm adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } 

        public ICollection<User> Students { get; set; }

        public ICollection<Course> Lessons { get; set; }
    }
}