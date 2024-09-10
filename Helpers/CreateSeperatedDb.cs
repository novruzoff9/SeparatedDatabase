using Microsoft.EntityFrameworkCore;
using SeperatedDatabase.Data;
using SeperatedDatabase.Models;
using System.Reflection.Emit;

namespace SeperatedDatabase.Helpers;

public class CreateSeperatedDb : ICreateSeperatedDb
{
    private SeperatedDatabaseContext _context;
    private IConfiguration _configuration;

    public CreateSeperatedDb(SeperatedDatabaseContext context, IConfiguration configuration)
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
                // Eğer veritabanı oluşturulmuşsa, seed verilerini ekleyin
                AddSeedData(context);
            }
        }

        return connectionString;
    }

    private void AddSeedData(UsersDbContext context)
    {
        context.Categories
            .Add(
                new Category { Name = "Ickiler" }
            );

        context.Products
            .AddRange(
                new Product { Name = "Coca Cola", Description = "0.5l plastik qabda Coca cola" },
                new Product { Name = "Fanta", Description = "0.5l plastik qabda Fanta" }
            );

        context.SaveChanges();
    }
}
