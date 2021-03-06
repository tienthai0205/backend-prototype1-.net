using Microsoft.EntityFrameworkCore;

namespace ReservationApi.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Reservation>()
                .HasIndex(re => re.Date)
                .IsUnique();
        }
    }
}