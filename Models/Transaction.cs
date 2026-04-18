using System.ComponentModel.DataAnnotations;

namespace RUG.WebEng.Transactions.Models;

public class Transaction
{
    [Key]
    public long Id { get; set; }

    public required DateTime Date { get; set; }

    public required int ClientId { get; set; }

    public required decimal Amount { get; set; }

    public string? MerchantCity { get; set; }

    public string? MerchantState { get; set; }
}
