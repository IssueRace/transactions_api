using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RUG.WebEng.Transactions.Models;
using RUG.WebEng.Transactions.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RUG.WebEng.Transactions.Controllers;

[ApiController]
[Route("")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionRepository _repo;

    public TransactionsController(TransactionRepository repo)
    {
        _repo = repo;
    }

    // CRUD 
    // POST /transactions
    [HttpPost("transactions")]
    public async Task<ActionResult<Transaction>> CreateTransaction(
        [FromBody, Required] Transaction transaction)
    {
        await _repo.CreateAsync(transaction);
        return CreatedAtAction(nameof(GetTransactionById),
            new { id = transaction.Id }, transaction);
    }

    // GET /transactions/{id}
    [HttpGet("transactions/{id:long}")]
    public async Task<ActionResult<Transaction>> GetTransactionById(long id)
    {
        var transaction = await _repo.FindAsync(id);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    // PUT /transactions/{id}
    [HttpPut("transactions/{id:long}")]
    public async Task<ActionResult<Transaction>> UpdateTransaction(
        long id,
        [FromBody, Required] Transaction updated)
    {
        var existing = await _repo.FindAsync(id);
        if (existing is null)
            return NotFound();

        existing.Date = updated.Date;
        existing.ClientId = updated.ClientId;
        existing.Amount = updated.Amount;
        existing.MerchantCity = updated.MerchantCity;
        existing.MerchantState = updated.MerchantState;

        await _repo.UpdateAsync(existing);
        return Ok(existing);
    }

    // DELETE /transactions/{id}
    [HttpDelete("transactions/{id:long}")]
    public async Task<IActionResult> DeleteTransaction(long id)
    {
        var transaction = await _repo.FindAsync(id);
        if (transaction is null)
            return NotFound();

        await _repo.DeleteAsync(transaction);
        return NoContent();
    }

    // GET /client_transactions/{clientId}
    [HttpGet("client_transactions/{clientId:int}")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetClientTransactions(
        int clientId,
        int? month = null,
        int? year = null,
        int limit = 10,
        int offset = 0)
    {
        var query = _repo.SimpleCollection.Where(t => t.ClientId == clientId);

        if (month.HasValue)
            query = query.Where(t => t.Date.Month == month.Value);

        if (year.HasValue)
            query = query.Where(t => t.Date.Year == year.Value);

        var result = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return result.Any() ? Ok(result) : NoContent();
    }

    // DELETE /client_transactions/{clientId}
    [HttpDelete("client_transactions/{clientId:int}")]
    public async Task<IActionResult> DeleteClientTransactions(
        int clientId,
        int? month = null,
        int? year = null)
    {
        var query = _repo.SimpleCollection.Where(t => t.ClientId == clientId);

        if (month.HasValue)
            query = query.Where(t => t.Date.Month == month.Value);

        if (year.HasValue)
            query = query.Where(t => t.Date.Year == year.Value);

        var transactions = await query.ToListAsync();

        foreach (var t in transactions)
            await _repo.DeleteAsync(t);

        return NoContent();
    }
    
    // GET /average_transaction_amount
    [HttpGet("average_transaction_amount")]
    public async Task<ActionResult<object>> GetAverageTransactionAmount(
        [FromQuery, Required] string city,
        [FromQuery, Required] string state,
        int? month = null,
        int? year = null)
    {
        var query = _repo.SimpleCollection
            .Where(t => t.MerchantCity == city && t.MerchantState == state);

        if (month.HasValue)
            query = query.Where(t => t.Date.Month == month.Value);

        if (year.HasValue)
            query = query.Where(t => t.Date.Year == year.Value);

        if (!await query.AnyAsync())
            return NoContent();

        return Ok(new
        {
            average_amount = await query.AverageAsync(t => t.Amount),
            city,
            state,
            month,
            year
        });
    }

    // GET /total_transaction_amount_by_state_day
    [HttpGet("total_transaction_amount_by_state_day")]
    public async Task<ActionResult<IEnumerable<object>>> GetTotalAmountByStateAndDay(
        [FromQuery, Required] string state,
        [FromQuery, Required] int month,
        int batchSize = 10)
    {
        if (!new[] { 10, 20, 50, 100 }.Contains(batchSize))
            return BadRequest("batchSize must be one of: 10, 20, 50, 100");

        var result = await _repo.SimpleCollection
            .Where(t => t.MerchantState == state && t.Date.Month == month)
            .GroupBy(t => t.Date.Day)
            .Select(g => new
            {
                day = g.Key,
                total_amount = g.Sum(t => t.Amount),
                transaction_count = g.Count()
            })
            .OrderBy(r => r.day)
            .Take(batchSize)
            .ToListAsync();

        return result.Any() ? Ok(result) : NoContent();
    }

    // GET /extreme_total_transaction_amount
    [HttpGet("extreme_total_transaction_amount")]
    public async Task<ActionResult<object>> GetExtremeTotalTransactionAmount(
        [FromQuery, Required] int fromYear,
        [FromQuery, Required] int toYear,
        [FromQuery, Required, RegularExpression("min|max")]
        string extremeType)
    {
        var query = _repo.SimpleCollection
            .Where(t => t.Date.Year >= fromYear && t.Date.Year <= toYear);

        if (!await query.AnyAsync())
            return NoContent();

        var transaction = extremeType == "min"
            ? await query.OrderBy(t => t.Amount).FirstAsync()
            : await query.OrderByDescending(t => t.Amount).FirstAsync();

        return Ok(new
        {
            amount = transaction.Amount,
            start_year = fromYear,
            end_year = toYear,
            timestamp = transaction.Date,
            location_of_purchase = transaction.MerchantCity ?? "online"
        });
    }
}