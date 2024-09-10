using SeperatedDatabase.Data;
using SeperatedDatabase.Models;

namespace SeperatedDatabase.Helpers;

public interface ISeperatedDbService
{
    public string CreateDb(int id);
    public UsersDbContext GetUsersDb(int id);
}
