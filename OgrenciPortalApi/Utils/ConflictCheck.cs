using System;
using System.Collections.Generic;
using System.Linq;
using OgrenciPortalApi.Models;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortalApi.Utils
{
    public class ConflictCheck
    {
        private readonly OgrenciPortalApiDB _db = new OgrenciPortalApiDB();

        private List<ConflictCheckDTO> GetStudentCourses(Guid studentId)
        {
            //SELECT OfferedCourses.DayOfWeek, OfferedCourses.StartTime, OfferedCourses.EndTime 
            // FROM StudentCourses JOIN OfferedCourses ON StudentCourses.OfferedCourseId = OfferedCourses.Id
            // WHERE StudentCourses.StudentId= studentId

            return _db.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new ConflictCheckDTO
                {
                    CourseId = sc.OfferedCourseId,
                    CourseName = sc.OfferedCourses.Courses.CourseName,
                    DayOfWeek = sc.OfferedCourses.DayOfWeek,
                    EndTime = sc.OfferedCourses.StartTime,
                    StartTime = sc.OfferedCourses.EndTime
                })
                .ToList();
        }

        public bool CanEnroll(ConflictCheckDTO model, Guid userId)
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