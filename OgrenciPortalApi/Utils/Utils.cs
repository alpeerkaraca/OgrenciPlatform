using Hangfire;
using Newtonsoft.Json;
using OgrenciPortalApi.Models;
using Shared.DTO;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OgrenciPortalApi.Utils
{
    public class Utils
    {
        private readonly OgrenciPortalApiDB db = new OgrenciPortalApiDB();
        public static Guid? GetActiveUserId()
        {
            try
            {
                var user = HttpContext.Current.User;
                if (user == null || !user.Identity.IsAuthenticated) return null;
                var userIdClaim = user.Identity.Name;
                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void SetAuditFieldsForCreate(BaseClass entitiy)
        {
            entitiy.CreatedAt = DateTime.Now;
            entitiy.UpdatedAt = DateTime.Now;
            entitiy.CreatedBy = GetActiveUserId().ToString();
            entitiy.UpdatedBy = GetActiveUserId().ToString();
            entitiy.IsDeleted = false;
        }

        public static void SetAuditFieldsForUpdate(BaseClass entitiy)
        {
            entitiy.UpdatedAt = DateTime.Now;
            entitiy.UpdatedBy = GetActiveUserId().ToString();
        }

        public static void SetAuditFieldsForDelete(BaseClass entitiy)
        {
            entitiy.DeletedAt = DateTime.Now;
            entitiy.DeletedBy = GetActiveUserId().ToString();
            entitiy.IsDeleted = true;
        }

        public static async Task<string> FillWithAi(string input)
        {
            var message = new Message
            {
                role = "system",
                content = $"{input} dersi için en fazla 500 harften oluşan akademik bir müfredat açıklaması oluştur. 500 karakteri aşmaması gerekli yanıtı göndermeden önce karakter uzunluğuna dikkat et. Sadece metni ver. Herhangi bir tablo içeriği vb. oluşturmaya çalışma."
            };

            var content = new AiRequestDto()
            {
                messages = new List<Message> { message },
                model = "deepseek-chat",
                stream = false
            };

             var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AppSettings.DeepseekApiKey);

            var jsonContent = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://api.deepseek.com/chat/completions", stringContent);

                // Hata durumunda response içeriğini oku
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"HTTP {response.StatusCode}: {errorContent}");
                    return string.Empty;
                }

                var responseData = await response.Content.ReadAsAsync<AiResponseDto>();
                return responseData.choices[0].message.content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI API Hatası: {ex.Message}");
                return string.Empty;
            }
        }

        [AutomaticRetry(Attempts = 3)] // Hata olursa 3 kez daha denemesini sağlar (önerilir)
        public static async Task FillWithAi(string input, Guid CourseId)
        {
            var message = new Message
            {
                role = "system",
                content = $"{input} dersi için en fazla 500 harften oluşan akademik bir müfredat açıklaması oluştur. 500 karakteri aşmaması gerekli yanıtı göndermeden önce karakter uzunluğuna dikkat et. Sadece metni ver. Herhangi bir tablo içeriği vb. oluşturmaya çalışma."
            };

            var content = new AiRequestDto()
            {
                messages = new List<Message> { message },
                model = "deepseek-chat",
                stream = false
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AppSettings.DeepseekApiKey);

            var jsonContent = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://api.deepseek.com/chat/completions", stringContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"HTTP {response.StatusCode}: {errorContent}");
                    throw new Exception($"AI API returned non-success status code: {response.StatusCode}");
                }

                var responseData = await response.Content.ReadAsAsync<AiResponseDto>();

                // DbContext'i 'using' bloğu içine alıyoruz.
                using (var db = new OgrenciPortalApiDB())
                {
                    var course = await db.Courses.FindAsync(CourseId);
                    if (course != null)
                    {
                        course.Description = responseData.choices[0].message.content;
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI API Hatası: {ex.Message}");
                // Hatayı tekrar fırlat ki Hangfire görevin başarısız olduğunu anlasın.
                throw;
            }
        }
    }
}
