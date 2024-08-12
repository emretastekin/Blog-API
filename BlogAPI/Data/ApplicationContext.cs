using System;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {



        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Author>? Authors { get; set; }

        public DbSet<Comment>? Comments { get; set; }

        public DbSet<Post>? Posts { get; set; }

        public DbSet<Favorite>? Favorites { get; set; }

        public DbSet<LikeDislike>? LikeDislikes { get; set; }

        public DbSet<Category>? Categories { get; set; }

        public DbSet<SubCategory>? SubCategories { get; set; }

        public DbSet<ApplicationUser>? ApplicationUsers { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ApplicationUser>()
               .HasIndex(u => u.IdNumber)
               .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();




            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasPrincipalKey(a => a.Id);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CommentOfComment)
                .WithMany(c => c.CommentAnswers)
                .HasForeignKey(c => c.CommentOfCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.SubCategory)
                .WithMany(sc => sc.Posts)
                .HasForeignKey(p => p.SubCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
               .HasOne(f => f.Post)
               .WithMany(p => p.Favorites)
               .HasForeignKey(f => f.PostId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Author)
                .WithMany(a => a.Favorites)
                .HasForeignKey(f => f.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikeDislike>()
                .HasOne(ld => ld.Post)
                .WithMany(p => p.LikeDislikes)
                .HasForeignKey(ld => ld.PostId)
                .OnDelete(DeleteBehavior.Restrict); // veya NoAction

            modelBuilder.Entity<LikeDislike>()
                .HasOne(ld => ld.Author)
                .WithMany(a => a.LikeDislikes)
                .HasForeignKey(ld => ld.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // veya NoAction

            modelBuilder.Entity<LikeDislike>()
                .HasOne(ld => ld.Comment)
                .WithMany(c => c.LikeDislikes)
                .HasForeignKey(ld => ld.CommentId)
                .OnDelete(DeleteBehavior.Restrict); // veya NoAction










        }

    }
}
