using OgrenciPortali.Models;
using System;

namespace OgrenciPortali.ViewModels
{
    public class ConflictCheckViewModel
    {
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}