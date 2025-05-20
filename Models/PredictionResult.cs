namespace PlantDiseaseWebAPI.Models
{
    public class PredictionResult
    {
        public string Diagnosis { get; set; }
        public float Confidence { get; set; }
        public string Recommendation { get; set; }
        public string Causes { get; set; }                 // ➕ Kasallik sabablari
        public string Treatment { get; set; }              // ➕ Davolash yo‘llari
        public string RecoveryTime { get; set; }           // ➕ Sog‘ayish muddati
        public string Facts { get; set; }                  // ➕ Qiziqarli/faktlar

    }
}
