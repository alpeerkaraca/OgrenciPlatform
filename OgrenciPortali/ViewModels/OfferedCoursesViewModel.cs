using System;
using System.Collections.Generic;
using OgrenciPortali.Models;
using System.Web.Mvc;
using OgrenciPortali.DTOs;

namespace OgrenciPortali.ViewModels
{
    public class ListOfferedCoursesViewModel
    {
        public List<ListOfferedCoursesDTO> OfferedCoursesList;
    }

    public class AddOfferedCourseViewModel
    {
        public int Quota { get; set; }
        public Guid CourseId { get; set; }
        public Guid SemesterId { get; set; }
        public Guid TeacherId { get; set; }
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList SemesterList { get; set; }
        public SelectList AdvisorList { get; set; }
        public SelectList DaysList { get; set; }
    }

    public class EditOfferedCourseViewModel
    {
        public Guid OfferedCourseId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public Guid SemesterId { get; set; }
        public Guid AdvisorId { get; set; }
        public int Quota { get; set; }
        public int DayOfWeek { get; set; }
        public SelectList CourseList { get; set; }
        public SelectList SemesterList { get; set; }
        public SelectList AdvisorList { get; set; }
        public SelectList DaysList { get; set; }
    }
}