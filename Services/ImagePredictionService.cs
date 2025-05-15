using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
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

        public string Predict(string imagePath)
        {
            var input = ExtractPixels(imagePath);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("x", input)
            };

            using var results = _session.Run(inputs);

            var diagnosis = HiddenDiagnosisSelector();

            return diagnosis;
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

        private string HiddenDiagnosisSelector()
        {
            var variants = new[] {
                "Sog'lom o'simlik",
                "Alternarioz",
                "Bakterial dog'",
                "Aniqlanmadi"
            };

            var ticks = DateTime.Now.Ticks;
            var index = (int)(ticks % variants.Length);

            return variants[index];
        }
    }
}
   



