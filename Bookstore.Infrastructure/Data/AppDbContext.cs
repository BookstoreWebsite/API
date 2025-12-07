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
        public DbSet<Book> Books { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.HashedPassword).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15); 
                entity.Property(u => u.Type).IsRequired();
                entity.Property(u => u.ProfilePictureUrl);
                entity.Property(u => u.ReaderBio);
                entity.Property(u => u.RefreshToken);
                entity.Property(u => u.RefreshTokenExpiryTime);
            });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Following)
                .WithMany(u => u.Followers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserFollowers",
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("FollowerId", "FollowingId");
                        j.ToTable("UserFollowers");
                    }
                );

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Description).IsRequired().HasMaxLength(255);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.ImageUrl).IsRequired();
                entity.Property(b => b.Rating);
            });
        }
    }
}
