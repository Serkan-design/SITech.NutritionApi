using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
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
                return BadRequest("Fotoğraf yok");

            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                string base64Image = Convert.ToBase64String(ms.ToArray());

                string apiKey = _configuration["OpenAI:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                    return StatusCode(500, "API KEY YOK");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "gpt-4.1-mini",
                    input = new object[]
                    {
                        new {
                            role = "user",
                            content = new object[]
                            {
                                new {
                                    type = "input_text",
                                    text = "Bu görseldeki yemeği analiz et ve SADECE JSON dön. Kod bloğu kullanma. Format: { \"yemek_adi\": \"string\", \"kalori\": number, \"protein\": number, \"karbonhidrat\": number, \"yag\": number }"
                                },
                                new {
                                    type = "input_image",
                                    image_url = $"data:{file.ContentType};base64,{base64Image}"
                                }
                            }
                        }
                    }
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/responses", content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("RAW AI: " + result);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, result);

                dynamic data = JsonConvert.DeserializeObject(result);

                string modelText = "";

                // 🔥 HER FORMATI YAKALA
                try
                {
                    modelText = data.output[0].content[0].text.ToString();
                }
                catch
                {
                    try
                    {
                        modelText = data.output_text.ToString();
                    }
                    catch
                    {
                        return Ok(new
                        {
                            yemek_adi = "Algılanamadı",
                            kalori = 0,
                            protein = 0,
                            karbonhidrat = 0,
                            yag = 0
                        });
                    }
                }

                // 🔥 JSON TEMİZLE
                modelText = modelText
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                NutritionResult finalResult;

                try
                {
                    finalResult = JsonConvert.DeserializeObject<NutritionResult>(modelText);
                }
                catch
                {
                    return Ok(new
                    {
                        yemek_adi = "Algılanamadı",
                        kalori = 0,
                        protein = 0,
                        karbonhidrat = 0,
                        yag = 0
                    });
                }

                if (finalResult == null)
                {
                    return Ok(new
                    {
                        yemek_adi = "Algılanamadı",
                        kalori = 0,
                        protein = 0,
                        karbonhidrat = 0,
                        yag = 0
                    });
                }

                return Ok(finalResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);

                return Ok(new
                {
                    yemek_adi = "Hata oluştu",
                    kalori = 0,
                    protein = 0,
                    karbonhidrat = 0,
                    yag = 0
                });
            }
        }
    }
}