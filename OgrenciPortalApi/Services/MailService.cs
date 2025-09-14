using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OgrenciPortalApi.Utils;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace OgrenciPortalApi.Services
{
    public class MailService
    {
        private static readonly Lazy<MailService> _lazyInstance = new Lazy<MailService>(() => new MailService());
        public static MailService Instance => _lazyInstance.Value;
        private readonly string _host;
        private readonly int _port;
        private readonly string _senderName;
        private readonly string _senderEmail;
        private readonly string _user;
        private readonly string _pass;

        private MailService()
        {
            _host = AppSettings.SmtpHost;
            _port = int.Parse(AppSettings.SmtpPort);
            _senderName = AppSettings.SmtpSenderName;
            _senderEmail = AppSettings.SmtpSenderEmail;
            _user = AppSettings.SmtpUser;
            _pass = AppSettings.SmtpPass;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlBody
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_user, _pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }


        }

        // Yeni metot
        public async Task SendCourseStatusUpdateEmailAsync(string studentFullName, string studentEmail, IEnumerable<CourseStatusInfo> courseUpdates)
        {
            // 1. HTML şablonunu oku
            string templatePath = HostingEnvironment.MapPath("~/Views/course-status-update-template.html");
            string htmlTemplate = File.ReadAllText(templatePath);

            // 2. Ana yer tutucuları doldur
            htmlTemplate = htmlTemplate.Replace("[Öğrenci Adı Soyadı]", studentFullName);
            // Giriş linkini Web.config'den alabiliriz
            string loginLink = AppSettings.JwtAudience;
            htmlTemplate = htmlTemplate.Replace("[Giris_Linki]", loginLink);

            // 3. Ders listelerini durumlarına göre gruplayarak HTML oluştur
            var approvedCoursesHtml = new System.Text.StringBuilder();
            var pendingCoursesHtml = new System.Text.StringBuilder();
            var rejectedCoursesHtml = new System.Text.StringBuilder();

            foreach (var course in courseUpdates)
            {
                switch (course.Status)
                {
                    case ApprovalStatus.Onaylandı:
                        approvedCoursesHtml.Append($"<li>{course.CourseName}</li>");
                        break;
                    case ApprovalStatus.Bekliyor:
                        pendingCoursesHtml.Append($"<li>{course.CourseName}</li>");
                        break;
                    case ApprovalStatus.Reddedildi:
                        rejectedCoursesHtml.Append($"<li>{course.CourseName}</li>");
                        break;
                }
            }

            htmlTemplate = htmlTemplate.Replace("[Onaylanan_Dersler_Listesi]", approvedCoursesHtml.Length > 0 ? $"<ul>{approvedCoursesHtml}</ul>" : "<p>Onaylanan dersiniz bulunmamaktadır.</p>");
            htmlTemplate = htmlTemplate.Replace("[Bekleyen_Dersler_Listesi]", pendingCoursesHtml.Length > 0 ? $"<ul>{pendingCoursesHtml}</ul>" : "<p>Bekleyen dersiniz bulunmamaktadır.</p>");
            htmlTemplate = htmlTemplate.Replace("[Reddedilen_Dersler_Listesi]", rejectedCoursesHtml.Length > 0 ? $"<ul>{rejectedCoursesHtml}</ul>" : "<p>Reddedilen dersiniz bulunmamaktadır.</p>");

            await SendEmailAsync(studentEmail, "Ders Seçim Durumunuz Güncellendi", htmlTemplate);
        }
    }
    public class CourseStatusInfo
    {
        public string CourseName { get; set; }
        public ApprovalStatus Status { get; set; }
    }
}