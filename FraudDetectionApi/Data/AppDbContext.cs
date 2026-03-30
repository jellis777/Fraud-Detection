
using FraudDetectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}