using Microsoft.EntityFrameworkCore;
using RUG.WebEng.Transactions.Models;

namespace RUG.WebEng.Transactions.Repositories;

public class TransactionRepository(DatabaseContext context) : IRepository<Transaction, long>
{
    protected DatabaseContext _context = context;

    public IQueryable<Transaction> SimpleCollection => _context.Transactions;

    public IQueryable<Transaction> FullCollection => _context.Transactions;

    public async Task<bool> CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Transaction transaction)
    {
        _context.Update(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Transaction?> FindAsync(long id) => await FullCollection.FirstOrDefaultAsync(t => t.Id == id);
}