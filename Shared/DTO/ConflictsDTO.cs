using System;

namespace Shared.DTO
{
    public class ConflictCheckDTO
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}