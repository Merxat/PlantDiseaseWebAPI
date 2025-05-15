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

        private static string lastDiagnosis = null;
        private static string lastFilePath = null;

        public UploadController()
        {
            _predictionService = new ImagePredictionService(ModelPath);
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Rasm tanlanmadi.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, image.FileName);

            if (lastFilePath != null && lastFilePath == filePath)
            {
                return Ok(new { message = "Tashxis allaqachon qo‘yilgan.", diagnosis = lastDiagnosis, filePath });
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            var result = _predictionService.Predict(filePath);

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
