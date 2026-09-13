using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Infrastructure.Data;
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

}
