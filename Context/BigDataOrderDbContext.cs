using BigDataOrdersDashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Context
{
    public class BigDataOrderDbContext:DbContext
    {
        public BigDataOrderDbContext 
            (DbContextOptions<BigDataOrderDbContext> options):base(options)
        {

        }
        public DbSet<Category> Categories {  get; set; }
        public DbSet<Product> Products {  get; set; }
        public DbSet<Customer> Customers {  get; set; }
        public DbSet<Order> Orders {  get; set; }
        public DbSet<Review> Reviews {  get; set; }
        public DbSet<Message> Messages {  get; set; }
        
        

    }
}
