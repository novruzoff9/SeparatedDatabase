using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeperatedDatabase.Models;

namespace SeperatedDatabase.Data;

public class SeperatedDatabaseContext : DbContext
{
    public SeperatedDatabaseContext(DbContextOptions<SeperatedDatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(x => x.Id);
    }

}
