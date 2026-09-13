using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Infrastructure.Data;
using CryptoTrackingSystem.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoTrackingSystem.Infrastructure.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly AppDbContext _context;

    public PriceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal?> GetAveragePriceAsync(string symbol, DateTime from, DateTime to)
    {
        var records = await _context.PriceRecords
            .Where(r => r.Symbol == symbol && r.Timestamp >= from && r.Timestamp <= to)
            .ToListAsync();

        if (records.Count == 0)
            return null;

        return records.Average(r => r.Price);
    }

    public async Task AddPriceRecordAsync(string symbol, decimal price, DateTime timestamp)
    {
        _context.PriceRecords.Add(new PriceRecord
        {
            Symbol = symbol,
            Price = price,
            Timestamp = timestamp
        });

        await _context.SaveChangesAsync();
    }
}
