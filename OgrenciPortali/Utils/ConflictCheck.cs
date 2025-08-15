using System;
using System.Collections.Generic;
using System.Linq;
using OgrenciPortali.Models;
using OgrenciPortali.ViewModels;

namespace OgrenciPortali.Utils
{

    public class ConflictCheck
    {
        private readonly OgrenciPortalContext _db = new OgrenciPortalContext();

        private List<ConflictCheckViewModel> GetStudentCourses(Guid studentId)
        {
            //SELECT OfferedCourses.DayOfWeek, OfferedCourses.StartTime, OfferedCourses.EndTime 
            // FROM StudentCourses JOIN OfferedCourses ON StudentCourses.OfferedCourseId = OfferedCourses.Id
            // WHERE StudentCourses.StudentId= studentId

            return _db.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new ConflictCheckViewModel
                {
                    DayOfWeek = sc.OfferedCourse.DayOfWeek,
                    EndTime = sc.OfferedCourse.StartTime,
                    StartTime = sc.OfferedCourse.EndTime
                })
                .ToList();
        }

        public bool CanEnroll(ConflictCheckViewModel model, Guid userId)
        {
            var courses = GetStudentCourses(userId);
            var sameDayCourses = courses.Where(c => c.DayOfWeek == model.DayOfWeek);

            foreach (var course in sameDayCourses)
            {
                if (TimeSpan.Compare(model.StartTime, course.EndTime) < 0 &&
                    TimeSpan.Compare(course.StartTime, model.EndTime) < 0)
                    return false;
            }

            return true;
        }
    }
}