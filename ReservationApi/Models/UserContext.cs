using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReservationApi.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            LoadDefaultUsers();
        }
        public DbSet<User> Users { get; set; }
        public List<User> getUsers () => Users.Local.ToList<User>();

        private void LoadDefaultUsers()
        {
            Users.Add(new User { Id = 1, Email = "tien@saxion.nl", Password = "Tien12345"});
            Users.Add(new User { Id = 2, Email = "max@saxion.nl", Password = "Max12345"});
        }
    }
}