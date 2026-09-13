using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Infrastructure.Repositories;

namespace CryptoTrackingSystem.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Prices = new PriceRepository(context);
    }

    public IPriceRepository Prices { get; }

    public void Dispose() => _context.Dispose();
}
