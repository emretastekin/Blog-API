using System;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
    


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }


        public DbSet<Author>? Authors { get; set; }

        public DbSet<Comment>? Comments { get; set; }

        public DbSet<Post>? Posts { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);


            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AuthorId);


            
        }

    }
}

