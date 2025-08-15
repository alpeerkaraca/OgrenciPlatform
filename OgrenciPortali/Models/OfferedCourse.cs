using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciPortali.Models
{
    public class OfferedCourse : BaseClass
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")] public virtual Course Course { get; set; }
        [Required] public Guid? SemesterId { get; set; }
        [ForeignKey("SemesterId")] public virtual Semester Semester { get; set; }
        [Required] public Guid? TeacherId { get; set; }
        [ForeignKey("TeacherId")] public virtual User Teacher { get; set; }
        public int Quota { get; set; }
        public int CurrentUserCount { get; set; } = 0;
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


        public virtual ICollection<StudentCourse> EnrolledStudents { get; set; }
    }
}