using System;
using System.ComponentModel.DataAnnotations;

namespace OgrenciPortali.Models
{
    public abstract class BaseClass
    {
        [ScaffoldColumn(false)] public DateTime CreatedAt { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(100)]
        public string CreatedBy { get; set; }

        [ScaffoldColumn(false)] public DateTime UpdatedAt { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(100)]
        public string UpdatedBy { get; set; }

        [ScaffoldColumn(false)] public bool IsDeleted { get; set; }

        [ScaffoldColumn(false)] public DateTime? DeletedAt { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(100)]
        public string DeletedBy { get; set; }
    }
}