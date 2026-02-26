using Microsoft.ML.Data;

namespace BigDataOrdersDashboard.Dtos.LoyalthMLDtos
{
    public class LoyalthScoreMLPredictionDto
    {
        [ColumnName("Score")]
        public float LoyalthScore { get; set; }
    }
}
