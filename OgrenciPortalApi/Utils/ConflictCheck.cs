using System;
using System.Collections.Generic;
using System.Linq;
using OgrenciPortalApi.Models;
using Shared.DTO;

namespace OgrenciPortalApi.Utils
{
    public class ConflictCheck
    {
        private readonly OgrenciPortalApiDB _db = new OgrenciPortalApiDB();

        private List<ConflictCheckDTO> GetStudentCourses(Guid studentId)
        {
            return _db.StudentCourses
                .Where(sc => sc.StudentId == studentId && sc.OfferedCourses.Semesters.IsActive)
                .Select(sc => new ConflictCheckDTO
                {
                    CourseId = sc.OfferedCourseId,
                    CourseName = sc.OfferedCourses.Courses.CourseName,
                    DayOfWeek = sc.OfferedCourses.DayOfWeek,
                    // Önceki hatanın düzeltildiğinden emin olalım:
                    StartTime = sc.OfferedCourses.StartTime,
                    EndTime = sc.OfferedCourses.EndTime
                })
                .ToList();
        }

        public bool CanEnroll(ConflictCheckDTO model, Guid userId)
        {
            // GetConflictingCourse metodunu kullanarak bu metodu daha basit hale getirebiliriz.
            return GetConflictingCourse(model, userId) == null;
        }

        /// <summary>
        /// Yeni bir dersin, öğrencinin mevcut programıyla çakışıp çakışmadığını kontrol eder
        /// ve çakışma varsa çakışan dersi döndürür.
        /// </summary>
        /// <param name="model">Kontrol edilecek yeni dersin bilgileri.</param>
        /// <param name="userId">Öğrencinin ID'si.</param>
        /// <returns>Çakışma varsa çakışan dersi, yoksa null döner.</returns>
        public ConflictCheckDTO GetConflictingCourse(ConflictCheckDTO model, Guid userId)
        {
            // Öğrencinin mevcut programını al
            var existingCourses = GetStudentCourses(userId);

            var sameDayCourses = existingCourses.Where(c => c.DayOfWeek == model.DayOfWeek);

            foreach (var existingCourse in sameDayCourses)
            {
                // Çakışma koşulu:
                // (Yeni Dersin Başlangıcı < Mevcut Dersin Bitişi) VE (Mevcut Dersin Başlangıcı < Yeni Dersin Bitişi)
                if (model.StartTime < existingCourse.EndTime && existingCourse.StartTime < model.EndTime)
                {
                    return existingCourse; // Çakışma bulundu, çakışan dersi döndür.
                }
            }

            return null; // Hiçbir dersle çakışma bulunamadı.
        }
    }
}