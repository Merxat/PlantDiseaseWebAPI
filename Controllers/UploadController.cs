using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlantDiseaseWebAPI.Services;
using System.IO;
using System.Threading.Tasks;

namespace PlantDiseaseWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ImagePredictionService _predictionService;

        private static readonly string ModelPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "mobilenetv2_100_OpSet17.onnx");

        // Rasm yuklanganidan keyin tashxisni saqlash
        private static string lastDiagnosis = null;
        private static string lastFilePath = null;

        public UploadController()
        {
            _predictionService = new ImagePredictionService(ModelPath);
        }

        // Rasmni yuklash va tashxisni qo'yish
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Rasm tanlanmadi.");

            // Yangi rasm yuklanishini tekshirish
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, image.FileName);

            // Agar tashxis allaqachon berilgan bo'lsa, eski tashxisni qaytarish
            if (lastFilePath != null && lastFilePath == filePath)
            {
                return Ok(new { message = "Tashxis allaqachon qo‘yilgan.", diagnosis = lastDiagnosis, filePath });
            }

            // Rasmni saqlash
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Yangi rasmni prognozlash (tashxis berish)
            var result = _predictionService.Predict(filePath);

            // Yangi tashxisni faqat yangi rasmga qo'yish
            lastDiagnosis = result;
            lastFilePath = filePath;

            return Ok(new
            {
                message = "Rasm yuklandi va yangi tashxis qo‘yildi.",
                diagnosis = result,
                filePath = filePath
            });
        }

        // So'nggi tashxisni olish
        [HttpGet("last-diagnosis")]
        public IActionResult GetLastDiagnosis()
        {
            if (lastDiagnosis == null)
            {
                return Ok(new { message = "Hali hech qanday tashxis berilmagan." });
            }
            return Ok(new
            {
                message = "So'nggi tashxis",
                diagnosis = lastDiagnosis,
                filePath = lastFilePath
            });
        }
    }
}
