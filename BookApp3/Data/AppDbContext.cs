using BookApp3.Models;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<UserBooks> User_Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
             .HasKey(a => a.Author_id);

            modelBuilder.Entity<Book>()
                .HasKey(b => b.Book_Id);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.Author_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBooks>()
             .HasKey(ub => new { ub.User_Id, ub.Book_Id });

            modelBuilder.Entity<UserBooks>()
                .Property(ub => ub.User_Id).HasColumnName("user_Id");

            modelBuilder.Entity<UserBooks>()
                .Property(ub => ub.Book_Id).HasColumnName("book_Id");

          
            modelBuilder.Entity<UserBooks>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBooks) 
                .HasForeignKey(ub => ub.User_Id)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<User>()
                .HasKey(u => u.User_Id);

            modelBuilder.Entity<Review>()
                .HasKey(r => r.Review_Id);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.Book_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Author>().ToTable("Authors");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Review>().ToTable("Reviews");
            modelBuilder.Entity<UserBooks>().ToTable("User_Books");


            base.OnModelCreating(modelBuilder);
        }

    }

}
