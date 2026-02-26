namespace BigDataOrdersDashboard.Models
{
    public class OrderSummaryViewClass
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
       
    }
}

