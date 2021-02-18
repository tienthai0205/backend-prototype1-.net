using Microsoft.EntityFrameworkCore;

namespace ReservationApi.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<UserContext> Users { get; set; }
    }
}