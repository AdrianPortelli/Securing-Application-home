using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Secure_Website.Models;

namespace Secure_Website.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TaskModel> Task { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskModel>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            //modelBuilder.Entity<Category>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

        }

     


    }
}
