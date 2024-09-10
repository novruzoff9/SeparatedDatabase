namespace SeperatedDatabase.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? ConnectionString { get; set; }
}
