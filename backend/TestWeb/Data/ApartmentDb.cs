using Microsoft.EntityFrameworkCore;
using TestWeb.Models;

namespace TestWeb.Data
{
    public class ApartmentDb : DbContext
    {
        public ApartmentDb(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Apartment> AbpApartments { get; set; }
    }
}
