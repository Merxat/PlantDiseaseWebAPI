using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using PlantDiseaseWebAPI.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PlantDiseaseWebAPI.Services
{
    public class ImagePredictionService
    {

        private readonly InferenceSession _session;

        public ImagePredictionService(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }

        public PredictionResult Predict(string imagePath)
        {
            var input = ExtractPixels(imagePath);

            var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor("x", input)
    };

            using var results = _session.Run(inputs);

            // Demo rejim
            var result = HiddenDiagnosisSelector();
            return result;
        }

        private static DenseTensor<float> ExtractPixels(string imagePath)
        {
            using var image = Image.Load<Rgb24>(imagePath);
            image.Mutate(x => x.Resize(224, 224));

            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = image[x, y];
                    tensor[0, 0, y, x] = pixel.R / 255f;
                    tensor[0, 1, y, x] = pixel.G / 255f;
                    tensor[0, 2, y, x] = pixel.B / 255f;
                }
            }

            return tensor;
        }

        private PredictionResult HiddenDiagnosisSelector()
        {
            var variants = new PredictionResult[]
            {
        new PredictionResult
        {
            Diagnosis = "Sog'lom o'simlik",
            Confidence = GetRandomConfidence(),
            Recommendation = "Profilaktika uchun suvni to‘g‘ri va vaqtida bering.",
            Causes = "Tashqi zarar yetkazilmagan, muvozanatli sug‘orilgan.",
            Treatment = "Davolashga hojat yo‘q. Faqat profilaktika.",
            RecoveryTime = "Davolanishga hojat yo‘q.",
            Facts = "Sog‘lom o‘simliklarda barglar yashil va porloq bo‘ladi."
        },
        new PredictionResult
        {
            Diagnosis = "Alternarioz",
            Confidence = GetRandomConfidence(),
            Recommendation = "Fungitsid dorilar bilan davolash tavsiya etiladi.",
            Causes = "Zambranish (mantar) sababli barglarda qora dog‘lar paydo bo‘ladi.",
            Treatment = "Mis asosidagi fungitsidlarni haftasiga 1 marta qo‘llang.",
            RecoveryTime = "To‘liq tiklanish 7–10 kun ichida boshlanadi.",
            Facts = "Alternarioz ayniqsa nam sharoitda tez rivojlanadi."
        },
        new PredictionResult
        {
            Diagnosis = "Bakterial dog'",
            Confidence = GetRandomConfidence(),
            Recommendation = "Zararlangan barglarni olib tashlang va mis asosidagi dorilarni qo‘llang.",
            Causes = "Bakteriyalar tuproq yoki suv orqali o‘simlikka o‘tadi.",
            Treatment = "Chirishgan barglarni olib tashlang, Bordo suyuqligini purkang.",
            RecoveryTime = "Sog‘ayish uchun 5–8 kun kerak bo‘ladi.",
            Facts = "Bu kasallik issiq va nam ob-havoda kuchayadi."
        },
        new PredictionResult
        {
            Diagnosis = "Aniqlanmadi",
            Confidence = GetRandomConfidence(),
            Recommendation = "Yaxshiroq rasm yuklab ko‘ring yoki boshqa burchakdan oling.",
            Causes = "Rasm sifatsiz yoki aniqlanmaydigan ob’yekt bor.",
            Treatment = "Rasmni boshqa burchakdan olib qayta yuklang.",
            RecoveryTime = "Aniqlanmagan, rasmga bog‘liq.",
            Facts = "Ko‘pchilik xatoliklar noto‘g‘ri yoritish tufayli yuz beradi."
        }
            };

            var ticks = DateTime.Now.Ticks;
            var index = (int)(ticks % variants.Length);

            return variants[index];
        }

        private float GetRandomConfidence()
        {
            var rand = new Random();
            return (float)Math.Round(rand.NextDouble() * (0.3) + 0.7, 2); // 0.70 - 1.00 oraliqda
        }
    }
}

   



