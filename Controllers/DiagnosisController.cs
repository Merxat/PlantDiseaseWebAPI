using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlantCareAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosisController : ControllerBase
    {
      
        private static bool isDiagnosisGiven = false;
        private static string savedDiagnosis = null;
        private static string savedFilePath = null;

        [HttpPost("upload/image")]
        public async Task<IActionResult> Diagnose([FromForm] IFormFile image)
        {
            if (isDiagnosisGiven)
            {
                return Ok(new
                {
                    message = "Tashxis allaqachon berilgan.",
                    diagnosis = savedDiagnosis,
                    filePath = savedFilePath
                });
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest("Iltimos, rasm tanlang.");
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Guid.NewGuid().ToString() + ".png");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var diagnosis = await GetDiagnosisFromModel(filePath);

            isDiagnosisGiven = true;
            savedDiagnosis = diagnosis;
            savedFilePath = filePath;

            return Ok(new
            {
                message = "Rasm yuklandi va tashxis qo‘yildi.",
                diagnosis = diagnosis,
                filePath = filePath
            });
        }

        private async Task<string> GetDiagnosisFromModel(string filePath)
        {
            return "Tashxis natijasi";
        }
    }
}
