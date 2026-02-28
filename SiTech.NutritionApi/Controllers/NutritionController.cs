using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using SITech.NutritionApi.Models;

namespace SITech.NutritionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public NutritionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("analiz")]
        public async Task<IActionResult> AnalyzeImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Lütfen bir fotoğraf seçin.");

            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                string base64Image = Convert.ToBase64String(ms.ToArray());

                string apiKey = _configuration["OpenAI:ApiKey"];

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new object[]
                    {
                        new {
                            role = "user",
                            content = new object[]
                            {
                                new {
                                    type = "text",
                                    text = "Bu görseldeki yemeği analiz et ve sadece şu JSON formatında cevap ver: { \"yemek_adi\": \"string\", \"kalori\": number, \"protein\": number, \"karbonhidrat\": number, \"yag\": number }"
                                },
                                new {
                                    type = "image_url",
                                    image_url = new {
                                        url = $"data:{file.ContentType};base64,{base64Image}"
                                    }
                                }
                            }
                        }
                    },
                    max_tokens = 500
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, result);

                dynamic data = JsonConvert.DeserializeObject(result);
                string modelText = data.choices[0].message.content.ToString();

                var finalResult = JsonConvert.DeserializeObject<NutritionResult>(modelText);

                return Ok(finalResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }
    }
}