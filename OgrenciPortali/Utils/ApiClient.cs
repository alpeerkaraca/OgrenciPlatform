using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Shared.DTO;

namespace OgrenciPortali.Utils
{
    public class ApiClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ApiClient));
        private static readonly HttpClient client;

        static ApiClient()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = false
            };
            client = new HttpClient(handler)
            {
                BaseAddress = new Uri(AppSettings.ApiBaseAddress)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
         {
            AddAuthorizationHeader(request);

            var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized &&
                !request.RequestUri.PathAndQuery.Contains("refresh-token"))
            {
                var refreshSuccess = await AttemptTokenRefreshAsync();
                if (refreshSuccess)
                {
                    var newRequest = await CloneHttpRequestMessageAsync(request);
                    AddAuthorizationHeader(newRequest);
                    response = await client.SendAsync(newRequest);
                }
            }

            return response;
        }

        private async Task<bool> AttemptTokenRefreshAsync()
        {
            var refreshTokenCookie = HttpContext.Current.Request.Cookies["RefreshToken"];
            if (refreshTokenCookie == null || string.IsNullOrEmpty(refreshTokenCookie.Value))
            {
                return false;
            }

            try
            {
                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh-token");
                refreshRequest.Headers.Add("Cookie", refreshTokenCookie.Name + "=" + refreshTokenCookie.Value);

                var response = await client.SendAsync(refreshRequest);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result =
                        JsonConvert.DeserializeObject<LoginSuccessResponse>(jsonString);

                    var newAuthCookie = new HttpCookie("AuthToken", result.AccessToken)
                    {
                        HttpOnly = true,
                        Secure = HttpContext.Current.Request.IsSecureConnection,
                        Expires = DateTime.UtcNow.AddMinutes(15),
                        Path = "/"
                    };
                    HttpContext.Current.Response.Cookies.Set(newAuthCookie);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Token yenilerken hata: ", ex);
                return false;
            }

            return false;
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            var tokenCookie = HttpContext.Current.Request.Cookies["AuthToken"];
            if (tokenCookie != null && !string.IsNullOrEmpty(tokenCookie.Value))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenCookie.Value);
            }
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);
            if (req.Content != null)
            {
                var ms = new System.IO.MemoryStream();
                await req.Content.CopyToAsync(ms);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);
                foreach (var header in req.Content.Headers)
                {
                    clone.Content.Headers.Add(header.Key, header.Value);
                }
            }

            clone.Version = req.Version;
            foreach (var prop in req.Properties)
            {
                clone.Properties.Add(prop);
            }

            foreach (var header in req.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
    }
}