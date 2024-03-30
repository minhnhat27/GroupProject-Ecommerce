using Azure;
using GroupProject_Ecommerce.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace GroupProject_Ecommerce.Helpers
{
    public static class ReCaptcha
    {
        public class CaptchaResponse
        {
            public bool success { get; set; }
        }
        public static async Task<bool> Validate(string code)
        {
            
            string serectKey = "6LdYW6kpAAAAALMOcSIcWeKNrhcOhVBNW3_93_HZ";
            using(var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",serectKey, code));

                var ggResponse = await client.SendAsync(request);
                var content = await ggResponse.Content.ReadAsStringAsync();
                CaptchaResponse response = JsonConvert.DeserializeObject<CaptchaResponse>(content);
                return response.success;
            }
        }
    }
}
