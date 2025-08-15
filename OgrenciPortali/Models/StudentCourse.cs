using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciPortali.Models
{
    public class StudentCourse : BaseClass
    {
        [Key, Column(Order = 0)] public Guid StudentId { get; set; }

        [Key, Column(Order = 1)] public Guid OfferedCourseId { get; set; }

        [Required] public ApprovalStatus ApprovalStatus { get; set; }

        [ForeignKey("StudentId")] public virtual User Student { get; set; }

        [ForeignKey("OfferedCourseId")] public virtual OfferedCourse OfferedCourse { get; set; }
    }
}