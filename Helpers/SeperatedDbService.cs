using Microsoft.EntityFrameworkCore;
using SeperatedDatabase.Data;
using SeperatedDatabase.Models;
using System.Reflection.Emit;

namespace SeperatedDatabase.Helpers;

public class SeperatedDbService : ISeperatedDbService
{
    private SeperatedDatabaseContext _context;
    private IConfiguration _configuration;

    public SeperatedDbService(SeperatedDatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public string CreateDb(int id)
    {
        string defaultConnection = _configuration.GetConnectionString("SeperatedDatabaseContext");
        string serverName = defaultConnection.Substring(7, defaultConnection.IndexOf(';') - 7);

        var user = _context.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new ArgumentNullException();
        }

        string connectionString = $"Server={serverName};Database={user.UserName}Db;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using (var context = new UsersDbContext(optionsBuilder.Options))
        {
            if (context.Database.EnsureCreated())
            {
                AddSeedData(context);
            }
        }

        return connectionString;
    }

    public UsersDbContext GetUsersDb(int id)
    {
        string connectionString = _context.Users.FirstOrDefault(x => x.Id == id).ConnectionString;
        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        var context = new UsersDbContext(optionsBuilder.Options);

        return context;
    }

    private void AddSeedData(UsersDbContext context)
    {
        context.Categories
            .Add(
                new Category { Name = "Ickiler" }
            );

        context.SaveChanges();

        context.Products
            .AddRange(
                new Product { Name = "Coca Cola", Description = "0.5l plastik qabda Coca cola", CategoryId = 1 },
                new Product { Name = "Fanta", Description = "0.5l plastik qabda Fanta", CategoryId = 1 }
            );

        context.SaveChanges();
    }
}
