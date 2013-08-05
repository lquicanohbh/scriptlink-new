using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TestScriptLink2.Entities;

namespace TestScriptLink2
{
    public class AvatarSQLContext : DbContext
    {
        public AvatarSQLContext()
        {
            Database.SetInitializer<AvatarSQLContext>(null);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Staff>().Property(s => s.EmployeeId).HasColumnName("EmployeeID");
            modelBuilder.Entity<Staff>().Property(s => s.EmployeeName).HasColumnName("EmployeeName");
            modelBuilder.Entity<Staff>().Property(s => s.HomeRu).HasColumnName("HomeRU");
            modelBuilder.Entity<Staff>().Property(s => s.Hours).HasColumnName("Hours");
            modelBuilder.Entity<Staff>().Property(s => s.Location).HasColumnName("Location");
            modelBuilder.Entity<Staff>().Property(s => s.Title).HasColumnName("TitlePosition");
            modelBuilder.Entity<Staff>().Property(s => s.Id).HasColumnName("Id");
        }
        public DbSet<Staff> Staff { get; set; }
    }
}