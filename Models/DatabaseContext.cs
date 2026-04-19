using Microsoft.EntityFrameworkCore;

namespace RUG.WebEng.Transactions.Models;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
}