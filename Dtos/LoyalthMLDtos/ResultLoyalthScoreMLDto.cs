namespace BigDataOrdersDashboard.Dtos.LoyalthMLDtos
{
    public class ResultLoyalthScoreMLDto
    {
        public string CustomerName { get; set; }
        public double Recency { get; set; }
        public double Frequency { get; set; }
        public double Monetary { get; set; }
        public double ActualLoyalthScore { get; set; }
        public double PredictedLoyalthScore { get; set; }

    }
}





