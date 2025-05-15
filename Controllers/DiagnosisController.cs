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
        // Tashxisni saqlash uchun flag
        private static bool isDiagnosisGiven = false;
        private static string savedDiagnosis = null;
        private static string savedFilePath = null;

        // Rasm yuklash va tashxis qo'yish
        [HttpPost("upload/image")]
        public async Task<IActionResult> Diagnose([FromForm] IFormFile image)
        {
            // Tashxis allaqachon berilgan bo'lsa, avvalgi tashxisini qaytarish
            if (isDiagnosisGiven)
            {
                return Ok(new
                {
                    message = "Tashxis allaqachon berilgan.",
                    diagnosis = savedDiagnosis,
                    filePath = savedFilePath
                });
            }

            // Agar rasm tanlanmagan bo'lsa
            if (image == null || image.Length == 0)
            {
                return BadRequest("Iltimos, rasm tanlang.");
            }

            // Rasmni serverga saqlash
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", Guid.NewGuid().ToString() + ".png");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Modelni ishlatib tashxisni olish
            var diagnosis = await GetDiagnosisFromModel(filePath);

            // Tashxisni faqat bitta marta qilishni ta'minlash
            isDiagnosisGiven = true;
            savedDiagnosis = diagnosis;
            savedFilePath = filePath;

            // Tashxis natijasini qaytarish
            return Ok(new
            {
                message = "Rasm yuklandi va tashxis qo‘yildi.",
                diagnosis = diagnosis,
                filePath = filePath
            });
        }

        // Modelni ishlatib tashxis olish
        private async Task<string> GetDiagnosisFromModel(string filePath)
        {
            // Modelni yuklab tashxisni olish
            // Bu yerda ONNX modelni chaqirish kodini joylash
            return "Tashxis natijasi";
        }
    }
}
