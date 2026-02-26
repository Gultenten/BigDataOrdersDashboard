namespace BigDataOrdersDashboard.Dtos.LoyalthMLDtos
{
    public class LoyalthMLDataScoreDto
    {
        public string CustomerName { get; set; }
        public float Recency { get; set; }
        public float Frequency { get; set; }
        public float Monetary { get; set; }
        public float LoyalthScore { get; set; }
  
    }
}
