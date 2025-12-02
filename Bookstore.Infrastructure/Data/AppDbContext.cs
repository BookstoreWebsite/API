using Bookstore.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(320);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.HashedPassword).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15); 
                entity.Property(u => u.Type).IsRequired();
                entity.Property(u => u.RefreshToken);
                entity.Property(u => u.RefreshTokenExpiryTime);
            });
        }
    }
}
