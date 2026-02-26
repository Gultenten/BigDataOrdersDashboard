namespace BigDataOrdersDashboard.Dtos.LoyalthDtos
{
    public class LoyalthScoreDto
    {
        public string  CustomerName { get; set; }
        public int  TotalOrders { get; set; }
        public double  TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public double LoyalthScore { get; set; }
    }
}
