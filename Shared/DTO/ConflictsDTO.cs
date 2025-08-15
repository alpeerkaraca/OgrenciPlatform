using System;
using Shared.Enums;

namespace Shared.DTO
{
    public class ConflictCheckDTO
    {
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}