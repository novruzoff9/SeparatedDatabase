using Microsoft.EntityFrameworkCore;
using SeperatedDatabase.Data;
using Microsoft.Extensions.DependencyInjection;
using SeperatedDatabase.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SeperatedDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SeperatedDatabaseContext") ?? throw new InvalidOperationException("Connection string 'SeperatedDatabaseContext' not found.")));

builder.Services.AddScoped<ISeperatedDbService, SeperatedDbService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Users/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.Run();
