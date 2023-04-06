using Libra.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libra.DATA
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryBooks> CategoryBooks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryBooks>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            modelBuilder.Entity<CategoryBooks>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.CategoryBooks)
                .HasForeignKey(bc => bc.BookId);

            modelBuilder.Entity<CategoryBooks>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.CategoryBooks)
                .HasForeignKey(bc => bc.CategoryId);

            modelBuilder.Entity<Books>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Book)
                .HasForeignKey(b => b.AuthorId);
        }

    }
}
