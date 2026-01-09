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
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PriceListEntry> PriceListEntries { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Report> Reports { get; set; }

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

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Products");

                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).IsRequired().HasMaxLength(255);
                entity.Property(p => p.ImageUrl).IsRequired();

                entity.HasDiscriminator<string>("ProductType")
                      .HasValue<Book>("Book");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Rating);
            });

            modelBuilder.Entity<PriceListEntry>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Price)
                      .HasColumnType("numeric(18,2)")
                      .IsRequired();

                entity.Property(e => e.ValidFrom)
                      .HasColumnType("timestamp with time zone")
                      .IsRequired();

                entity.Property(e => e.ValidTo)
                      .HasColumnType("timestamp with time zone");

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.PriceListEntries)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ProductId, e.ValidTo });
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "GenreBooks",
                    j => j
                        .HasOne<Genre>()
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("BookId", "GenreId");
                        j.ToTable("GenreBooks");
                    }
                );

            modelBuilder.Entity<User>()
                .HasMany(u => u.Wished)
                .WithMany(b => b.WishedBy)
                .UsingEntity<Dictionary<string, object>>(
                    "UserWishedBooks",
                    j => j
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("UserId", "BookId");
                        j.ToTable("UserWishedBooks");
                    }
                );

            modelBuilder.Entity<User>()
                .HasMany(u => u.Read)
                .WithMany(b => b.ReadBy)
                .UsingEntity<Dictionary<string, object>>(
                    "UserReadBooks",
                    j => j
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("UserId", "BookId");
                        j.ToTable("UserReadBooks");
                    }
                );


            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(sc => sc.UserId);

                entity.HasOne(sc => sc.User)
                      .WithOne(u => u.ShoppingCart)
                      .HasForeignKey<ShoppingCart>(sc => sc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                entity.HasOne(ci => ci.ShoppingCart)
                      .WithMany(sc => sc.Items)
                      .HasForeignKey(ci => ci.ShoppingCartUserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                      .WithMany()
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ci => new { ci.ShoppingCartUserId, ci.ProductId }).IsUnique();
                entity.Property(ci => ci.Quantity).IsRequired();
                entity.Property(ci => ci.UnitPrice).IsRequired();
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.DateTime)
                      .IsRequired()
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("now()");

                entity.Property(p => p.TotalPrice)
                      .HasColumnType("numeric(18,2)")
                      .IsRequired();

                entity.HasOne(p => p.Reader)
                      .WithMany(u => u.Purchases)
                      .HasForeignKey(p => p.ReaderId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.ReaderId);

                entity.OwnsOne(p => p.Address, address =>
                {
                    address.Property(a => a.Country).HasMaxLength(100).IsRequired();
                    address.Property(a => a.City).HasMaxLength(100).IsRequired();
                    address.Property(a => a.Street).HasMaxLength(200).IsRequired();
                    address.Property(a => a.Number).HasMaxLength(20).IsRequired();
                    address.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
                });

                entity.Navigation(p => p.Address).IsRequired();

                entity.HasMany(p => p.Items)
                      .WithOne(i => i.Purchase)
                      .HasForeignKey(i => i.PurchaseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Quantity)
                      .IsRequired();

                entity.Property(i => i.UnitPrice)
                      .HasColumnType("numeric(18,2)")
                      .IsRequired();

                entity.HasOne(i => i.Product)
                      .WithMany()
                      .HasForeignKey(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(i => new { i.PurchaseId, i.ProductId }).IsUnique();
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(r => r.Text)
                      .IsRequired()
                      .HasMaxLength(2000); 

                entity.Property(r => r.Rating)
                      .IsRequired();

                entity.HasOne(r => r.Book)
                      .WithMany(b => b.Reviews)
                      .HasForeignKey(r => r.BookId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Reader)
                      .WithMany(r => r.Reviews)
                      .HasForeignKey(r => r.ReaderId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.BookId);

                entity.HasCheckConstraint("CK_Reviews_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");

            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Text)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.Property(c => c.HasReplies)
                      .IsRequired();

                entity.HasOne(c => c.Reader)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.ReaderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Review)
                      .WithMany(r => r.Comments)
                      .HasForeignKey(c => c.ReviewId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Book)
                      .WithMany(b => b.Comments)
                      .HasForeignKey(c => c.BookId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.Replies)
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.ReaderId);
                entity.HasIndex(c => c.ReviewId);
                entity.HasIndex(c => c.BookId);
                entity.HasIndex(c => c.ParentCommentId);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports", t =>
                {
                    t.HasCheckConstraint(
                        "CK_Reports_Target_Xor",
                        "num_nonnulls(\"ReviewId\", \"CommentId\") = 1"
                    );
                });

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Text)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.Property(r => r.Reason)
                      .IsRequired();

                entity.HasOne(r => r.Reader)
                      .WithMany(u => u.Reports)
                      .HasForeignKey(r => r.ReaderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Review)
                      .WithMany(rv => rv.Reports)
                      .HasForeignKey(r => r.ReviewId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Comment)
                      .WithMany(c => c.Reports)
                      .HasForeignKey(r => r.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.ReaderId);
                entity.HasIndex(r => r.ReviewId);
                entity.HasIndex(r => r.CommentId);
            });


        }
    }
}
