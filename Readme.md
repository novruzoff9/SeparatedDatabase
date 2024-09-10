# Ayrı Verilənlər bazası Yaratma Sistemi

Bu layihə hər bir istifadəçi üçün ayrılmış bir verilənlər bazası yaradır və həmin verilənlər bazasını birləşdirmək üçün fərdi bir **Connection String** əlavə edir. Verilən məlumatlar əsasında verilənlər bazası yaradıldıqdan sonra ilkin məlumat (seed data) avtomatik olaraq verilənlər bazasına əlavə olunur.

## Layihənin Funksionallığı

- İstifadəçi qeydə alınır və ona aid bir verilənlər bazası yaradılır.
- Hər istifadəçi üçün ayrıca olaraq bir **Connection String** yaradılır və həmin istifadəçinin məlumatlarına əlavə edilir.
- verilənlər bazası yaradıldıqda avtomatik olaraq müəyyən "seed data" əlavə edilir (məsələn, məhsullar və kateqoriyalar).
  
## Tələblər

- **.NET 8 Core** və ya daha yuxarı versiya.
- **Entity Framework Core**.
- **Microsoft SQL Server** (Verilənlər bazasının saxlanılması üçün).
  
## Quraşdırma və İstifadə

1. **Repository**-ni klonlayın:

    ```bash
    git clone https://github.com/novruzoff9/SeparatedDatabase.git
    ```

2. **appsettings.json** faylında `SeperatedDatabaseContext` üçün SQL Server bağlantı sətrini (`ConnectionString`) konfiqurasiya edin. Məsələn:

    ```json
    "ConnectionStrings": {
        "SeperatedDatabaseContext": "Server=localhost;Database=MasterDb;Trusted_Connection=True;"
    }
    ```

3. Layihəni işə salın:

    ```bash
    dotnet run
    ```

4. **İstifadəçi** yaratdıqda, həmin istifadəçi üçün xüsusi bir verilənlər bazası avtomatik olaraq yaradılacaqdır və həmin verilənlər bazasına aşağıdakı məlumatlar seed ediləcəkdir:

    **Kateqoriya**:
    - İçkilər

    **Məhsullar**:
    - Coca Cola (0.5L plastik qabda)
    - Fanta (0.5L plastik qabda)

## Kod Təsviri

Layihənin əsas funksiyası **CreateDb** metodudur. Bu metod müəyyən bir istifadəçinin məlumatlarına əsasən ona aid xüsusi bir verilənlər bazası yaradır. Verilənlər bazası `EnsureCreated` metodu ilə yaradıldıqda, `AddSeedData` metodu vasitəsilə həmin istifadəçinin verilənlər bazasına ilkin məlumatlar (seed data) əlavə olunur.

```csharp
public string CreateDb(int id)
{
    string connectionString = $"Server=...;Database={user.UserName}Db;...";

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
```

İstifadəçi yaradıldıqdan sonra həmin istifadəçinin ```ConnectionString``` məlumatı da veritabanına əlavə edilir.

```csharp
public async Task<IActionResult> Create([Bind("Id,UserName,ConnectionString")] User user)
{
    if (ModelState.IsValid)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
        var lastUser = _context.Users.ToList().Last();
        lastUser.ConnectionString = _createSeperatedDb.CreateDb(lastUser.Id);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(user);
}

```

Daha sonra isə istifadəçinin məlumatlarını əldə etmək üçün əsas verilənlər bazasından onun ```connectionString```i götürülürərək özünə aid olan verilənlər bazasına giriş edilir və məlumatları əldə olunur.

```csharp
public UsersDbContext GetUsersDb(int id)
{
    string connectionString = _context.Users.FirstOrDefault(x => x.Id == id).ConnectionString;
    var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
    optionsBuilder.UseSqlServer(connectionString);

    var context = new UsersDbContext(optionsBuilder.Options);

    return context;
}
```

Gətirilən məlumatlar birbaşa controller-da istifadə olunur.

```csharp
public async Task<IActionResult> UserDbDetails(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var database = _createSeperatedDb.GetUsersDb((int)id);
    if (database == null)
    {
        return NotFound();
    }

    return View(database.Products.Include(x => x.Category).ToList());
}
```