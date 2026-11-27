using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace RC.Services
{
    public class ImgS
    {
        private readonly string _apiKey;
        private readonly HttpClient _client;

        public ImgS(IConfiguration config)
        {
            _apiKey = config["ImgBB:ApiKey"] ?? "";
            _client = new HttpClient();
        }

        // دالة الرفع ترجع رابط الصورة (String) أو null إذا فشلت
        public async Task<string?> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(_apiKey))
                return null;

            try
            {
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(_apiKey), "key");

                using var stream = file.OpenReadStream();
                form.Add(new StreamContent(stream), "image", file.FileName);

                var response = await _client.PostAsync("https://api.imgbb.com/1/upload", form);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    // استخراج الرابط من استجابة الموقع
                    var url = doc.RootElement.GetProperty("data").GetProperty("url").GetString();
                    return url;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"============ IMGBB ERROR ============");
                Console.WriteLine(ex.Message);
                Console.WriteLine($"=====================================");
                return null;
            }
        }

        // دالة الحذف: ImgBB لا توفر حذفاً سهلاً عبر API، لذا سنتركها فارغة
        public Task Delete(string publicId)
        {
            return Task.CompletedTask;
        }
    }
}